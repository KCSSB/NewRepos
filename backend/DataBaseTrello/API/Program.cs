
using DataBaseInfo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore;
using DataBaseInfo.Services;

//Создаёт билдер для настройки приложения
var builder = WebApplication.CreateBuilder(args);

// Регистрация фабрики контекста
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Настройка CORS


// Другие сервисы
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<JWTServices>();
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options => options.AddPolicy("MyPolicy", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

app.UseRouting();

// Включение CORS
app.UseAuthorization();
app.UseCors("MyPolicy");
app.MapControllers();

app.Run();