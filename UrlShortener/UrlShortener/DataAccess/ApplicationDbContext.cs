using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using UrlShortener.Entities;
using UrlShortener.Services;

namespace UrlShortener.DataAccess
{
	public class ApplicationDbContext:DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
		public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ShortenedUrl>(builder =>
			{
				builder.Property(s => s.Code).HasMaxLength(UrlShortiningService.NumberOfCharsInShortLink);
				builder.HasIndex(s => s.Code).IsUnique();
			});
		}

	}
}
