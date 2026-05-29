using BookStore.BL;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.SignalR;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("http://127.0.0.1:5500") 
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials(); // Required for SignalR 
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<BookStoreUser>();
builder.Services.AddSignalR();
builder.Services.AddHostedService<WeeklyEventService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(); 

// Static file configuration
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"uploadedFiles")),
    RequestPath = new PathString("/Images")
});

// SignalR Hub mapping
app.MapHub<NotificationHub>("/notifications");
app.MapControllers();

app.Run();
