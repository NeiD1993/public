using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TypingTest.Service
{
    static class RegularExpressionsService
    {
        private static readonly string exceptionMessage = "Exception in ";

        public static void InsertInInputStringBeforePatternStringFirstCharacterReplacementString(string patternString, string oppositePatternString, string replacementString, ref string inputString)
        {
            try
            {
                int replacementStringLength = replacementString.Length;
                MatchCollection matchCollection = GetInputStringPatternStringMathes(patternString, inputString);
                int incrementIndex = 0;
                foreach (Match match in matchCollection)
                {
                    if (!HasInputStringPatternStringFormat(oppositePatternString, match.Value))
                        inputString = inputString.Insert(match.Index + incrementIndex++ * replacementStringLength, replacementString);
                }
            }
            catch { throw new Exception(exceptionMessage + "ReplaceInputStringPatternStringFirstCharacterOnReplacementString"); }
        }

        public static bool HasInputStringPatternStringFormat(string patternString, string inputString)
        {
            try { return (new Regex(patternString).Match(inputString).Length == inputString.Length); }
            catch { return false; }
        }

        public static int FindInputStringFirstPatternStringStartIndex(string patternString, string inputString, int startIndex, bool isRightToLeftDirection = false)
        {
            int matchIndex = -1;
            try
            {
                Regex regex = (!isRightToLeftDirection) ? new Regex(patternString) : new Regex(patternString, RegexOptions.RightToLeft);
                Match match = regex.Match(inputString, startIndex);
                if (match.Success)
                    matchIndex = match.Index;
            }
            catch { return -1; }
            return matchIndex;
        }

        public static MatchCollection GetInputStringPatternStringMathes(string patternString, string inputString)
        {
            return new Regex(patternString).Matches(inputString);
        }
    }
}
