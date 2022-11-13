using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Cjtsd.Net.Tests
{
    [TestClass]
    public class CJTSDTest
    {
        JsonSerializerSettings jsonSettings = new JsonSerializerSettings();

        public CJTSDTest()
        {
            jsonSettings.NullValueHandling = NullValueHandling.Ignore;
        }

        [TestMethod]
        public void TestEmpty()
        {
            CJTSD cjtsd = CJTSD.Create().Build();

            Assert.IsNotNull(cjtsd);
            Assert.AreEqual(0, cjtsd.t.Count);
            Assert.AreEqual(0, cjtsd.d.Count);

        }

        [TestMethod]
        public void TestWithNoDurationSpecified()
        {
            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
            CJTSD cjtsd = CJTSD.Create().SetUnitToMillis()
                .Add(start.Add(duration))
                .AddCount(10L)
                .Add(start.Add(duration).Add(duration))
                .AddCount(20L)
                .Add(start.Add(duration).Add(duration).Add(duration))
                .AddCount(30L)
                .Build();

            Assert.AreEqual(3, cjtsd.t.Count);
            Assert.AreEqual(3, cjtsd.c.Count);
            Assert.AreEqual(1, cjtsd.d.Count);    // [500]
            Assert.AreEqual(0, cjtsd.d[0]);

            string jsonString = JsonConvert.SerializeObject(cjtsd, jsonSettings);
            System.Diagnostics.Debug.WriteLine(jsonString);

            IList<CJTSD.Entry> list = JsonConvert.DeserializeObject<CJTSD>(jsonString).ToList();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(10, list[0].Count);
            Assert.AreEqual(20, list[1].Count);
            Assert.AreEqual(30, list[2].Count);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, 0), list[0].Duration);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, 0), list[1].Duration);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, 0), list[2].Duration);
        }

        [TestMethod]
        public void TestSameDuration()
        {
            DateTime start = DateTime.Now;
            TimeSpan duration = new TimeSpan(0, 0, 0, 0, 500);
            CJTSD cjtsd = CJTSD.Create().SetUnitToMillis()
                .Add(start.Add(duration), duration)
                .AddCount(10L)
                .Add(start.Add(duration).Add(duration), duration)
                .AddCount(20L)
                .Add(start.Add(duration).Add(duration).Add(duration), duration)
                .AddCount(30L)
                .Build();

            Assert.AreEqual(3, cjtsd.t.Count);
            Assert.AreEqual(3, cjtsd.c.Count);
            Assert.AreEqual(1, cjtsd.d.Count);    // [500]
            Assert.AreEqual(500, cjtsd.d[0]);

            string jsonString = JsonConvert.SerializeObject(cjtsd, jsonSettings);
            System.Diagnostics.Debug.WriteLine(jsonString);

            IList<CJTSD.Entry> list = JsonConvert.DeserializeObject<CJTSD>(jsonString).ToList();
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(10, list[0].Count);
            Assert.AreEqual(20, list[1].Count);
            Assert.AreEqual(30, list[2].Count);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, 500), list[0].Duration);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, 500), list[1].Duration);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 0, 500), list[2].Duration);


            duration = new TimeSpan(0, 0, 0, 500, 0);
            cjtsd = CJTSD.Create().SetUnitToSeconds()
                    .Add(start.Add(duration), duration)
                    .AddCount(10L)
                    .Add(start.Add(duration).Add(duration), duration)
                    .AddCount(20L)
                    .Add(start.Add(duration).Add(duration).Add(duration), duration)
                    .AddCount(30L)
                    .Add(start.Add(duration).Add(duration).Add(duration).Add(duration), new TimeSpan(0, 0, 0, 100, 0))
                    .AddCount(40L)
                    .Add(start.Add(duration).Add(duration).Add(duration).Add(duration).Add(new TimeSpan(0, 0, 0, 100, 0)), new TimeSpan(0, 0, 0, 100, 0))
                    .AddCount(50L)
                    .Build();
            Assert.AreEqual(5, cjtsd.t.Count);
            Assert.AreEqual(5, cjtsd.c.Count);
            Assert.AreEqual(4, cjtsd.d.Count);   // [500, -1, -1, 100]
            Assert.AreEqual(500, cjtsd.d[0]);
            Assert.AreEqual(-1, cjtsd.d[1]);
            Assert.AreEqual(-1, cjtsd.d[2]);
            Assert.AreEqual(100, cjtsd.d[3]);

            jsonString = JsonConvert.SerializeObject(cjtsd, jsonSettings);
            System.Diagnostics.Debug.WriteLine(jsonString);
            list = JsonConvert.DeserializeObject<CJTSD>(jsonString).ToList();

            Assert.AreEqual(5, list.Count);
            Assert.AreEqual(10, list[0].Count);
            Assert.AreEqual(20, list[1].Count);
            Assert.AreEqual(30, list[2].Count);
            Assert.AreEqual(40, list[3].Count);
            Assert.AreEqual(50, list[4].Count);
            Assert.AreEqual(duration, list[0].Duration);
            Assert.AreEqual(duration, list[1].Duration);
            Assert.AreEqual(duration, list[2].Duration);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 100, 0), list[3].Duration);
            Assert.AreEqual(new TimeSpan(0, 0, 0, 100, 0), list[4].Duration);

            
		    duration = new TimeSpan(0, 0, 0, 99, 0);
		    cjtsd = CJTSD.Create().SetUnitToSeconds()
				.Add(start.Add(duration), duration)
				.AddCount(10L)
				.Add(start.Add(duration).Add(duration), duration)
				.AddCount(20L)
				.Add(start.Add(duration).Add(duration).Add(duration), duration)
				.AddCount(30L)
				.Build();

            Assert.AreEqual(3, cjtsd.t.Count);
            Assert.AreEqual(3, cjtsd.c.Count);
            Assert.AreEqual(1, cjtsd.d.Count);    // [99]
            Assert.AreEqual(99, cjtsd.d[0]);

            jsonString = JsonConvert.SerializeObject(cjtsd, jsonSettings);
            System.Diagnostics.Debug.WriteLine(jsonString);
            list = JsonConvert.DeserializeObject<CJTSD>(jsonString).ToList();

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(duration, list[0].Duration);
            Assert.AreEqual(duration, list[1].Duration);
            Assert.AreEqual(duration, list[2].Duration);



		    duration = new TimeSpan(0, 0, 99, 0, 0);
		    cjtsd = CJTSD.Create().SetUnitToMinutes()
				.Add(start.Add(duration), duration)
				.AddCount(10L)
				.Add(start.Add(duration).Add(duration), duration)
				.AddCount(20L)
				.Add(start.Add(duration).Add(duration).Add(duration), duration)
				.AddCount(30L)
				.Add(start.Add(duration).Add(duration).Add(duration),new TimeSpan(0, 0, 100, 0, 0))
				.AddCount(40L)
				.Build();

            Assert.AreEqual(4, cjtsd.t.Count);
            Assert.AreEqual(4, cjtsd.c.Count);
            Assert.AreEqual(4, cjtsd.d.Count);    // [99, 99, 99, 100]
            Assert.AreEqual(99, cjtsd.d[0]);
            Assert.AreEqual(99, cjtsd.d[1]);
            Assert.AreEqual(99, cjtsd.d[2]);
            Assert.AreEqual(100, cjtsd.d[3]);

            jsonString = JsonConvert.SerializeObject(cjtsd, jsonSettings);
            System.Diagnostics.Debug.WriteLine(jsonString);
            list = JsonConvert.DeserializeObject<CJTSD>(jsonString).ToList();

            Assert.AreEqual(4, list.Count);
            Assert.AreEqual(duration, list[0].Duration);
            Assert.AreEqual(duration, list[1].Duration);
            Assert.AreEqual(duration, list[2].Duration);
            Assert.AreEqual(new TimeSpan(0, 0, 100, 0, 0), list[3].Duration);
        }
    }
}
