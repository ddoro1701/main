using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebApplication1.Models;

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
            // Step 1: Extract candidate name from OCR text.
            string candidateName = ExtractNameFromOcrText(ocrText);
            if (string.IsNullOrEmpty(candidateName))
            {
                return null;
            }

            // Get all lecturers from the database.
            List<Lecturer> lecturers = await _lecturerService.GetAllLecturersAsync();
            Lecturer? bestMatch = null;
            int bestDistance = int.MaxValue;

            // If candidateName is a single token, compare to lecturer surname; otherwise use full name.
            bool singleToken = !candidateName.Contains(" ");

            foreach (var lecturer in lecturers)
            {
                if (string.IsNullOrEmpty(lecturer.Name))
                    continue;

                int dist;
                if (singleToken)
                {
                    // Use last token (surname) if available, otherwise use full name.
                    var nameParts = lecturer.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string lecturerSurname = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : lecturer.Name;
                    dist = LevenshteinDistance(candidateName.ToLower(), lecturerSurname.ToLower());
                }
                else
                {
                    // Compare full candidate with full lecturer name.
                    dist = LevenshteinDistance(candidateName.ToLower(), lecturer.Name.ToLower());
                }

                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestMatch = lecturer;
                }
            }

            // Only return the best match if its distance is within threshold.
            return bestDistance <= DistanceThreshold ? bestMatch?.Email : null;
        }

        // Extraction method: removes common title prefixes, filters out tokens containing digits,
        // and if at least two valid tokens exist, returns "first second"; otherwise returns the single token.
        private string ExtractNameFromOcrText(string text)
        {
            // Remove title prefixes (case-insensitive)
            string pattern = @"\b(Mr\.|Mrs\.|Ms\.|Dr\.)\b";
            string cleaned = Regex.Replace(text, pattern, "", RegexOptions.IgnoreCase);

            // Split cleaned text by whitespace and filter out tokens containing any digit.
            var tokens = cleaned.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                .Where(token => !token.Any(char.IsDigit))
                                .ToArray();

            // If no tokens found, return empty string.
            if (tokens.Length == 0)
                return string.Empty;

            // If only one token is available, return it.
            if (tokens.Length == 1)
                return tokens[0];

            // Otherwise, return the first two tokens.
            return $"{tokens[0]} {tokens[1]}";
        }

        // Basic Levenshtein Distance algorithm.
        private int LevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // If one string is empty, return the length of the other.
            if (n == 0)
                return m;
            if (m == 0)
                return n;

            // Initialize matrix.
            for (int i = 0; i <= n; i++)
                d[i, 0] = i;
            for (int j = 0; j <= m; j++)
                d[0, j] = j;

            // Compute the matrix.
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