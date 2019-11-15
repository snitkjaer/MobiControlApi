using System;
using System.Collections.Generic;
using System.Text;

namespace MobiControlApi
{
    public static class Helpers
    {

        // 2015-12-19T16:39:57-02:00

        public static string GetUrlDate(DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        public static string GetUrlStartStop(DateTimeOffset startDate, DateTimeOffset stopDate)
        {
            // startDate=2019-11-01T00%3A00%3A00-02%3A00&endDate=2019-12-01T00%3A00%3A00-02%3A00

            return "startDate=" + GetUrlDate(startDate) + "&endDate=" + GetUrlDate(stopDate);
        }

    }
}
