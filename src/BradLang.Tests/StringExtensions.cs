using System;
using System.Collections.Generic;
using System.IO;

namespace BradLang.Tests;

static class StringExtensions
{
    public static string Unindent(this string text)
    {
        return string.Join(Environment.NewLine, text.UnindentLines());
    }

    public static string[] UnindentLines(this string text)
    {
        var minimumIndent = int.MaxValue;

        var lines = new List<string>();

        var reader = new StringReader(text);
            
        while (reader.ReadLine() is { } line)
        {
            var trimmedLineLength = line.TrimStart().Length;

            if (trimmedLineLength == 0)
            {
                lines.Add(string.Empty);
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

        return lines.ToArray();
    }
}
