using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether.src.Utils
{
    public static class StringExtensionMethods
    {
        private static readonly Random randomizer = new Random();
        private static readonly string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Generates a random username based on the given length
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string generateUsername(int length)
        {
            if (length < 0) { return String.Empty; }
            string letters = characters.Substring(10);
            string vowels = "aeiou";
            StringBuilder sb = new StringBuilder();
            sb.Append(letters[randomizer.Next(0, 26)]);
            for (int i = 1; i < length; i++)
            {
                bool val = (randomizer.Next(2) == 1 ? true : false);
                if (val)
                {
                    sb.Append(letters[randomizer.Next(26, letters.Length)]);
                }
                else
                {
                    sb.Append(vowels[randomizer.Next(vowels.Length)]);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Determines if the given string has a sequance of same characters
        /// returns true if the limit has been exceeded
        /// </summary>
        /// <param name="str"></param>
        /// <param name="redundancyLimit"></param>
        /// <returns></returns>
        public static bool HasMoreRedundantCharactersThan(this string str, int redundancyLimit)
        {
            redundancyLimit--;
            for (int i = 0; i < str.Length - 1; i++)
            {
                if (str[i] != str[i + 1]) { continue; }
                int count = 1;
                // current char and next char are the same
                for (int j = i + 1; j < i + redundancyLimit + 1; j++)
                {
                    // check if the limit has already been exceeded
                    if (count > redundancyLimit) { return true; }
                    // out of bounds check
                    if (j >= str.Length - 1) { return false; }
                    // the sequance of same characters has been broken
                    if (str[i] != str[j]) { break; }
                    count++;
                }
                if (count > redundancyLimit) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Converts the given string to an integer
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInteger(this string str)
        {
            if (str.IsNumeric() == false) { return -1; }
            if (str.Length > 6) { return -1; }
            return Int32.Parse(str);
            
        }
        /// <summary>
        /// converts the given string into a bool value
        /// if the given string isnt an equivalent boolean value, returns false.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ToBoolean(this string str)
        {
            str = str.ToLower();
            if (str.Equals("true") || str.Equals("yes") || str.Equals("yea") || str.Equals("sure") ||
                str.Equals("roger") || str.Equals("indeed") || str.Equals("absolutely") || str.Equals("y"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Returns true if the string does not begin with
        /// or end with a symbol or space
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool StartsOrEndsSpecially(this string str)
        {
            return isSpecialCharacter(str[0]) || isSpecialCharacter(str[str.Length - 1]);
        }

        /// <summary>
        /// Checks if a characters is special
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool isSpecialCharacter(char c)
        {
            return c == '`' || c == '~' || c == '!' || c == '@' || c == '#' || c == '$' || c == '%' ||
                c == '^' || c == '&' || c == '*' || c == '(' || c == ')' || c == '-' || c == '_' ||
                c == '+' || c == '=' || c == '{' || c == '}' || c == '[' || c == ']' || c == '|' || 
                c == ':' || c == ';' || c == '"' || c == '<' || c == '>' || c == ',' || c == '\'' ||
                c == '.' || c == '?' || c == '\'' || c == '\\';
        }

        /// <summary>
        /// Determines if the given string can represented numerically
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!Char.IsDigit(str[i])) { return false; }
            }
            return true;
        }

        /// <summary>
        /// changes the space occurence to a plus occurence
        /// </summary>
        /// <param name="entryPoint"></param>
        /// <param name="searchTerm"></param>
        /// <returns></returns>
        public static string URLifyParseAddition(string entryPoint, string searchTerm)
        {
            return entryPoint + searchTerm.Replace(' ', '+');
        }
        /// <summary>
        /// Truncates the given string, based on the bool value, truncates from
        /// the beginning or the end where the returned string is less than or equal to the alloted 
        /// amount 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="onBeginning"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string Truncate(this string str, bool onBeginning, int amount)
        {
            if (onBeginning)
            {
                // truncate from beginning
                if (str.Length > amount) { return str.Substring(0, amount); }
            }
            else
            {
                // truncate from end
                if (str.Length > amount) { return str.Substring(str.Length - amount); }
            }
            // didnt need to truncate
            return str;
        }
        /// <summary>
        /// Counts the amount of times the first string contains the last given string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="toFind"></param>
        /// <returns></returns>
        public static int Occurrences(this string str, string toFind)
        {
            int lastIndex = 0;
            int count = 0;

            while (lastIndex != -1)
            {
                lastIndex = str.IndexOf(toFind, lastIndex);

                if (lastIndex != -1)
                {
                    count++;
                    lastIndex += toFind.Length;
                }
            }
            return count;
        }
        /// <summary>
        /// Given a string, adds all the characters from left to right
        /// to an integer, returning the integer
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetSpecialHashCode(this string str)
        {
            if (str == null) { return -1; }
            int hash = 0;
            for (int i = 0; i < str.Length; i++)
            {
                hash += (str[i] / 2) * (i + 1);
            }
            return hash;
        }
        /// <summary>
        /// Counts the amount of chunks delimited by white space within the string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int getChunks(this string str)
        {
            int count = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (i < 1) { continue; }
                if (str[i] != ' ') { continue; }
                if (str[i - 1] == ' ') { continue; }
                if (i == str.Length - 1) { continue; }
                count++;
            }
            if (str[str.Length - 1] == ' ') { count--; }
            return count + 1;
        }
    }
}
