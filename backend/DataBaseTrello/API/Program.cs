
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
using API.Middleware;
using OpenTelemetry.Resources;
using API.Services.BackGroundServices;
using API.Services.Application.Implementations;
using API.Exceptions.ContextCreator;
using API.Services.Helpers.Implementations;
using API.Services.Helpers.Interfaces;
using API.Services.Application.Interfaces;
using API.Repositories.Uof;
using API.Repositories.Queries;
using API.Repositories.Implementations;
using API.Repositories.Interfaces;
using API.Repositories.Queries.Implementations;
using API.Repositories.Queries.Intefaces;
using API.Repositories.Queries.Interfaces;
using DataBaseInfo.models;
using Microsoft.AspNetCore.Identity;

// Создаёт билдер для настройки приложения
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IErrorContextCreatorFactory, ErrorContextCreatorFactory>();

builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(ctx.Configuration));

// Добавление секции AuthSettings в Сервисы Билдера
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.Configure<TLLSettings>(builder.Configuration.GetSection("TLLSettings"));
builder.Services.Configure<ImageKitSettings>(builder.Configuration.GetSection("ImageKitSettings"));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddSingleton(new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? "localhost:6379")));
//builder.Services.AddSingleton<IRedisService,RedisService>();


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
//serv
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IHashService,HashService>();
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IJWTService,JWTService>();
builder.Services.AddScoped<IGetPagesService,GetPagesService>();
builder.Services.AddScoped<IProjectService,ProjectService>();
builder.Services.AddScoped<IBoardService,BoardService>();
builder.Services.AddScoped<IImageService,ImageService>();
builder.Services.AddScoped<ISessionService,SessionService>();
//repos
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IMembersOfBoardRepository, MembersOfBoardRepository>();
builder.Services.AddScoped<IBoardRepository, BoardRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Queries
builder.Services.AddScoped<IBoardQueries, BoardQueries>();
builder.Services.AddScoped<IUserQueries, UserQueries>();
builder.Services.AddScoped<IProjectQueries, ProjectQueries>();
builder.Services.AddScoped<IProjectUserQueries, ProjectUserQueries>();
builder.Services.AddScoped<ISessionQueries, SessionQueries>();

builder.Services.AddScoped<IQueries,Queries>();


//swagger
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
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

app.UseExceptionHandling();
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("MyPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseSessionValidation();
app.MapControllers();

app.Run();