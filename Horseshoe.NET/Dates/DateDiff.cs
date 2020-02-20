using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET.Dates
{
    public static class DateDiffx
    {
        public static int Years(DateTime from, DateTime to)
        {
            var neg = false;
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
                neg = true;
            }
            int years = to.Year - from.Year;
            if (to.Month < from.Month)
            {
                years--;
            }
            else if (to.Month == from.Month)
            {
                if (to.Day < from.Day)
                {
                    years--;
                }
            }
            if (neg) years *= -1;
            return years;
        }

        public static double PreciseYears(DateTime from, DateTime to, int decimals = -1)
        {
            var neg = false;
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
                neg = true;
            }
            double years = to.Year - from.Year;
            if (to.DayOfYear < from.DayOfYear)
            {
                int gap = to.DayOfYear + (365 - from.DayOfYear);
                years -= gap / 365D;
            }
            else
            {
                int gap = to.DayOfYear - from.DayOfYear;
                years += gap / 365D;
            }
            if (neg) years *= -1D;
            return decimals < 0
                ? years
                : Math.Round(years, decimals);
        }

        public static int Months(DateTime from, DateTime to)
        {
            var neg = false;
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
                neg = true;
            }
            var years = Years(from, to);
            var months = years * 12;
            from = from.AddYears(years);
            while (!DateUtil.SameMonth(to, from))
            {
                from = from.AddMonths(1);
                months++;
            }
            if (months > 0 && to.Day < from.Day)
            {
                months--;
            }
            if (neg) months *= -1;
            return months;
        }

        public static double PreciseMonths(DateTime from, DateTime to, int decimals = -1)
        {
            var neg = false;
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
                neg = true;
            }
            var months = Months(from, to);
            from = from.AddMonths(months);
            double daysInMonth = DateTime.DaysInMonth(from.Year, from.Month);
            var gap = to.Day - from.Day;
            var doubleMonths = (months + (gap / daysInMonth));
            if (neg) doubleMonths *= -1D;
            return decimals < 0
                ? doubleMonths
                : Math.Round(doubleMonths, decimals);
        }

        public static int Days(DateTime from, DateTime to)
        {
            var span = to - from;
            return span.Days;
        }

        public static double PreciseDays(DateTime from, DateTime to, int decimals = -1)
        {
            var span = to - from;
            return decimals < 0
                ? span.TotalDays
                : Math.Round(span.TotalDays, decimals);
        }
    }
}
