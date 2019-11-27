using System.Threading.Tasks;
using Woz.TEMPer;

namespace TestRig
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Quick and dirty test rig, not planned to be part of library

            TEMPerSensors.Register(null, null);

            var sensors = TEMPerSensors.Create(null);
            await sensors.InitialiseAsync();
            var results = await sensors.ReadTemperatures();
        }
    }
}
