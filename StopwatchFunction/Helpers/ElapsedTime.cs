using System;
using System.Globalization;

namespace StopwatchFunction.Helpers
{
    public class ElapsedTime
    {
        public string CalculateElapsedTime(string startTime, string endTime)
        {
            var startDateTime = DateTime.ParseExact(startTime, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            var endDateTime = string.IsNullOrEmpty(endTime)
                ? DateTime.UtcNow
                : DateTime.ParseExact(endTime, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
            ;
            var elapsedTime = endDateTime - startDateTime;

            return elapsedTime.ToString();
        }
    }
}
