using Microsoft.EntityFrameworkCore;
using WebApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowViteClient", policy =>
	{
		policy
			.WithOrigins("http://localhost:5173") // React dev server origin
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

// Add services to the container.
builder.Services.AddHttpClient();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("InMemDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowViteClient");

app.UseAuthorization();

app.MapControllers();

app.Run();
