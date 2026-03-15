using Microsoft.EntityFrameworkCore;
using ShopManagementAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

// Configure DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add Controllers
builder.Services.AddControllers();

// Enable Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Authorization middleware (optional but recommended)
app.UseAuthorization();

// Map controller routes
app.MapControllers();

app.Run();