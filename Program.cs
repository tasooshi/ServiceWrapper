using System.ServiceProcess;

namespace ServiceWrapper
{
    static class Program
    {
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new ServiceWrapper()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
