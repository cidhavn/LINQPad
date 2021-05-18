<Query Kind="Program" />

void Main()
{
	DiffDays(DateTime.Parse("2020-06-01"), DateTime.Parse("2020-07-01")).Dump();
	//DiffMonths(DateTime.Parse("2020-05-11"), DateTime.Parse("2020-06-01")).Dump();
}

public static int DiffDays(DateTime startDateTime, DateTime endDateTime)
{
    // Reference: https://www.died.tw/2011/10/c-daydiff.html

    int day = DateTime.IsLeapYear(startDateTime.Year) ? 366 : 365;

    if (endDateTime.Year < startDateTime.Year)
    {
        return endDateTime.DayOfYear - startDateTime.DayOfYear - day;
    }
    else if (endDateTime.Year > startDateTime.Year)
    {
        return endDateTime.DayOfYear - startDateTime.DayOfYear + day;
    }

    return endDateTime.DayOfYear - startDateTime.DayOfYear;
}

public static int DiffMonths(DateTime from, DateTime to)
{
    // 處理跨年度問題
    int maxMonth = ((to.Year - from.Year) * 12) + to.Month;

    return maxMonth - from.Month;
}