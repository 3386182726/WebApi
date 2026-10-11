using Common.Abstractions;
using Common.Service;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using NoteService.Data;
using NoteService.Modules.Notes.Consumer;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;
using NoteService.Modules.Notes.Repository;
using NoteService.Modules.Notes.Service;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IService<Note, NoteResponse>, NoteService.Modules.Notes.Service.NoteService>();
builder.Services.AddScoped<IRepository<Note, NoteResponse>, NoteRepository>();

builder.Services.AddScoped<INoteCategoryService, NoteCategoryService>();
builder.Services.AddScoped<IRepository<NoteCategory, NoteCategoryResponse>, NoteCategoryRepository>();

builder.Services.AddScoped<IUploadService, UploadService>();

builder.Services.AddControllers();
builder.Services.AddDbContext<NoteDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );
builder.WebHost.UseUrls("http://0.0.0.0:8080");
builder.Services
           .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
           .AddJwtBearer(options =>
           {
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,

                   ValidIssuer = builder.Configuration["Jwt:Issuer"],
                   ValidAudience = builder.Configuration["Jwt:Audience"],

                   IssuerSigningKey = new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
               };
           });
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddAuthorization();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy())
    .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")!);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserUpsertConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbit = builder.Configuration.GetSection("RabbitMq");
        cfg.Host(
            rabbit["Host"] ?? "localhost",
            rabbit["VirtualHost"] ?? "/",
            h =>
            {
                h.Username(rabbit["Username"] ?? "guest");
                h.Password(rabbit["Password"] ?? "guest");
            });
        cfg.ConfigureEndpoints(context);  // 根据 Consumer 创建队列并绑定
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();

app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
