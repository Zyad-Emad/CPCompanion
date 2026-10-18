using CPCompanion.API.Filters;
using CPCompanion.API.Middlewares;
using CPCompanion.Application.Interfaces;
using CPCompanion.Application.Mappings;
using CPCompanion.Application.Validators.Goal;
using CPCompanion.Infrastructure.BackgroundServices;
using CPCompanion.Infrastructure.ExternalServices;
using CPCompanion.Infrastructure.Identity;
using CPCompanion.Infrastructure.Persistence;
using CPCompanion.Infrastructure.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddValidatorsFromAssemblyContaining<CreateGoalValidator>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<ISyncService, SyncService>();
builder.Services.AddScoped<IStatsService, StatsService>();
builder.Services.AddScoped<ValidationFilter>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<BackgroundServiceSettings>(
    builder.Configuration.GetSection("BackgroundServiceSettings"));
builder.Services.AddHostedService<CodeforcesSyncBackgroundService>();
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddHttpClient<ICodeforcesApiService, CodeforcesApiService>(client =>
{
    client.BaseAddress = new Uri("https://codeforces.com/api/");
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddAuthentication(options => 
{ 
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(options => 
{ options.TokenValidationParameters = 
    new TokenValidationParameters 
    {   
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)) 
    };
});
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
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