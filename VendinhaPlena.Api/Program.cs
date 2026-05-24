using Microsoft.EntityFrameworkCore;
using VendinhaPlena.Infrastructure.Data;
using VendinhaPlena.Application.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<VendinhaDbContext>(options =>
    options.UseSqlite("Data Source=vendinha.db"));


builder.Services.AddScoped<ClienteService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();