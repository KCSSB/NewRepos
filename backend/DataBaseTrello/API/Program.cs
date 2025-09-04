
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Configuration;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using API.Exceptions.Context;
using System.Net;
using Serilog;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using API.Middleware;
using OpenTelemetry.Resources;
using API.Services.BackGroundServices;
using API.Services.Application.Implementations;
using API.Exceptions.ContextCreator;
using API.Services.Helpers.Implementations;
using API.Services.Helpers.Interfaces;
using API.Services.Application.Interfaces;
using API.Services.Helpers.Interfaces.Redis;

// Создаёт билдер для настройки приложения
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IErrorContextCreatorFactory, ErrorContextCreatorFactory>();

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// Добавление секции AuthSettings в Сервисы Билдера
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.Configure<TLLSettings>(builder.Configuration.GetSection("TLLSettings"));
builder.Services.Configure<ImageKitSettings>(builder.Configuration.GetSection("ImageKitSettings"));
// Регистрация фабрики контекста
builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddSingleton(new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379")));
builder.Services.AddSingleton<IRedisService,RedisService>();


//Регистрация сервиса для очистки рефреш токенов:
builder.Services.AddHostedService<SessionsCleaner>();
//Регистрация сервиса валидации Токенов

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {

        var authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>(); 
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(authSettings.SecretKey))
        };
       
    });
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyService"))
            .AddAspNetCoreInstrumentation()
            .AddConsoleExporter();
    });


// Другие сервисы
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IHashService,HashService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IJWTService,JWTService>();
builder.Services.AddScoped<IGetPagesService,GetPagesService>();
builder.Services.AddScoped<IProjectService,ProjectService>();
builder.Services.AddScoped<IBoardService,BoardService>();
builder.Services.AddScoped<IImageService,ImageService>();
builder.Services.AddScoped<ISessionService,SessionService>();
builder.Services.AddSwaggerGen(c =>
{
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173", "https://localhost:7098")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});




var app = builder.Build();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Connection string: {connectionString?.Replace("Password=", "Password=***")}");

if (string.IsNullOrEmpty(connectionString))
    throw new AppException(new ErrorContext("Program",
                           "Program",
                           (HttpStatusCode)1001,
                           $"Произошла ошибка в момент подключения к базе данных"));


using (var scope = app.Services.CreateScope())
{
    var service = scope.ServiceProvider;
    var dbFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
    using(var dbContext = dbFactory.CreateDbContext())
    {
        var logger = service.GetService<ILogger<Program>>();
        try
    {
        dbContext.Database.Migrate();
            
            logger.LogInformation("Миграции были успешно применены");
    }
    catch (Exception ex)
    {
            throw new AppException(new ErrorContext("Program",
                               "Program",
                               (HttpStatusCode)1001,
                               $"Произошла ошибка при попытке применить миграции"));

        }
    }
}
   

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandling();
app.UseSessionValidation();
app.MapControllers();

app.Run();