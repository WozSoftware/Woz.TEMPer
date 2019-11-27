using Device.Net;
using Woz.Functional.Monads;

namespace Woz.TEMPer
{
    public sealed class SensorResult
    {
        private static string Unknown = "Unknown";

        public static SensorResult Create(IDevice device, decimal temperature)
            => new SensorResult(device, Result<decimal, string>.Create(temperature));

        public static SensorResult Error(IDevice device, string errorMessage)
            => new SensorResult(device, Result<decimal, string>.Raise(errorMessage));

        private SensorResult(IDevice device, Result<decimal, string> result)
        {
            SensorType = device.DetermineType();
            DeviceId = device.DeviceId;
            Manufacturer = device.ConnectedDeviceDefinition?.Manufacturer ?? Unknown;
            Result = result;
        }

        public SensorType SensorType { get; }
        public string DeviceId { get; }
        public string Manufacturer { get; }
        public Result<decimal, string> Result { get; }
    }
}
