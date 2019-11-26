using Device.Net;
using Hid.Net.Windows;
using System;
using System.Threading;
using Usb.Net.Windows;
using Woz.TEMPer;

namespace TestRig
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            // Quick and dirty test rig, not planned to be part of library

            TEMPerSensors.Register(null, null);

            var sensor = new TEMPerSensors(null);
            sensor.Start();
            await sensor.InitialiseAsync();
            var results = await sensor.ReadTemperatures();
        }
    }
}
