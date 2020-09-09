using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Horseshoe.NET.Extensions;

namespace Horseshoe.NET.Dates
{
    public struct Age
    {
        public TimeSpan TimeSpan { get; }
        private TimeSpan PostYearMonthTimeSpan { get; }
        public double TotalYears { get; }
        public double TotalMonths { get; }
        public double TotalDays => Decimals >= 0 ? Math.Round(TimeSpan.TotalDays, Decimals) : TimeSpan.TotalDays;
        public double TotalHours => Decimals >= 0 ? Math.Round(TimeSpan.TotalHours, Decimals) : TimeSpan.TotalHours;
        public double TotalMinutes => Decimals >= 0 ? Math.Round(TimeSpan.TotalMinutes, Decimals) : TimeSpan.TotalMinutes;
        public double TotalSeconds => Decimals >= 0 ? Math.Round(TimeSpan.TotalSeconds, Decimals) : TimeSpan.TotalSeconds;
        public double TotalMilliseconds => Decimals >= 0 ? Math.Round(TimeSpan.TotalMilliseconds, Decimals) : TimeSpan.TotalMilliseconds;
        public int Years { get; }
        public int Months { get; }
        public int Days => PostYearMonthTimeSpan.Days;
        public int Hours => PostYearMonthTimeSpan.Hours;
        public int Minutes => PostYearMonthTimeSpan.Minutes;
        public int Seconds => PostYearMonthTimeSpan.Seconds;
        public int Milliseconds => PostYearMonthTimeSpan.Milliseconds;
        public long Ticks => TimeSpan.Ticks;
        public int Decimals { get; }

        public Age(DateTime from, int decimals = -1) : this(from, DateTime.Now, decimals) { }

        public Age(DateTime from, DateTime asOf, int decimals = -1)
        {
            TimeSpan = asOf - from;
            DateTime temp;
            double totalYears;
            var years = 0;
            double totalMonths;
            var months = 0;
            var neg = false;

            if (from > asOf)   // reverse from and to, if applicable, to simplify algorithm
            {
                temp = from;
                from = asOf;
                asOf = temp;
                neg = true;
            }

            temp = from.AddYears(1);
            while (temp < asOf)
            {
                years++;
                from = temp;
                temp = temp.AddYears(1);
            }
            PostYearMonthTimeSpan = asOf - from;
            totalYears = years + (PostYearMonthTimeSpan.TotalDays / (DateUtil.CountLeapDaysBetween(from, asOf) == 0 ? 365 : 366));

            temp = from.AddMonths(1);
            while (temp < asOf)
            {
                months++;
                from = temp;
                temp = temp.AddMonths(1);
            }
            PostYearMonthTimeSpan = asOf - from;
            totalMonths = years * 12 + months + (PostYearMonthTimeSpan.TotalDays / from.GetNumberOfDaysInMonth());

            if (neg)
            {
                TotalYears = -(decimals >= 0 ? Math.Round(totalYears, decimals) : totalYears);
                Years = -years;
                TotalMonths = -(decimals >= 0 ? Math.Round(totalMonths, decimals) : totalMonths);
                Months = -months;
            }
            else
            {
                TotalYears = decimals >= 0 ? Math.Round(totalYears, decimals) : totalYears;
                Years = years;
                TotalMonths = decimals >= 0 ? Math.Round(totalMonths, decimals) : totalMonths;
                Months = months;
            }
            Decimals = decimals;
        }

        public override string ToString()
        {
            return
                (Years > 0 ? Years + " Years " : "") +
                (Months > 0 ? Months + " Months " : "") +
                PostYearMonthTimeSpan;
        }
    }
}
