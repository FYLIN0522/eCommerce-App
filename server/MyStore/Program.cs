// Additional using declarations
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using MyStore.Data;
using Repositories.Concrete;
using Repositories.Abstract;
using MyStore.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyStore.Models;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using MyStore.Repositories.Concrete;
using MyStore.Service;
using MyStore.Repositories.Abstract;
using System.Text.Json.Serialization;
using MyStore.Services;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//builder.Services.AddHttpContextAccessor();

// UserContext
//builder.Services.AddSqlite<UserContext>("Data Source=User.db");
// Add the ProductContext
builder.Services.AddSqlite<StoreContext>("Data Source=Store.db");
// Add the PromotionsContext
builder.Services.AddSqlite<PromotionsContext>("Data Source=Promotions.db");

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ImageService>();
builder.Services.AddScoped<PasswordService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:5200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// ========= SwaggerGen ======================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MyStore API",
        Description = "Online Shopping",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
                {
                        new OpenApiSecurityScheme
                        {
                                Reference = new OpenApiReference
                                {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                }
                        },
                        new string[] {}
                }
        });

});

// ========= JWT Authentication ======================================================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(option =>
{
    //option.RequireHttpsMetadata = false;
    //option.SaveToken = true;

    var token = builder.Configuration.GetSection("JWT");

    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(token["secret"])),
        ValidIssuer = token["issuer"],
        ValidateIssuer = true,
        ValidateAudience = false,
    };
});



var app = builder.Build();

// Enable CORS
app.UseCors("AllowReactApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add the CreateDbIfNotExists method call
app.CreateDbIfNotExists();
app.MapGet("/", () => @"Store API. Navigate to /swagger to open the Swagger test UI.");

app.Run();
