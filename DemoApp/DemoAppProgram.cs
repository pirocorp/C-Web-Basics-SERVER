namespace DemoApp
{
    using System.Threading.Tasks;
    using SIS.HTTP;

    public static class DemoAppProgram
    {
        public static async Task Main()
        {
            // Actions;
            // / => Response IndexPage(Request)
            // /favicon.ico => favicon.ico
            // GET /Contact => Response ShowContactForm(Request)
            // POST /Contact => Response FillContactForm(Request)

            // new HttpServer(80, Actions)
            // .Start();

            var httpServer = new HttpServer(80);
            await httpServer.StartAsync();
        }
    }
}
