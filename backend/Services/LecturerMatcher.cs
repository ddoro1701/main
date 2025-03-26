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
        private const int DistanceThreshold = 6;

        public LecturerMatcher(LecturerService lecturerService)
        {
            _lecturerService = lecturerService;
        }

        public async Task<string?> FindLecturerEmailAsync(string ocrText)
        {
            string candidateName = ExtractNameFromOcrText(ocrText);
            if (string.IsNullOrEmpty(candidateName))
            {
                return null;
            }

            List<Lecturer> lecturers = await _lecturerService.GetAllLecturersAsync();
            Lecturer? bestMatch = null;
            int bestDistance = int.MaxValue;

            bool singleToken = !candidateName.Contains(" ");

            foreach (var lecturer in lecturers)
            {
                if (string.IsNullOrEmpty(lecturer.Name))
                    continue;

                int dist;
                if (singleToken)
                {
                    var nameParts = lecturer.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string lecturerSurname = nameParts.Length > 1 ? nameParts[^1] : lecturer.Name;
                    dist = EnhancedLevenshteinDistance(candidateName.ToLower(), lecturerSurname.ToLower());
                }
                else
                {
                    dist = EnhancedLevenshteinDistance(candidateName.ToLower(), lecturer.Name.ToLower());
                }

                if (dist < bestDistance)
                {
                    bestDistance = dist;
                    bestMatch = lecturer;
                }
            }

            return bestDistance <= DistanceThreshold ? bestMatch?.Email : null;
        }

        private string ExtractNameFromOcrText(string text)
        {
            string pattern = @"\b(Mr\.?|Mrs\.?|Ms\.?|Dr\.?|Prof\.?|Miss|Phd)\b";
            string cleaned = Regex.Replace(text, pattern, "", RegexOptions.IgnoreCase).Trim();

            // Removing common UK street indicators from extracted text
            string streetPattern = @"\b(Road|Street|Avenue|Lane|Drive|Close|Court|Place|Terrace|Way|Rd|)\b";
            cleaned = Regex.Replace(cleaned, streetPattern, "", RegexOptions.IgnoreCase).Trim();

            var tokens = cleaned.Split(new char[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                                .Where(token => !token.Any(char.IsDigit))
                                .ToArray();

            if (tokens.Length == 0)
                return string.Empty;

            if (tokens.Length == 1)
                return tokens[0];

            return string.Join(" ", tokens.Take(3));
        }

        private int EnhancedLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int i = 0; i <= n; i++) d[i, 0] = i;
            for (int j = 0; j <= m; j++) d[0, j] = j;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;

                    if (OCRCommonErrors(s[i - 1], t[j - 1]))
                    {
                        cost = 1;
                    }

                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

        private bool OCRCommonErrors(char a, char b)
        {
            var commonErrors = new Dictionary<char, char[]>
            {
                { '0', new[] { 'O', 'o' } },
                { '1', new[] { 'I', 'l' } },
                { '5', new[] { 'S', 's' } },
                { '8', new[] { 'B' } },
                { 'M', new[] { 'N' } },
                { 'i', new[] { 'l', '1' } }
            };

            return commonErrors.ContainsKey(a) && commonErrors[a].Contains(b);
        }
    }
}
