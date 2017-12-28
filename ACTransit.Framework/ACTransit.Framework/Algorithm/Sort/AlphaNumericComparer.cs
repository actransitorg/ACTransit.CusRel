﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace ACTransit.Framework.Algorithm.Sort
{    
    /// <summary>
    /// String casted FastAlphaNumericComparer.
    /// Usage: x is string: .OrderBy(x => x, new AlphaNumericComparer())
    /// </summary>
    public class AlphaNumericComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return FastAlphaNumericComparer.CompareFast(x, y);
        }
    }

    /// <summary>
    /// Fast alphanumeric comparison 
    /// </summary>
    public class FastAlphaNumericComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return CompareFast(x, y);
        }

        public static int CompareFast(object x, object y)
        {
            var s1 = x as string;
            if (s1 == null)
                return 0;
            var s2 = y as string;
            if (s2 == null)
                return 0;

            var len1 = s1.Length;
            var len2 = s2.Length;
            var marker1 = 0;
            var marker2 = 0;

            // Walk through two the strings with two markers.
            while (marker1 < len1 && marker2 < len2)
            {
                var ch1 = s1[marker1];
                var ch2 = s2[marker2];

                // Some buffers we can build up characters in for each chunk.
                var space1 = new char[len1];
                var loc1 = 0;
                var space2 = new char[len2];
                var loc2 = 0;

                // Walk through all following characters that are digits or
                // characters in BOTH strings starting at the appropriate marker.
                // Collect char arrays.
                do
                {
                    space1[loc1++] = ch1;
                    marker1++;

                    if (marker1 < len1)
                        ch1 = s1[marker1];
                    else
                        break;

                } while (char.IsDigit(ch1) == char.IsDigit(space1[0]));

                do
                {
                    space2[loc2++] = ch2;
                    marker2++;

                    if (marker2 < len2)
                        ch2 = s2[marker2];
                    else
                        break;

                } while (char.IsDigit(ch2) == char.IsDigit(space2[0]));

                // If we have collected numbers, compare them numerically.
                // Otherwise, if we have strings, compare them alphabetically.
                var str1 = new string(space1);
                var str2 = new string(space2);

                int result;

                if (char.IsDigit(space1[0]) && char.IsDigit(space2[0]))
                {
                    var thisNumericChunk = int.Parse(str1);
                    var thatNumericChunk = int.Parse(str2);
                    result = thisNumericChunk.CompareTo(thatNumericChunk);
                }
                else
                    result = String.Compare(str1, str2, StringComparison.Ordinal);

                if (result != 0)
                    return result;
            }
            return len1 - len2;
        }
    }
}
