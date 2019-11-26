using Device.Net;

namespace Woz.TEMPer
{
    public static class IDeviceExtensions
    {
        public static SensorType DetermineType(this IDevice device)
        {
            switch (device?.ConnectedDeviceDefinition?.ProductName)
            {
                case "TEMPerV1.4":
                    return SensorType.TRMPerV14;
            }

            return SensorType.Unknown;
        }
    }
}
