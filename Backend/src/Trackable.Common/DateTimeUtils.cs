// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Trackable.Common
{
    public static class DateTimeUtils
    {
        /// <summary>
        /// Get the equivalent datetime from the provided timestamp
        /// </summary>
        /// <param name="unixTime">Number of milliseconds since the epoch</param>
        /// <returns></returns>
        public static DateTime FromUnixTime(long unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

        /// <summary>
        /// Gets the equivalent timestamp from the provided time
        /// </summary>
        /// <param name="time">the time</param>
        /// <returns></returns>
        public static long ToUnixTime(DateTime time)
        {
            return (time.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks) / TimeSpan.TicksPerMillisecond;
        }

        /// <summary>
        /// Convert the timestamp from milliseconds to seconds
        /// </summary>
        /// <param name="timestamp">Number of milliseconds since the epoch</param>
        /// <returns>Corresponding number of seconds</returns>
        public static long TimestampToSeconds(long timestamp)
        {
            return timestamp / 1000;
        }

        /// <summary>
        /// Convert the timestamp from seconds to milliseconds
        /// </summary>
        /// <param name="timestamp">Number of seconds since the epoch</param>
        /// <returns>Corresponding number of milliseconds</returns>
        public static long SecondsToTimestamp(long timestamp)
        {
            return timestamp * 1000;
        }

        /// <summary>
        /// Increment the timestamp
        /// </summary>
        /// <param name="timestamp">Number of milliseconds since the epoch</param>
        /// <param name="seconds">Number of seconds to increment the timestamp by</param>
        /// <returns></returns>
        public static long IncrementTimestamp(long timestamp, long seconds)
        {
            return timestamp + SecondsToTimestamp(seconds);
        }

        /// <summary>
        /// Difference between two timestamps in seconds
        /// </summary>
        /// <param name="timestamp1">Number of milliseconds since the epoch</param>
        /// <param name="timestamp2">Number of milliseconds since the epoch</param>
        /// <returns></returns>
        public static long DifferenceInMilliseconds(long timestamp1, long timestamp2)
        {
            return Math.Abs(timestamp1 - timestamp2);
        }

        /// <summary>
        /// Get Current time as seconds from epoch
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimeInSeconds()
        {
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            return ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerSecond);
        }

        /// <summary>
        /// Get Current time as milliseconds from epoch
        /// </summary>
        /// <returns></returns>
        public static long CurrentTimeInMillseconds()
        {
            long epochTicks = new DateTime(1970, 1, 1).Ticks;
            return ((DateTime.UtcNow.Ticks - epochTicks) / TimeSpan.TicksPerMillisecond);
        }
    }
}
