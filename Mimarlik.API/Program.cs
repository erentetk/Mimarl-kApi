using Microsoft.EntityFrameworkCore;
using Mimarlik.Infrastructure.Data;
using Mimarlik.Core.Interfaces;
using Mimarlik.Infrastructure.Repositories;
using Mimarlik.Application.Interfaces;
using Mimarlik.Application.Services;
using Mimarlik.Infrastructure.Services;
using Mimarlik.Application.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Ignore null values in JSON responses
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        // Handle circular references
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        // Set max depth to prevent stack overflow
        options.JsonSerializerOptions.MaxDepth = 32;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<MimarlikDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Repository pattern
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<ITranslationService, TranslationService>();
builder.Services.AddScoped<IDeletionService, DeletionService>();

// Infrastructure services
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<DataSeedService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Configure static files - serve from Uploads directory without /Uploads prefix
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "Uploads")),
    RequestPath = ""
});

app.UseAuthorization();
app.MapControllers();

app.Run();
