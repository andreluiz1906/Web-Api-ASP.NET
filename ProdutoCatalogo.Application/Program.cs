using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProdutoCatalogo.Domain.Interfaces.Repositories;
using ProdutoCatalogo.Domain.Interfaces.Services;
using ProdutoCatalogo.Infra.Configurations.Headers;
using ProdutoCatalogo.Infra.Configurations.Swagger;
using ProdutoCatalogo.Infra.DataAccess;
using ProdutoCatalogo.Infra.Interfaces;
using ProdutoCatalogo.Infra.Repositories;
using ProdutoCatalogo.Service.Services;
using ProdutoCatalogo.Shared.Configurations;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args); 
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
string connMySQL = builder.Configuration.GetSection("ConnectionStrings").GetSection("MySQLDatabase").Value;

builder.Services.AddSingleton<IConnectionMySQL>(new ConnectionMySQL(connMySQL));
builder.Services.AddSingleton(jwtSettings);

builder.Services.Configure<RouteOptions>(options => {
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SwaggerSecurity", policy =>
    {
        policy.AuthenticationSchemes.Add(
            IdentityServerAuthenticationDefaults.AuthenticationScheme);
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api1");
    });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder => builder.AllowAnyMethod()
                                                      .AllowAnyHeader()
                                                      .AllowCredentials()
                                                      .SetIsOriginAllowed(origin => true));
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
});
builder.Services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
builder.Services.AddScoped<HeaderValidationFilter>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuth, AuthRepository>();
builder.Services.AddScoped<IPermission, PermissionRepository>();
builder.Services.AddScoped<ICategory, CategoryRepository>();
builder.Services.AddScoped<IUser, UserRepository>();
builder.Services.AddScoped<IProduct, ProductRepository>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Catálogo de Produtos - API", Version = "v1" });
    options.DocumentFilter<SwaggerDocumentFilter>();

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (!File.Exists(xmlPath))
    {
        // Se não existir, cria um novo arquivo XML
        using (var writer = new StreamWriter(xmlPath))
        {
            writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            writer.WriteLine("<doc></doc>");
        }
    }

    options.IncludeXmlComments(xmlPath);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Acesso protegido utilizando o accessToken obtido em \"api/auth\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var services = new ServiceCollection();
services.AddHttpContextAccessor();
var serviceProvider = services.BuildServiceProvider();

var app = builder.Build();
app.UseCors("CorsPolicy");
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
