using System.ServiceProcess;

namespace Blast
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the service application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new srvcBlast() };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
