using System;
using System.Collections.Generic;
using System.IO;

namespace BradLang.Tests
{
    public static class StringExtensions
    {
        public static string Unindent(this string text)
        {
            var minimumIndent = Int32.MaxValue;

            var lines = new List<string>();
            string line;

            var reader = new StringReader(text);
            
            while ((line = reader.ReadLine()) != null)
            {
                var trimmedLineLength = line.TrimStart().Length;

                if (trimmedLineLength == 0)
                {
                    lines.Add(String.Empty);
                    continue;
                }

                var indent = line.Length - trimmedLineLength;

                minimumIndent = Math.Min(minimumIndent, indent);

                lines.Add(line);
            }

            for (var i = 0; i < lines.Count; i++)
            {
                if (lines[i].Length > 0)
                {
                    lines[i] = lines[i].Substring(minimumIndent);
                }
            }

            // Remove any leading blank lines
            while (lines.Count > 0 && lines[0].Length == 0)
            {
                lines.RemoveAt(0);
            }

            // Remove any trailing blank lines
            while (lines.Count > 0 && lines[lines.Count - 1].Length == 0)
            {
                lines.RemoveAt(lines.Count - 1);
            }

            return String.Join(Environment.NewLine, lines);
        }
    }
}
