using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChatDbContext>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSignalR();
builder.Services.AddControllers();

var app = builder.Build();

app.MapHub<ChatHub>("/chatHub");
app.MapControllers();

app.Run();
