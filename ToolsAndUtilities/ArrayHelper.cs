using System;
using System.Collections.Generic;
using System.Text;

namespace ToolsAndUtilities
{
    public static class ArrayHelper
    {
        /// <summary>
        ///  Search if a specific value is present in a SORTED array of integers
        /// </summary>
        public static bool BinarySearch(this int[] numbers, int value)
        {
            if (numbers != null)
            {
                int mid;
                int min = 0;
                int max = numbers.Length - 1;

                while (min <= max)
                {
                    mid = (max + min) / 2;
                    if (numbers[mid] == value)
                    {
                        return true;
                    }
                    else if (numbers[mid] < value)
                    {
                        min = mid + 1;
                    }
                    else // if (numbers[mid] > value)
                    {
                        max = mid - 1;
                    }
                }
            }

            return false;
        }
    }

}
