
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using StackExchange.Redis;
namespace Demo
{
    public class Scheduler
    {
        private readonly IMongoCollection<HangFireLog> _collection;
        private readonly IMongoCollection<HangFireFirstJobLog> _firstcollection;
        private readonly string _machineIdentifier;
        private static readonly HashSet<string> PausedJobs = new HashSet<string>();
        public Scheduler(IMongoDatabase database)
        {
            _collection = database.GetCollection<HangFireLog>("HangFireLog");
            _firstcollection = database.GetCollection<HangFireFirstJobLog>("HangFireFirstJobLog");
            _machineIdentifier = Environment.MachineName;
        }
        public void WriteTime(string cron)
        {
            string id = Guid.NewGuid().ToString();
            string jobName = $"{_machineIdentifier} - {id}";
            RecurringJob.AddOrUpdate(jobName, () => WriteTimeToFile(jobName), cron);
        }
        [CanBePaused]
        public void WriteTimeToFile(string jobName)
        {
            if (IsFirstExecution(jobName))
            {
                Console.WriteLine("Hello, Hangfire.NET!" + DateTime.Now);

                HangFireFirstJobLog newData = new HangFireFirstJobLog
                {
                    LogName = $"{_machineIdentifier}",
                    LogTime = DateTime.Now,
                    JobId = jobName,
                };
                try
                {
                    _firstcollection.InsertOneAsync(newData);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
        }
        public bool IsFirstExecution(string jobId)
        {
            var filter = Builders<HangFireLog>.Filter.Eq(x => x.JobId, jobId);
            var count = _collection.Find(filter).CountDocuments();

            HangFireLog firstdata = new HangFireLog
            {
                LogName = $"{_machineIdentifier}",
                LogTime = DateTime.Now,
                JobId = jobId,
            };
            _collection.InsertOneAsync(firstdata);
            return count == 0;
        }
        public void StopJob(string JobId)
        {
            RecurringJob.RemoveIfExists(JobId);
        }
        public void PauseJob(string jobId)
        {
            PausedJobs.Add(jobId);

            var filter = Builders<HangFireFirstJobLog>.Filter.Eq(x => x.JobId, jobId);
            var combinedUpdate = Builders<HangFireFirstJobLog>.Update
             .Combine(
                 Builders<HangFireFirstJobLog>.Update.Set(x => x.PauseTime, DateTime.Now)
             );
            _firstcollection.FindOneAndUpdateAsync(filter, combinedUpdate);
        }

        public void ResumeJob(string jobId)
        {
            PausedJobs.Remove(jobId);

            var filter = Builders<HangFireFirstJobLog>.Filter.Eq(x => x.JobId, jobId);
            var combinedUpdate = Builders<HangFireFirstJobLog>.Update
             .Combine(
                 Builders<HangFireFirstJobLog>.Update.Set(x => x.ResumeTime, DateTime.Now)
             );

            _firstcollection.FindOneAndUpdateAsync(filter, combinedUpdate);
        }
       
        public static bool IsJobPaused(string jobId)
        {
            lock (PausedJobs) 
            {
                return PausedJobs.Contains(jobId);
            }
        }
        public async Task PauseJobForDuration(string jobId, TimeSpan duration)
        {
            PauseJob(jobId); // Pause the job
            await Task.Delay(duration); // Wait for the specified duration
            ResumeJob(jobId); // Resume the job after the delay
        }

    }

    public class CanBePausedAttribute : JobFilterAttribute, IServerFilter
    {
        public void OnPerforming(PerformingContext filterContext)
        {
            var jobId = filterContext.Job.Arguments[0].ToString().Trim('"');
            // Check if the job ID is in the paused jobs list
            if (Scheduler.IsJobPaused(jobId))
            {
                filterContext.Canceled = true; // Skip execution if paused
            }
        }

        public void OnPerformed(PerformedContext filterContext)
        {
        }
    }
    public class HangFireLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string JobId { get; set; }
        public string? LogName { get; set; }
        public DateTime LogTime { get; set; }
    }

    public class HangFireFirstJobLog
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string JobId { get; set; }
        public string? LogName { get; set; }
        public DateTime LogTime { get; set; }
        public DateTime PauseTime { get; set; }
        public DateTime ResumeTime { get; set; }
    }
}
