using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Web_XuongMay.Data;
using Web_XuongMay.Models;
using Web_XuongMay.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<MyDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyDB"));
});

builder.Services.AddScoped<ILoaiRepository, LoaiRepository>();

builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("AppSettings"));

var secretKey = builder.Configuration["AppSettings:SecretKey"];
var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,

        ValidateIssuerSigningKey = false,
        IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
        ClockSkew = TimeSpan.Zero
    };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
