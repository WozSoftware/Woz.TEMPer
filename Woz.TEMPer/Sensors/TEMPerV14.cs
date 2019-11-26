using Device.Net;
using System.Threading.Tasks;

namespace Woz.TEMPer.Sensors
{
    public static class TEMPerV14
    {
        private const decimal CalibrationOffset = 0m;
        private const decimal CalibrationScale = 1m;

        public static uint VendorId = 0x0c45;
        public static uint ProductId = 0x7401;

        public static FilterDeviceDefinition Definition
            => new FilterDeviceDefinition {DeviceType = DeviceType.Hid, VendorId = VendorId, ProductId = ProductId};

        public static async Task<decimal> ReadTemperature(IDevice device)
        {
            var buffer = new byte[9] { 0x00, 0x01, 0x80, 0x33, 0x01, 0x00, 0x00, 0x00, 0x00 };

            var rawResult = await device.WriteAndReadAsync(buffer);
            int tempRaw = (rawResult.Data[4] & 0xFF) + (rawResult.Data[3] << 8);
            return (CalibrationScale * (tempRaw * (125.0m / 32000.0m))) + CalibrationOffset;
        }
    }
}
