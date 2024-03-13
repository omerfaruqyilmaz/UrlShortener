using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UrlShortener.DataAccess;
using UrlShortener.Entities;
using UrlShortener.Extensions;
using UrlShortener.Models;
using UrlShortener.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<UrlShortiningService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	app.ApplMigrations();
}

//public record ShortenUrlRequest(string Url);

app.MapPost("shorten", async (
	ShortenUrlRequest request,
	UrlShortiningService urlShorteningService,
	ApplicationDbContext dbContext,
	HttpContext httpContext) =>
{
	if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
	{
		return Results.BadRequest("The specified URL is invalid.");
	}

	var code = await urlShorteningService.GenerateUniqueCode();

	var shortenedUrl = new ShortenedUrl
	{
		Id = Guid.NewGuid(),
		LongUrl = request.Url,
		Code = code,
		ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/{code}",
		CreatedOnUtc = DateTime.UtcNow
	};

	dbContext.ShortenedUrls.Add(shortenedUrl);

	await dbContext.SaveChangesAsync();

	return Results.Ok(shortenedUrl.ShortUrl);
});


app.MapGet("{code}", async (string code, ApplicationDbContext dbContext) =>
{
	var shortenedUrl = await dbContext.ShortenedUrls.FirstOrDefaultAsync(s => s.Code == code);

	if (shortenedUrl is null)
	{
		return Results.NotFound();
	}

	return Results.Redirect(shortenedUrl.LongUrl);
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
