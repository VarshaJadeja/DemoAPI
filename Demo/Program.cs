using Demo.Entities;
using Demo.Configuration;
using Demo.Repositories;
using Demo.Services;
using Demo.Repositories.Repositories;
using Demo.Entities.Entities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Demo;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Core.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Demo.ActionFilter;
using Serilog;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Boxed.Mapping;
using Demo.Entities.ViewModels;
using Demo.Entities.Mapper;
using Demo.Middleware;
using Microsoft.AspNetCore.Builder;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/Log.txt", rollingInterval: RollingInterval.Day)
    //.WriteTo.Console()
    .CreateLogger();
try
{
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

    //builder.Services.AddCustomMongoDBContext(); 
    builder.Services.AddScoped<CustomsDeclarationsContext>();
    builder.Services.AddScoped<IRepository, Repository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddSingleton<ExceptionFilter>();
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
    //builder.Services.AddCors(options =>
    //{
    //    options.AddDefaultPolicy(builder =>
    //    {
    //        builder.WithOrigins("http://localhost:4200/")
    //               .AllowAnyHeader()
    //               .AllowAnyMethod();
    //    });
    //});
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

    var app = builder.Build();
    app.UseCors();
    //app.UseHttpsRedirection();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    app.UseMiddleware();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}

