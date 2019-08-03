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
