using ScintillaNET;
using ScintillaNET.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoStudio
{
    internal static class ScintillaBreakPointValidator
    {
        internal static bool IsBreakPointAllowed(ScintillaWPF scintilla, Line line, out int breakPointStartPosition, out int breakPointHighlightLength)
        {
            string lineString = line.Text.Trim();
            int lineStringLength = lineString.Length;
            breakPointStartPosition = 0;
            breakPointHighlightLength = 0;
            string firstWord = string.Empty;
            int firstWhiteSpaceIndex = lineString.IndexOf(" ");

            if (lineStringLength > 1 && firstWhiteSpaceIndex >= 0)
                firstWord = lineString.Substring(0, firstWhiteSpaceIndex);
            else
                firstWord = lineString;

            string[] disAllowedString = new string[] { "NAMESPACE" };
            if (!disAllowedString.Contains(firstWord.ToUpper()) && !string.IsNullOrEmpty(firstWord))
            {
                breakPointStartPosition = line.Position + (line.Length - lineStringLength - 2);
                breakPointHighlightLength = lineStringLength;
                return true;
            }

            return false;
        }
    }
}
