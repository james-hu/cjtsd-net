using System;
using System.Collections.Generic;

namespace CJTSD.Net
{
    /// <summary>
    /// This is the DTO class for <see href="https://github.com/james-hu/cjtsd-js/wiki/Compact-JSON-Time-Series-Data">Compact JSON Time Series Data (CJTSD)</see> data.
    /// To generate a CJTSD object, use <code>CJTSD</code> class.
    /// </summary>
    public class PlainCJTSD
    {
        static private IList<RawEntry> emptyRawEntries = new List<RawEntry>().AsReadOnly();

        public string u { get; set; }
        public IList<long> t { get; set; }
        public IList<int> d { get; set; }

        public IList<long> c { get; set; }
        public IList<decimal> s { get; set; }
        public IList<decimal> a { get; set; }
        public IList<decimal> m { get; set; }
        public IList<decimal> x { get; set; }
        public IList<decimal> n { get; set; }
        public IList<Object> o { get; set; }

        public PlainCJTSD()
        {

        }

        /// <summary>
        /// Shallow copy constructor
        /// </summary>
        /// <param name="other">another instance from which the properties will be copied</param>
        public PlainCJTSD(PlainCJTSD other)
        {
            this.u = other.u;
            this.t = other.t;
            this.d = other.d;
            this.c = other.c;
            this.s = other.s;
            this.a = other.a;
            this.m = other.m;
            this.x = other.x;
            this.n = other.n;
            this.o = other.o;
        }

        /// <summary>
        /// Convert into raw list form.
        /// </summary>
        /// <returns>the list containing entries of data points</returns>
        public IList<RawEntry> ToRawList()
        {
            if (t == null || t.Count == 0)
            {
                return emptyRawEntries;
            }

            IList<RawEntry> result = new List<RawEntry>(t.Count);
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

                long timestampMillis;
                long durationMillis;
                if (u == null || u.Equals("m"))
                {
                    timestampMillis = 1000L * 60 * timestamp;
                    durationMillis = 1000L * 60 * duration;
                }
                else if (u.Equals("s"))
                {
                    timestampMillis = 1000L * timestamp;
                    durationMillis = 1000L * duration;
                }
                else if (u.Equals("S"))
                {
                    timestampMillis = timestamp;
                    durationMillis = duration;
                }
                else
                {
                    throw new ArgumentException("Unit not supported: " + u);
                }

                result.Add(new RawEntry(timestampMillis, durationMillis,
                        c == null || i >= c.Count ? (long?) null : c[i],
                        s == null || i >= s.Count ? (decimal?) null : s[i],
                        a == null || i >= a.Count ? (decimal?)null : a[i],
                        m == null || i >= m.Count ? (decimal?)null : m[i],
                        x == null || i >= x.Count ? (decimal?)null : x[i],
                        n == null || i >= n.Count ? (decimal?)null : n[i],
                        o == null || i >= o.Count ? null : o[i]
                        ));

            }
            return result;
        }


        public class RawEntry
        {
            /// <summary>
            /// Timestamp as number of milliseconds since 00:00:00 local time, 1 January 1970
            /// </summary>
            public long t { get; set; }
            public long d { get; set; }
            public long? c { get; set; }
            public decimal? s { get; set; }
            public decimal? a { get; set; }
            public decimal? m { get; set; }
            public decimal? x { get; set; }
            public decimal? n { get; set; }
            public Object o { get; set; }

            internal RawEntry(long timestamp, long duration, long? count, decimal? sum, decimal? avg, decimal? min, decimal? max, decimal? number, Object obj)
            {
                this.t = timestamp;
                this.d = duration;
                this.c = count;
                this.s = sum;
                this.a = avg;
                this.m = min;
                this.x = max;
                this.n = number;
                this.o = obj;
            }

        }
    }
}
