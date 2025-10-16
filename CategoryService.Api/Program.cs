using CategoryService.Application.Interfaces.Repositories;
using CategoryService.Application.Interfaces.Services;
using CategoryService.Application.Mapping;
using CategoryService.Infrastructure.Configuration;
using CategoryService.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


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
builder.Services.AddScoped<ICategoryService, CategoryService.Application.Services.CategoryService>();


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


// Authentication & Authorization
builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),

        };
    });
builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
