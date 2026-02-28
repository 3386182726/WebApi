using Common.Repository;
using Common.Service;
using Microsoft.EntityFrameworkCore;
using NoteService.Data;
using NoteService.Modules.Notes.Dto;
using NoteService.Modules.Notes.Model;
using NoteService.Modules.Notes.Repository;

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
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
