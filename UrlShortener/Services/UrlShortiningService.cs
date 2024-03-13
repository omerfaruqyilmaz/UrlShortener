using Microsoft.EntityFrameworkCore;
using System;
using UrlShortener.DataAccess;

namespace UrlShortener.Services
{
	public class UrlShortiningService
	{
		public const int NumberOfCharsInShortLink = 7;
		public const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		private readonly Random _random = new Random();
		private readonly ApplicationDbContext _dbContext;

        public UrlShortiningService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<string> GenerateUniqueCode()
		{
			var codeChars = new char[NumberOfCharsInShortLink];
			while (true)
			{

				for (int i = 0; i < NumberOfCharsInShortLink; i++)
				{
					var randomIndex = _random.Next(Alphabet.Length - 1);
					codeChars[i] = Alphabet[randomIndex];
				}

				var code = new string(codeChars);

				if (!await _dbContext.ShortenedUrls.AnyAsync(s => s.Code == code))
				{
					return code;
				}

			}
		}



	}
}
