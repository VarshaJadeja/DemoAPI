using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers;


[Route("api/[controller]")]
[ApiController]
public class SchedulerController : Controller
{
    private readonly QuartzManager _quartzManager;
    public readonly Scheduler scheduler;

    public SchedulerController(Scheduler scheduler, QuartzManager quartzManager)
    {
        this.scheduler = scheduler;
        _quartzManager = quartzManager;
    }

    [HttpGet("hangfire-schedule-print-time")]
    public IActionResult SchedulePrintTime(string cron)
    {
        scheduler.WriteTime(cron);
        return Ok("Print current time job scheduled successfully.");
    }

    [HttpPost("hangfire-pause-job")]
    public IActionResult PauseJob(string jobId)
    {
        scheduler.PauseJob(jobId);
        return Ok("Job paused successfully.");
    }

    [HttpPost("hangfire-resume-job")]
    public IActionResult ResumeJob(string jobId)
    {
        scheduler.ResumeJob(jobId);
        return Ok("Job resumed successfully.");
    }

    [HttpPost("hangfire-stop-job")]
    public IActionResult StopJob(string JobId)
    {
        scheduler.StopJob(JobId);
        return Ok("Job stopped successfully.");
    }
    [HttpPost("hangfire-pauseForDuration-job")]
    public async Task<IActionResult> PauseForDurationAsync(string JobId)
    {
        var duration = TimeSpan.FromMinutes(2);
        await scheduler.PauseJobForDuration(JobId, duration);
        return Ok("Job Paused For Duration successfully.");
    }
    [HttpPost("quartz-start")]
    public async Task<ActionResult> StartJob(string cronexpression)
    {
        await _quartzManager.StartAsync(cronexpression);
        return Ok("Job started successfully.");
    }

    [HttpPost("quartz-stop")]
    public async Task<ActionResult> StopJob()
    {
        await _quartzManager.StopAsync();
        return Ok("Job stopped successfully.");
    }
}
