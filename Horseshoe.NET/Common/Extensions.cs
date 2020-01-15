using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Common
{
    public static class Extensions
    {
        public static int AgeInYearsFrom(this DateTime from)
        {
            return DateUtil.AgeInYears(from);
        }

        public static double TotalAgeInYearsFrom(this DateTime from, int decimals = -1)
        {
            return DateUtil.TotalAgeInYears(from, decimals: decimals);
        }

        public static int AgeInMonthsFrom(this DateTime from)
        {
            return DateUtil.AgeInMonths(from);
        }

        public static double TotalAgeInMonthsFrom(this DateTime from, int decimals = -1)
        {
            return DateUtil.TotalAgeInMonths(from, decimals: decimals);
        }

        public static int AgeInDaysFrom(this DateTime from)
        {
            return DateUtil.AgeInDays(from);
        }

        public static double TotalAgeInDaysFrom(this DateTime from, int decimals = -1)
        {
            return DateUtil.TotalAgeInDays(from, decimals: decimals);
        }

        public static bool IsLeapYear(this DateTime date)
        {
            return DateUtil.IsLeapYear(date);
        }
    }
}
