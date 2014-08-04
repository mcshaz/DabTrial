using System;
using System.Text.RegularExpressions;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Collections.Generic;

namespace DabTrial.Utilities
{
    public static class StringExtensions
    {
        /*
        static Regex _seperateWordsRegex;
        public static string ToSeparateWords(this string value)
        {
            if (value == null)
            {
                return null;
            }

            return (_seperateWordsRegex ?? (_seperateWordsRegex = new Regex("([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))")))
                .Replace(value, "$1 ");
        }
        */
        public static string ToSeparateWords(this string value)
        {
            if (value==null){return null;}
            if(value.Length <=1){return value;}
            char[] inChars = value.ToCharArray();
            List<int> uCWithAnyLC = new List<int>();
            int i = 0;
            while (i < inChars.Length && char.IsUpper(inChars[i])) { ++i; }
            for (; i < inChars.Length; i++)
            {
                if (char.IsUpper(inChars[i]))
                {
                    uCWithAnyLC.Add(i);
                    if (++i < inChars.Length && char.IsUpper(inChars[i]))
                    {
                        while (++i < inChars.Length) 
                        {
                            if (!char.IsUpper(inChars[i]))
                            {
                                uCWithAnyLC.Add(i - 1);
                                break;
                            }
                        }
                    }
                }
            }
            char[] outChars = new char[inChars.Length + uCWithAnyLC.Count];
            int lastIndex = 0;
            for (i=0;i<uCWithAnyLC.Count;i++)
            {
                int currentIndex = uCWithAnyLC[i];
                Array.Copy(inChars, lastIndex, outChars, lastIndex + i, currentIndex - lastIndex);
                outChars[currentIndex + i] = ' ';
                lastIndex = currentIndex;
            }
            int lastPos = lastIndex + uCWithAnyLC.Count;
            Array.Copy(inChars, lastIndex, outChars, lastPos, outChars.Length - lastPos);
            return new string(outChars);
        }
        public static string ToBriefString(this string value) // expression trees cannot handle default values - do it manually
        {
            return ToBriefString(value, 15);
        }
        public static string ToBriefString(this string value, int strLength)
        {
            return ToBriefString(value, strLength, "...");
        }
        public static string ToBriefString(this string value, int strLength, string truncateWith)
        {
            if (value == null) { return null; }
            if (value.Length < strLength) {return value;}
            strLength -= truncateWith.Length;
            return (value.Substring(0, strLength) + truncateWith);
        }
        /// <summary>
        ///convert text to Pascal Case
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ConvertToPascalCase(string str)
        {
            //if nothing is proivided throw a null argument exception
            if (str == null) throw new ArgumentNullException("str", "Null text cannot be converted!");
            str = str.Trim();
            if (str.Length == 0) return str;

            //split the provided string into an array of words
            string[] words = str.Split(' ');

            //loop through each word in the array
            for (int i = 0; i < words.Length; i++)
            {
                //if the current word is greater than 1 character long
                if (words[i].Length > 0)
                {
                    //grab the current word
                    string word = words[i];

                    //convert the first letter in the word to uppercase
                    char firstLetter = char.ToUpper(word[0]);

                    //concantenate the uppercase letter to the rest of the word
                    words[i] = firstLetter + word.Substring(1);
                }
            }

            //return the converted text
            return string.Join(string.Empty, words);
        }
        public static string Pluralise(this string value, int count)
        {
            if (count == 1)
            {
                return value;
            }
            return PluralizationService
                .CreateService(new CultureInfo("en-AU"))
                .Pluralize(value);
        }
        public static string Singularise(this string value)
        {
            return PluralizationService
                .CreateService(new CultureInfo("en-AU"))
                .Singularize(value);
        }
        public static string Replace(this string source, string oldString, string newString, StringComparison comp)
        {
            int index = source.IndexOf(oldString, comp);

            // Determine if we found a match
            bool MatchFound = index >= 0;

            if (MatchFound)
            {
                // Remove the old text
                source = source.Remove(index, oldString.Length);

                // Add the replacemenet text
                source = source.Insert(index, newString);
            }

            return source;
        }

        public static DateTime? ToDateTime(this string val)
        {
            DateTime temp;
            if (DateTime.TryParse(val, out temp)) { return temp; }
            else { return null; }
        }

        public static int? ToInt(this string val)
        {
            int temp;
            if (int.TryParse(val, out temp)) { return temp; }
            else { return null; }
        }
    }
}