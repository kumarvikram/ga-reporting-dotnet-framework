# C# Console App to get reports from Google Analytics
C# console app to read data from Google Analytics using Google Analytics Reporting API V4 and save to file

## Prerequisites
For step by step guide on how to do API and credential setup in Google Developer Console
[Read Google Analytics reports using C# console app](https://kumarvikram.com/google-analytics-report-v4-csharp-console-app/ "Step by step guide on setting up project in Google Developer Console")

## How to run?
* Replace key.json with your actual key file generated from Google Developer Console. 
* Update KeyFileName key in App.config with your json file name.
```sh
 <add key="KeyFileName" value="key.json"/>
```
* Google Analytics ViewId(s) to fetch report from. Separate by comma if there are multiple ViewIds.
```sh
<add key="Views" value="123456,123456"/>
```
* Folder location where generated report files would be saved
```sh
<add key="OutputDirectory" value="C:\Reports" />
```
* A sample report configuration 
```sh
<ReportConfiguration>
  <!--Specify either Start & End date in MM/DD/YYYY format or NumberOfDays. In case of start and end date, number of days configuration would be skipped-->
  <DateConfiguration StartDate="" EndDate="" NumberOfDays="1" />
  <Reports>
    <Report name="PageViews" metrics="ga:pageviews" dimensions="ga:dateHourMinute,ga:pagePath" />      
  </Reports>
</ReportConfiguration>
```
View the complete list of metrics and dimensions that you can use [here](https://ga-dev-tools.appspot.com/dimensions-metrics-explorer/ "Dimensions & Metrics Explorer")
