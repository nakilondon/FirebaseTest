using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using AuthSeries.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth Series", Version = "v1.0.0" });

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };

                c.AddSecurityRequirement(securityRequirement);

            });
//local auth
builder.Services.AddTransient<ITokenService, TokenService>();

// here we specify our authentication settings to validate the JWT token
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer("AuthDemo", opt =>
//    {
//        opt.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:AuthDemo:ValidIssuer"],
//            ValidAudience = builder.Configuration["Jwt:AuthDemo:ValidAudience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:AuthDemo:Key"]))
//        };
//});


// firebase auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer( opt =>
    {
        opt.Authority = builder.Configuration["Jwt:Firebase:ValidIssuer"];
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Firebase:ValidIssuer"],
            ValidAudience = builder.Configuration["Jwt:Firebase:ValidAudience"]
        };
    });

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("Verified", policy =>
            policy.RequireClaim("verified", "true"));
    });

//builder.Services.AddAuthorization(opt =>
//{
//    opt.DefaultPolicy = new AuthorizationPolicyBuilder()
//    .AddAuthenticationSchemes("Firebase", "AuthDemo")
//    .RequireAuthenticatedUser()
//    .Build();
//});


FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(@"Config\credentials.json")
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
