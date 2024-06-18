using System;

namespace XGolf.Game.Component
{
    public interface ITimeProvider
    {
        bool TryParse(string time, out long result);
        long CurrentUTCTimeInSeconds();
        bool IsTimeInThePast(long time);
        long GetTimeRemaining(long time);
        long GetTimeElapsed(long time);
        bool IsExpired(long endsAt);
        bool IsInProgress(long startsAt, long endsAt);
        long GetTimeFromNow(long interval);
    }

    public class TimeProvider : ITimeProvider
    {
        public long CurrentUTCTimeInSeconds()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        public bool TryParse(string time, out long result)
        {
            var parsed = DateTime.TryParse(time, out DateTime dateTime);
            result = GetUnixTime(dateTime);
            return parsed;
        }

        public static long GetUnixTime(DateTime dateTime)
        {
            DateTimeOffset unixEpoch = new DateTimeOffset(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, TimeSpan.Zero); // Unix epoch time
            return unixEpoch.ToUnixTimeSeconds(); // Convert to seconds and return
        }

        public bool IsTimeInThePast(long time)
        {
            return time <= CurrentUTCTimeInSeconds();
        }

        public long GetTimeRemaining(long time)
        {
            return time - CurrentUTCTimeInSeconds();
        }

        public long GetTimeElapsed(long time)
        {
            return CurrentUTCTimeInSeconds() - time;
        }

        public bool IsExpired(long endsAt)
        {
            return endsAt - CurrentUTCTimeInSeconds() <= 0;
        }

        public bool IsInProgress(long startsAt, long endsAt)
        {
            long currentTime = CurrentUTCTimeInSeconds();
            return startsAt <= currentTime && (currentTime < endsAt || endsAt <= 0);
        }

        public long GetTimeFromNow(long interval)
        {
            return CurrentUTCTimeInSeconds() + interval;
        }

    }
}