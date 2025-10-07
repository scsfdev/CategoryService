using CategoryService.Application.Interfaces;
using CategoryService.Application.Mapping;
using CategoryService.Domain.Interfaces;
using CategoryService.Infrastructure.Configuration;
using CategoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection("RabbitMQ")
);


// Register DbContext with SQL Server provider.
builder.Services.AddDbContext<CategoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CategoryDb")));

// Register repository (from Infrastructure Layer)
builder.Services.AddScoped<ICategoryRepository, CategoryService.Infrastructure.Repositories.CategoryRepository>();


// Register service (from Application Layer)
builder.Services.AddScoped<ICategoryService, CategoryService.Infrastructure.Services.CategoryService>();


// Register AutoMapper
builder.Services.AddAutoMapper(p =>
{
    p.AddProfile(new MappingProfile());
});

// Add Hosted Service for RabbitMQ Subscriber
builder.Services.AddHostedService<CategoryService.Infrastructure.Services.CategoryValidationSubscriber>();

// Controller and Swagger
builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
