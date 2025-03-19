using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Services
{
    public class LecturerMatcher
    {
        private readonly LecturerService _lecturerService; 
        private const int DistanceThreshold = 5; // adjust threshold as needed
        public LecturerMatcher(LecturerService lecturerService)
        {
            _lecturerService = lecturerService;
        }

        // Main method: given OCR text, extract candidate name and return the lecturer email if found.
        public async Task<string?> FindLecturerEmailAsync(string ocrText)
        {
            // Step 1: Extract candidate full name from OCR text.
            string candidateName = ExtractNameFromOcrText(ocrText);
            if (string.IsNullOrEmpty(candidateName))
            {
                return null;
            }

            // Step 2: Get all lecturers from the database.
            List<Lecturer> lecturers = await _lecturerService.GetAllLecturersAsync();
            Lecturer? bestMatch = null;
            int bestDistance = int.MaxValue;

            // Step 3: Compute fuzzy match distance (Levenshtein) for each lecturer.
            foreach (var lecturer in lecturers)
            {
                if (string.IsNullOrEmpty(lecturer.Name))
                    continue;

                int dist = LevenshteinDistance(candidateName.ToLower(), lecturer.Name.ToLower());
                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestMatch = lecturer;
                }
            }

            // Only return if our best match is close enough
            return bestDistance <= DistanceThreshold ? bestMatch?.Email : null;
        }

        // A simple extraction method. This removes common title prefixes and takes the next two words.
        private string ExtractNameFromOcrText(string text)
        {
            // Remove potential titles (case-insensitive)
            string pattern = @"\b(Mr\.|Mrs\.|Ms\.|Dr\.)\b";
            string cleaned = Regex.Replace(text, pattern, "", RegexOptions.IgnoreCase);
            // Split remaining text by whitespace
            var words = cleaned.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

            // Assuming name is in the first two words after title removal
            if (words.Length < 2)
                return string.Empty;
            return $"{words[0]} {words[1]}";
        }

        // Basic Levenshtein distance algorithm
        private int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 2
            if (n == 0)
                return m;
            if (m == 0)
                return n;

            // Step 3
            for (int i = 0; i <= n; i++)
                d[i, 0] = i;
            for (int j = 0; j <= m; j++)
                d[0, j] = j;

            // Step 4
            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = s[i - 1] == t[j - 1] ? 0 : 1;
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }
        

    }
}