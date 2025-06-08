
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using DataBaseInfo.Services;
using System.Text;

//Создаёт билдер для настройки приложения
var builder = WebApplication.CreateBuilder(args);
//Добавление секции AuthSettings в Сервисы Билдера
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));

// Регистрация фабрики контекста
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
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
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(authSettings.SecretKey))
        };
       
    });




// Другие сервисы
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<HashService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JWTServices>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddPolicy("MyPolicy", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}


app.UseHttpsRedirection();
app.UseRouting();
// Включение CORS
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("MyPolicy");
app.MapControllers();

app.Run();