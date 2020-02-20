using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Horseshoe.NET
{
    public static class DateUtil
    {
        public static bool IsLeapYear(DateTime date)
        {
            return IsLeapYear(date.Year);
        }

        public static bool IsLeapYear(int year)
        {
            if (year % 400 == 0) return true;
            if (year % 100 == 0) return false;
            if (year % 4 == 0) return true;
            return false;
        }

        public static int CountLeapDaysBetween(DateTime from, DateTime to)
        {
            if (from > to)
            {
                var temp = from;
                from = to;
                to = temp;
            }
            to = new DateTime(to.Year, to.Month, 1);
            int leapDayCounter = 0;
            while (from < to)
            {
                if (IsLeapYear(from.Year))
                {
                    if (from.Month < 2)
                    {
                        from = from.AddMonths(1);
                        continue;
                    }
                    else if (from.Month == 2)
                    {
                        leapDayCounter++;
                    }
                }
                from = new DateTime(from.Year + 1, 1, 1);
            }
            return leapDayCounter;
        }

        public static bool SameDay(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month && date1.Day == date2.Day;
        }

        public static bool SameMonth(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month;
        }
    }
}
