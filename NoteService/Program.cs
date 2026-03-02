using Common.Repository;
using Common.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NoteService.Data;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;
using NoteService.Modules.Notes.Repository;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IService<Note, NoteResponse>, NoteService.Modules.Notes.Service.NoteService>();
builder.Services.AddScoped<IRepository<Note, NoteResponse>, NoteRepository>();
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

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseStaticFiles();
app.Run();

public partial class Program { }
