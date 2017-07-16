using System;
using System.Collections.Generic;

namespace CJTSD.Net
{
    /// <summary>
    /// 
    /// </summary>
    public class CJTSD : PlainCJTSD
    {
        static private IList<Entry> emptyEntries = new List<Entry>().AsReadOnly();

        public CJTSD() : base()
        {

        }

        public CJTSD(PlainCJTSD other) : base(other)
        {

        }

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

        static public Builder Create()
        {
            return new Builder();
        }

        static public Builder Create(int expectedSize)
        {
            return new Builder(expectedSize);
        }

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

            public Builder SetUnitToMinutes()
            {
                this.unit = MINUTES;
                return this;
            }

            public Builder SetUnitToSeconds()
            {
                this.unit = SECONDS;
                return this;
            }

            public Builder SetUnitToMillis()
            {
                this.unit = MILLIS;
                return this;
            }

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

            public Builder Add(DateTime timestamp)
            {
                return Add(timestamp, (TimeSpan?) null);
            }

            public Builder AddCount(long count)
            {
                if (counts == null)
                {
                    counts = new List<long>(expectedSize);
                }
                counts.Add(count);
                return this;
            }

            public Builder AddSum(decimal sum)
            {
                if (sums == null)
                {
                    sums = new List<decimal>(expectedSize);
                }
                sums.Add(sum);
                return this;
            }

            public Builder AddAvg(decimal avg)
            {
                if (avgs == null)
                {
                    avgs = new List<decimal>(expectedSize);
                }
                avgs.Add(avg);
                return this;
            }

            public Builder AddMin(decimal min)
            {
                if (mins == null)
                {
                    mins = new List<decimal>(expectedSize);
                }
                mins.Add(min);
                return this;
            }

            public Builder AddMax(decimal max)
            {
                if (maxs == null)
                {
                    maxs = new List<decimal>(expectedSize);
                }
                maxs.Add(max);
                return this;
            }

            public Builder AddNumber(decimal n)
            {
                if (numbers == null)
                {
                    numbers = new List<decimal>(expectedSize);
                }
                numbers.Add(n);
                return this;
            }

            public Builder AddObj(Object obj)
            {
                if (objs == null)
                {
                    objs = new List<Object>(expectedSize);
                }
                objs.Add(obj);
                return this;
            }

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
