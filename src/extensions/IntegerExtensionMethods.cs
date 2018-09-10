using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAether.src.Utils
{
    public static class IntegerExtensionMethods
    {
        /// <summary>
        /// Returns the precentags of the number based on the given float
        /// </summary>
        /// <param name="number"></param>
        /// <param name="percent"></param>
        /// <returns></returns>
        public static int getNthPercentageOf(this int number, float percent)
        {
            if (percent < 0 || percent > 100)
            {
                return -1;
            }
            else
            {
                return (int)(number * (percent / 100.0f));
            }
        }
        /// <summary>
        /// Returns the amount of characters within the integer
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public static int charactersWithin(this int num)
        {
            int count = 0;
            if (num < 0) { count++; }
            while (num != 0)
            {
                num = num / 10;
                count++;
            }
            return count;
        }

    }
}
