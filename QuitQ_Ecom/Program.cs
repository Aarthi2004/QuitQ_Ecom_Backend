using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuitQ_Ecom;
using QuitQ_Ecom.Context;
using QuitQ_Ecom.Interfaces;
using QuitQ_Ecom.Mapper;
using QuitQ_Ecom.Repositories;
using QuitQ_Ecom.Repository;
using QuitQ_Ecom.Service;
using QuitQ_Ecom.Services;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

// Configure EF Core with SQL Server connection string from appsettings.json
builder.Services.AddDbContext<QuitQEcomContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add AutoMapper profile
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<MappingProfile>();
});

// Dependency Injection registrations
builder.Services.AddScoped<IBrandRepository, BrandRepositoryImpl>();
builder.Services.AddScoped<IOrder, OrderRepositoryImpl>();
builder.Services.AddScoped<IOrderItem, OrderItemRepositoryImpl>();
builder.Services.AddScoped<IPayment, PaymentRepositoryImpl>();
builder.Services.AddScoped<IShipper, ShipperRepositoryImpl>();
// Register the Service
// Register UserAddressRepository and its interface
builder.Services.AddScoped<IUserAddressRepository, UserAddressRepository>();

// Register UserAddressService and its interface
builder.Services.AddScoped<IUserAddressService, UserAddressService>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IStateRepository, StateRepository>();
builder.Services.AddScoped<IStateService, StateService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<ICart, CartRepositoryImpl>();
builder.Services.AddScoped<ICartService, CartServiceImpl>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IWishlistService, WishlistServiceImpl>();
builder.Services.AddScoped<ISubCategory, SubCategoryRepositoryImpl>();
builder.Services.AddScoped<ISubCategoryService, SubCategoryServiceImpl>();
builder.Services.AddScoped<ICategory, CategoryRepositoryImpl>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IStore, StoreRepositoryImpl>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IGenderRepository, GenderRepository>();
builder.Services.AddScoped<IGenderService, GenderService>();
builder.Services.AddScoped<IProduct, ProductRepositoryImpl>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IUser, UserRepositoryImpl>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Configure CORS policy to allow React frontend (adjust origin as needed)
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCORS", policy =>
        policy.WithOrigins("http://localhost:3000")  // Only allow React frontend origin
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// Configure JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.IncludeErrorDetails = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = "UserId"
    };
});

// Authorization policies (if needed)
builder.Services.AddAuthorization();

// Swagger configuration for JWT bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter 'Bearer {token}' in the Authorization header.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference=new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseRouting();

app.UseCors("DefaultCORS");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
