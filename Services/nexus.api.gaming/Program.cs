
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
//using nexus.api.Consumers.common;

using nexus.common;
using nexus.common.dal;
using System.Text;

using ConfigurationManager = nexus.web.auth.ConfigurationManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("V1", new OpenApiInfo
    {
        Version     = "V1",
        Title       = "NextNet WebAPI",
        Description = "NextNet WebAPI"
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Name         = "Authorization",
        Description  = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id   = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

//builder.Services.AddScoped<ICacheService, CacheService>();

//builder.Services.AddDbContext<NexusDB>();

builder.Services.AddAuthentication(opt => {
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = ConfigurationManager.AppSetting["JWT:ValidIssuer"],
            ValidAudience            = ConfigurationManager.AppSetting["JWT:ValidAudience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ConfigurationManager.AppSetting["JWT:Secret"]))
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline to be used in dev and release
app.UseSwagger();
app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/V1/swagger.json", "NextNet WebAPI"); });


//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new Info { Title = "API WSVAP (WebSmartView)", Version = "v1" });
//    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); //This line
//});

//app.UseHttpsRedirection();


app.UseDeveloperExceptionPage();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseCors(x => x
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader());

//setting.GetSettings();
//var enumHelper = new EnumCodeHelper();

app.Run();


//{
//    "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
//  "AllowedHosts": "*",
//  "RedisURL": "127.0.0.1:6379",
//  "JWT": { "ValidAudience": "http://localhost:7299","ValidIssuer": "http://localhost:7299", "Secret": "#Z2z8vcVnj#WoCs3" },
//  "ConnectionStrings": { "DefaultConnection": "Server=58.105.169.184;Database=Kemps;User Id=Nexus;Password=N3x9s1!3" }
//}

