namespace DemoApp
{
    using System.Threading.Tasks;
    using SIS.MvcFramework;

    public static class DemoAppProgram
    {
        public static async Task Main()
        {
            await WebHost.StartAsync(new Startup());
        }
    }
}
