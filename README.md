# Woz.TEMPer

C# Library for the TEMPer sensors. 

This is an early but functional cut that works with the TEMPerV1.4 sensor. Tested with multiple sensors connected at once.

Feel free to help add more of the TEMPer sensor family if you have access to them. Not much code is required.

```
// Uses Device.NET nuget package so prep that
TEMPerSensors.Register(null, null);

// Connect to all TEMPer sensors on the machine
var sensors = TEMPerSensors.Create(null);
await sensors.InitialiseAsync();

// Get temp from them all
var results = await sensors.ReadTemperatures();
```
