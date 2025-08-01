using Microsoft.EntityFrameworkCore;
using OnlineQuizApplicationAPI.Models;
using OnlineQuizApplicationAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
string sql = builder.Configuration.GetConnectionString("DbConn");
builder.Services.AddDbContext<QuizDbContext>(op =>
{
    op.UseSqlServer(sql);
});
builder.Services.AddTransient<ServicesAPI>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
