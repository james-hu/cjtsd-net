using System;
using System.Collections.Generic;

namespace CJTSD.Net
{
    /// <summary>
    /// This is the class for handling (primarily generating) <see href="https://github.com/james-hu/cjtsd-js/wiki/Compact-JSON-Time-Series-Data">Compact JSON Time Series Data (CJTSD)</see> data.
    /// To generate a CJTSD object:
    /// <code>
    ///     CJTSD.builder().add(...).add(...).add(...).build()
    /// </code>
    /// To consume a CJTSD object:
    /// <code>
    ///     JsonConvert.DeserializeObject<CJTSD>(jsonString).ToList()
    /// </code>
    /// </summary>
    public class CJTSD : PlainCJTSD
    {
        static private IList<Entry> emptyEntries = new List<Entry>().AsReadOnly();

        public CJTSD() : base()
        {

        }

        /// <summary>
        /// Shallow copy constructor
        /// </summary>
        /// <param name="other">another instance from which the properties will be copied</param>
        public CJTSD(PlainCJTSD other) : base(other)
        {

        }

        /// <summary>
        /// Convert into list form
        /// </summary>
        /// <returns>the list containing entries of data points</returns>
        public IList<Entry> ToList()
        {
            if (t == null || t.Count == 0)
            {
                return emptyEntries;
            }

            List<Entry> result = new List<Entry>(t.Count);
            int lastDuration = 0;
            for (int i = 0; i < t.Count; i++)
            {
                long timestamp = t[i];
                int duration = -1;
                if (i < d.Count)
                {
                    duration = d[i];
                }
                if (duration == -1)
                {
                    duration = lastDuration;
                }
                lastDuration = duration;

                DateTime timestampObj;
                TimeSpan durationObj;
                if (u == null || u.Equals("m"))
                {
                    timestampObj = new DateTime(timestamp * TimeSpan.TicksPerMinute, DateTimeKind.Local);
                    durationObj = new TimeSpan(duration * TimeSpan.TicksPerMinute);
                }
                else if (u.Equals("s"))
                {
                    timestampObj = new DateTime(timestamp * TimeSpan.TicksPerSecond, DateTimeKind.Local);
                    durationObj = new TimeSpan(duration * TimeSpan.TicksPerSecond);
                }
                else if (u.Equals("S"))
                {
                    timestampObj = new DateTime(timestamp * TimeSpan.TicksPerMillisecond, DateTimeKind.Local);
                    durationObj = new TimeSpan(duration * TimeSpan.TicksPerMillisecond);
                }
                else
                {
                    throw new ArgumentException("Unit not supported: " + u);
                }

                result.Add(new Entry(timestampObj, durationObj,
                        c == null || i >= c.Count ? (long?) null : c[i],
                        s == null || i >= s.Count ? (decimal?) null : s[i],
                        a == null || i >= a.Count ? (decimal?) null : a[i],
                        m == null || i >= m.Count ? (decimal?) null : m[i],
                        x == null || i >= x.Count ? (decimal?) null : x[i],
                        n == null || i >= n.Count ? (decimal?) null : n[i],
                        o == null || i >= o.Count ? null : o[i]
                        ));

            }
            return result;
        }

        /// <summary>
        /// Create a builder for generating CJTSD object.
        /// The expected number of data points is 50.
        /// If the actual number of data points exceeds the expected, the builder will just grow.
        /// </summary>
        /// <returns>the builder</returns>
        static public Builder Create()
        {
            return new Builder();
        }

        /// <summary>
        /// Create a builder for generating CJTSD object.
        /// The expected number of data points is specified as argument to this method.
        /// If the actual number of data points exceeds the expected, the builder will just grow.
        /// </summary>
        /// <param name="expectedSize">the expected number of data points</param>
        /// <returns>the builder</returns>
        static public Builder Create(int expectedSize)
        {
            return new Builder(expectedSize);
        }

        /// <summary>
        /// Builder that keeps intermediate data structure for creating CJTSD object.
        /// </summary>
        public class Builder
        {
            private const byte MINUTES = 1;
            private const byte SECONDS = 2;
            private const byte MILLIS = 3;

            private int expectedSize;
            private byte unit = MINUTES;
            private IList<long> timestamps;
            private IList<int> durations;
            private IList<long> counts;
            private IList<decimal> sums;
            private IList<decimal> avgs;
            private IList<decimal> mins;
            private IList<decimal> maxs;
            private IList<decimal> numbers;
            private IList<Object> objs;

            internal Builder() : this(50)
            {
            }

            internal Builder(int expectedSize)
            {
                this.expectedSize = expectedSize;
                timestamps = new List<long>(expectedSize);
                durations = new List<int>(expectedSize);
            }

            /// <summary>
            /// Set the unit to minute.
            /// If no SetUnitToXxx method has been called, the default unit (MINUTES) will be used.
            /// SetUnitToXxx methods should be called no more than once, and only before calling other methods.
            /// </summary>
            /// <returns>the builder itself</returns>
            public Builder SetUnitToMinutes()
            {
                this.unit = MINUTES;
                return this;
            }

            /// <summary>
            /// Set the unit to second.
            /// If no SetUnitToXxx method has been called, the default unit (MINUTES) will be used.
            /// SetUnitToXxx methods should be called no more than once, and only before calling other methods.
            /// </summary>
            /// <returns>the builder itself</returns>
            public Builder SetUnitToSeconds()
            {
                this.unit = SECONDS;
                return this;
            }

            /// <summary>
            /// Set the unit to millisecond.
            /// If no SetUnitToXxx method has been called, the default unit (MINUTES) will be used.
            /// SetUnitToXxx methods should be called no more than once, and only before calling other methods.
            /// </summary>
            /// <returns>the builder itself</returns>
            public Builder SetUnitToMillis()
            {
                this.unit = MILLIS;
                return this;
            }

            /// <summary>
            /// Add a data point
            /// </summary>
            /// <param name="timestamp">the timestamp of the data point</param>
            /// <param name="duration">the duration</param>
            /// <returns>the builder itself</returns>
            public Builder Add(long timestamp, int duration)
            {
                if (timestamps.Count == 0 && duration == -1)
                {
                    throw new ArgumentException("Duration must be specified for the first data point");
                }

                timestamps.Add(timestamp);
                int d = duration;
                if (durations.Count > 0)
                {
                    int n = indexOfLastSpecifiedDuration();
                    if (n >= 0 && duration == durations[n])
                    {
                        d = -1;
                    }
                }
                durations.Add(d);
                return this;
            }

            /// <summary>
            /// Add a data point
            /// </summary>
            /// <param name="timestamp">the timestamp of the data point</param>
            /// <param name="duration">the duration</param>
            /// <returns>the builder itself</returns>
            public Builder Add(DateTime timestamp, TimeSpan? duration)
            {
                if (timestamps.Count == 0 && duration == null)
                {
                    throw new ArgumentException("Duration must be specified for the first data point");
                }
                long tsLong;
                int durInt;
                long ticksPerUnit = TimeSpan.TicksPerMinute;
                switch (unit)
                {
                    case MINUTES:
                        ticksPerUnit = TimeSpan.TicksPerMinute;
                        break;
                    case SECONDS:
                        ticksPerUnit = TimeSpan.TicksPerSecond;
                        break;
                    case MILLIS:
                        ticksPerUnit = TimeSpan.TicksPerMillisecond;
                        break;
                    default:
                        throw new ArgumentException("Unit not supported: " + unit);
                }
                tsLong = timestamp.Ticks / ticksPerUnit;
                durInt = duration == null ? -1 : (int)(duration?.Ticks / ticksPerUnit);

                return Add(tsLong, durInt);
            }

            /// <summary>
            /// Add a data point with the same duration as its previous data point
            /// </summary>
            /// <param name="timestamp">he timestamp of the data point</param>
            /// <returns>the builder itself</returns>
            public Builder Add(long timestamp)
            {
                if (timestamps.Count == 0)
                {
                    throw new ArgumentException("Duration must be specified for the first data point");
                }
                timestamps.Add(timestamp);
                durations.Add(-1);
                return this;
            }

            /// <summary>
            /// Add a data point with the same duration as its previous data point
            /// </summary>
            /// <param name="timestamp">the timestamp of the data point</param>
            /// <returns>the builder itself</returns>
            public Builder Add(DateTime timestamp)
            {
                return Add(timestamp, (TimeSpan?) null);
            }

            /// <summary>
            /// Add a count number ('c') to the current data point
            /// </summary>
            /// <param name="count">the count number</param>
            /// <returns>the builder itself</returns>
            public Builder AddCount(long count)
            {
                if (counts == null)
                {
                    counts = new List<long>(expectedSize);
                }
                counts.Add(count);
                return this;
            }

            /// <summary>
            /// Add a sum number ('s') to the current data point
            /// </summary>
            /// <param name="sum">the sum number</param>
            /// <returns>the builder itself</returns>
            public Builder AddSum(decimal sum)
            {
                if (sums == null)
                {
                    sums = new List<decimal>(expectedSize);
                }
                sums.Add(sum);
                return this;
            }

            /// <summary>
            /// Add a average number ('a') to the current data point
            /// </summary>
            /// <param name="avg">the average number</param>
            /// <returns>the builder itself</returns>
            public Builder AddAvg(decimal avg)
            {
                if (avgs == null)
                {
                    avgs = new List<decimal>(expectedSize);
                }
                avgs.Add(avg);
                return this;
            }

            /// <summary>
            /// Add a minimal number ('m') to the current data point
            /// </summary>
            /// <param name="min">the minimal number</param>
            /// <returns>the builder itself</returns>
            public Builder AddMin(decimal min)
            {
                if (mins == null)
                {
                    mins = new List<decimal>(expectedSize);
                }
                mins.Add(min);
                return this;
            }

            /// <summary>
            /// Add a maximal number ('x') to the current data point
            /// </summary>
            /// <param name="max">the maximal number</param>
            /// <returns>the builder itself</returns>
            public Builder AddMax(decimal max)
            {
                if (maxs == null)
                {
                    maxs = new List<decimal>(expectedSize);
                }
                maxs.Add(max);
                return this;
            }

            /// <summary>
            /// Add a generic number ('n') to the current data point
            /// </summary>
            /// <param name="n">the number</param>
            /// <returns>the builder itself</returns>
            public Builder AddNumber(decimal n)
            {
                if (numbers == null)
                {
                    numbers = new List<decimal>(expectedSize);
                }
                numbers.Add(n);
                return this;
            }

            /// <summary>
            /// Add an object ('o') to the current data point
            /// </summary>
            /// <param name="obj">the object</param>
            /// <returns>the builder itself</returns>
            public Builder AddObj(Object obj)
            {
                if (objs == null)
                {
                    objs = new List<Object>(expectedSize);
                }
                objs.Add(obj);
                return this;
            }

            /// <summary>
            /// Find the index of last explicitly specified duration element
            /// </summary>
            /// <returns>the index of the last explicitly specified duration element, or -1 if not found.</returns>
            private int indexOfLastSpecifiedDuration()
            {
                int result = -1;
                for (int i = durations.Count - 1; i >= 0; i--)
                {
                    if (durations[i] != -1)
                    {
                        result = i;
                        break;
                    }
                }
                return result;
            }

            /// <summary>
            /// Build the CJTSD object
            /// </summary>
            /// <returns>the CJTSD object that is ready to be serialized to JSON</returns>
            public CJTSD Build()
            {
                CJTSD result = new CJTSD();

                // u
                switch (unit)
                {
                    case MINUTES:
                        result.u = null;  // it is the default one
                        break;
                    case SECONDS:
                        result.u = "s";
                        break;
                    case MILLIS:
                        result.u = "S";
                        break;
                    default:
                        throw new ArgumentException("Unit not supported: " + unit);
                }

                // t
                result.t = this.timestamps;

                // d
                int n = indexOfLastSpecifiedDuration();
                if (n >= 0 && n < durations.Count - 1)
                {
                    while(durations.Count > n + 1)
                    {
                        durations.RemoveAt(durations.Count - 1);
                    }
                }
                for (int i = 1; i < durations.Count; i++)
                {
                    int d = durations[i - 1];
                    if (durations[i] == -1 && d < 100)
                    {   // no need to replace with -1 if there are only or less than two digits
                        durations[i] = d;
                    }
                }
                result.d = this.durations;

                // c, s, a, m, x, n, o
                result.c = this.counts;
                result.s = this.sums;
                result.a = this.avgs;
                result.m = this.mins;
                result.x = this.maxs;
                result.n = this.numbers;
                result.o = this.objs;

                return result;
            }

        }


        public class Entry
        {
            public DateTime Timestamp { get; set; }
            public TimeSpan Duration { get; set; }
            public long? Count { get; set; }
            public decimal? Sum { get; set; }
            public decimal? Avg { get; set; }
            public decimal? Min { get; set; }
            public decimal? Max { get; set; }
            public decimal? Number { get; set; }
            public Object Object { get; set; }

            internal Entry(DateTime timestamp, TimeSpan duration, long? count, decimal? sum, decimal? avg, decimal? min, decimal? max, decimal? number, Object obj)
            {
                this.Timestamp = timestamp;
                this.Duration = duration;
                this.Count = count;
                this.Sum = sum;
                this.Avg = avg;
                this.Min = min;
                this.Max = max;
                this.Number = number;
                this.Object = obj;
            }

        }
    }
}
