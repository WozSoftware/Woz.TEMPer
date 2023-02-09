using Device.Net;
using Hid.Net.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Woz.Functional.Monads;
using Woz.Linq;
using Woz.TEMPer.Sensors;

namespace Woz.TEMPer
{
    // See https://github.com/urwen/temper/blob/master/temper.py for some reference

    public sealed class TEMPerSensors : IDisposable
    {
        private const int PollMilliseconds = 3000;
        private const string BulkId = "mi_01"; // Believe same across all TEMPer type sensors

        private readonly FilterDeviceDefinition[] _deviceDefinitions = {TEMPerV14.Definition, TEMPerGoldV31.Definition, TEMPerXV31.Definition};
        private bool _disposed = false;
        private DeviceListener _deviceListener;
        private IDictionary<string, IDevice> _devices = new Dictionary<string, IDevice>();

        public event EventHandler? SensorInitialized;
        public event EventHandler? SensorDisconnected;

        public static void Register(ILogger? logger, ITracer? tracer) 
            => WindowsHidDeviceFactory.Register(logger, tracer);

        public static TEMPerSensors Create(ILogger? logger) => new TEMPerSensors(logger);

        private TEMPerSensors(ILogger? logger)
        {
            _deviceListener = new DeviceListener(_deviceDefinitions, PollMilliseconds) {Logger = logger};
            _deviceListener.DeviceInitialized += DeviceInitializedHandler;
            _deviceListener.DeviceDisconnected += DeviceDisconnectedHandler;
        }

        public IEnumerable<IDevice> Devices => _devices.Values;

        public async Task InitialiseAsync()
        {
            _deviceListener.Start();

            var devices = await DeviceManager.Current.GetDevicesAsync(_deviceDefinitions);
            var bulkDevices = devices.Where(device => device.DeviceId.Contains(BulkId)).ToArray();

            foreach (var device in bulkDevices)
            {
                await device.InitializeAsync();
                _devices[device.DeviceId] = device;
            }
        }

        public Task<SensorResult[]> ReadTemperatures() => Task.WhenAll(Devices.Select(ReadTemperature));

        public async Task<SensorResult> ReadTemperature(IDevice device)
        {
            if (device.IsInitialized)
            {
                try
                {
                    return await ReadDevice(device);
                }
                catch (Exception ex)
                {
                    return SensorResult.Error(device, ex.Message);
                }
            }
            else
            {
                return SensorResult.Error(device, "Device not initialised");
            }
        }

        private async Task<SensorResult> ReadDevice(IDevice device)
        {
            switch (device.DetermineType())
            {
                case SensorType.TEMPerV14:
                    return SensorResult.Create(device, await TEMPerV14.ReadTemperature(device));

                case SensorType.TEMPerGoldV31:
                    return SensorResult.Create(device, await TEMPerGoldV31.ReadTemperature(device));

                case SensorType.TEMPerXV31:
                    var temp = await TEMPerXV31.ReadTemperature(device);
                    var hum = await TEMPerXV31.ReadHumidity(device);

                    if(hum == 200) //this value means it's really a GoldV31 pretending to be an XV31
                        return SensorResult.Create(device, temp);

                    return SensorResult.Create(device,temp, hum);

                default:
                    return SensorResult.Error(device, "Device not supported");
            }
        }

        private void DeviceInitializedHandler(object? sender, DeviceEventArgs args)
        {
            _devices[args.Device.DeviceId] = args.Device;
            SensorInitialized?.Invoke(this, new EventArgs());
        }

        private void DeviceDisconnectedHandler(object? sender, DeviceEventArgs args)
        {
            _devices.Remove(args.Device.DeviceId);
            SensorDisconnected?.Invoke(this, new EventArgs());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~TEMPerSensors() => Dispose(false);

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _deviceListener.DeviceInitialized -= DeviceInitializedHandler;
                _deviceListener.DeviceDisconnected -= DeviceDisconnectedHandler;
                _deviceListener.Dispose();

                _devices.Values.ForEach(device => device.Dispose());
            }

            _disposed = true;
        }
    }
}
