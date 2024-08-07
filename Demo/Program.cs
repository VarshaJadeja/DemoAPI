using Demo.Entities;
using Demo.Configuration;
using Demo.Services.Services;
using Demo.Repositories;
using Demo.Entities.Entities;
using MongoDB.Driver;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Demo.ActionFilter;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using Boxed.Mapping;
using Demo.Entities.ViewModels;
using Demo.Entities.Mapper;
using Demo.Middleware;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.Mongo.Migration.Strategies;
using Demo;
using CrystalQuartz.AspNetCore;
using Quartz.Impl;
using Quartz;
using System.Collections.Specialized;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/Log.txt", rollingInterval: RollingInterval.Day)
    //.WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();
builder.Services.Configure<ProductDBSettings>(
      builder.Configuration.GetSection("ProductDatabase"));

    // Retrieve connection string 
    var connectionString = builder.Configuration.GetSection("ProductDatabase")
        .Get<ProductDBSettings>()?.ConnectionString;
    if (string.IsNullOrEmpty(connectionString))
    {
        throw new ArgumentNullException(nameof(connectionString), "Connection string is not configured.");
    }
    // Retrieve database name 
    var databaseName = builder.Configuration.GetSection("ProductDatabase")
        .Get<ProductDBSettings>()?.DatabaseName;
    if (string.IsNullOrEmpty(databaseName))
    {
        throw new ArgumentNullException(nameof(databaseName), "Database name is not configured.");
    }

    // Register MongoDB services
    builder.Services.AddSingleton<IMongoClient>(serviceProvider =>
    {
        var mongoClientSettings = MongoClientSettings.FromConnectionString(connectionString);
        return new MongoClient(mongoClientSettings);
    });

    builder.Services.AddScoped(sp =>
    {
        var client = sp.GetRequiredService<IMongoClient>();
        return client.GetDatabase(databaseName);
    });
    var key = builder.Configuration.GetValue<string>("ApiSetting:Secret");
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateIssuer = false,
            ValidateAudience = false,

        };
    });

    builder.Services.AddScoped<CustomsDeclarationsContext>();
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IEncryptionService, EncryptionService>();
    builder.Services.AddScoped<IEmailService, EmailService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddSingleton<ExceptionFilter>();

    builder.Services.AddHangfire(config =>
    {
        var mongoUrlBuilder = new MongoUrlBuilder(connectionString);
        var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());

        var storageOptions = new MongoStorageOptions
        {
            MigrationOptions = new MongoMigrationOptions
            {
                MigrationStrategy = new MigrateMongoMigrationStrategy(),
                BackupStrategy = new CollectionMongoBackupStrategy()
            }
        };
        GlobalConfiguration.Configuration
    .UseMongoStorage(mongoClient, databaseName, storageOptions);
    });

    builder.Services.AddSingleton<ISchedulerFactory>(sp =>
    {
        var properties = new NameValueCollection
        {
            { "org.quartz.scheduler.instanceName", "MyScheduler" },
            { "org.quartz.scheduler.instanceId", "AUTO" },
            { "org.quartz.jobStore.class", "org.terracotta.quartz.TerracottaJobStore" },
            { "org.quartz.jobStore.clusterCheckinInterval", "20000" },
            { "org.quartz.jobStore.isClustered", "true" },
            { "org.quartz.threadPool.class", "org.quartz.simpl.SimpleThreadPool" },
            { "org.quartz.threadPool.threadCount", "5" },
            { "org.quartz.threadPool.threadPriority", "5" }
        };
        return new StdSchedulerFactory(properties);
    });
    builder.Services.AddSingleton<QuartzManager>();

    // Add Hangfire server
    builder.Services.AddHangfireServer();
    builder.Services.AddScoped<Scheduler>();
    builder.Services.AddControllers(options => {
        options.Filters.Add(typeof(ExceptionFilter));
        options.Filters.Add(typeof(LogActionFilter));
        options.CacheProfiles.Add("Default30",
            new CacheProfile()
            {
                Duration = 30
            });
    });

    //AutoMapping
    //builder.Services.AddAutoMapper(typeof(Program));
    builder.Services.AddAutoMapper(typeof(AutoMapping));

    builder.Services.AddTransient<IMapper<ProductDetailsViewModel, ProductDetails>, ProductDetailMapper>();
    builder.Services.AddMemoryCache();

    // cors to allow access to frontend
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder =>
        {
            builder.WithOrigins("http://localhost:4200")
                   .AllowAnyMethod()    
                   .AllowAnyHeader();
        });
    });
    var emailConfig = builder.Configuration
            .GetSection("EmailConfiguration")
            .Get<SendEmailModel>();

    builder.Services.AddSingleton(emailConfig);

builder.Services.AddControllersWithViews();

builder.Services.AddSwaggerGen(option =>
    {
        option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "gdfgdf",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Scheme = "Bearer"
        });
        option.AddSecurityRequirement(new OpenApiSecurityRequirement()
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    },
                    Scheme = "oauth2",
                    Name = "Bearer",
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
    });
    var scheduler = new StdSchedulerFactory().GetScheduler().Result;
    scheduler.Start().Wait();
    var app = builder.Build();
    app.UseCrystalQuartz(() => scheduler);
    app.UseCors();
    app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
app.UseSwagger();
app.UseSwaggerUI();
app.UseStaticFiles();
app.UseMiddleware();
app.UseGlobalExceptionMiddleware();
app.UseAuthentication();
app.UseHangfireDashboard();
app.UseAuthorization();
app.UseEndpoints(endpoints => 
{
    endpoints.MapControllers();
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=ResetPassword}/{id?}");

app.Run();

public partial class Program { }