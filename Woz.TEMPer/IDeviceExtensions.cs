using Device.Net;

namespace Woz.TEMPer
{
    public static class IDeviceExtensions
    {
        public static SensorType DetermineType(this IDevice device)
        {
            if(device.ConnectedDeviceDefinition?.ProductName == "TEMPerV1.4")
                return SensorType.TEMPerV14;
            
            if (device.ConnectedDeviceDefinition is {ProductId: 8455, VendorId: 16701})
            {
                //Manufacturer and product name are corrupt, no easy way to differentiate GoldV31 and XV31
                //We can just use the XV31 code and throw away the humidity if it's not available
                return SensorType.TEMPerXV31;
            }
            
            return SensorType.Unknown;
        }
    }
}
