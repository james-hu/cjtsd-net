# CJTSD.Net
This is the C#/.NET library for generating and handling [Compact JSON Time Series Data (CJTSD)](https://github.com/james-hu/cjtsd-js/wiki/Compact-JSON-Time-Series-Data) data in either client or server environment.

There are CJTSD libraries for other programming languages:
* Java - [in jabb-core-java8](https://github.com/james-hu/jabb-core-java8/wiki/CJTSD-Java)
* Javascript - [cjtsd-js](https://github.com/james-hu/cjtsd-js)

# Usage

Latest version of this library ([CJTSD.Net](https://www.nuget.org/packages/CJTSD.Net/)) can be found on [nuget.org](https://www.nuget.org/packages/CJTSD.Net/).

Below is a very simple example showing how to use it:
```c#
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

string jsonString = JsonConvert.SerializeObject(cjtsd, jsonSettings);
```


# Build
```
"C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\Common7\Tools\VsMSBuildCmd.bat"
cd CJTSD
msbuild /t:pack /p:Configuration=Release /p:IncludeSource=true
```