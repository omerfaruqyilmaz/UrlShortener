using Microsoft.EntityFrameworkCore;
using UrlShortener.DataAccess;

namespace UrlShortener.Extensions
{
	public static class MigrationExtensions
	{
		public static void ApplMigrations(this WebApplication app)
		{
			using var scope = app.Services.CreateScope();

			var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

			dbContext.Database.Migrate();
		}
	}
}
