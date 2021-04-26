using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.ServiceProcess;
using System.Xml;

namespace ServiceWrapper
{
    public partial class ServiceWrapper : ServiceBase
    {
        private string ServiceLogName;  
        private string ServiceLogSource;
        private XmlNode Commands;
        private ArrayList ChildProcesses = new ArrayList();

        public ServiceWrapper()
        {
            XmlDocument configuration = new XmlDocument();
            string executablePath = Path.GetDirectoryName(Path.GetFullPath(System.Reflection.Assembly.GetExecutingAssembly().Location));
            configuration.Load(Path.Combine(executablePath, "ServiceWrapper.xml"));
            string serviceName = configuration.SelectSingleNode("//Name").InnerText;
            XmlNode logging = configuration.SelectSingleNode("//Logging");
            ServiceLogName = logging.SelectSingleNode("ServiceLogName").InnerText;
            ServiceLogSource = logging.SelectSingleNode("ServiceLogSource").InnerText;
            Commands = configuration.SelectSingleNode("//Commands");
            InitializeComponent(serviceName);
            if (!System.Diagnostics.EventLog.SourceExists(ServiceLogSource))
            {
                System.Diagnostics.EventLog.CreateEventSource(ServiceLogSource, ServiceLogName);
            }
            eventLog1.Source = ServiceLogSource;
            eventLog1.Log = ServiceLogName;
        }

        private static void KillProcessTree(int procId)
        {
            ManagementObjectSearcher objectSearcher = new ManagementObjectSearcher($"Select * From Win32_Process Where ParentProcessID={procId}");
            ManagementObjectCollection objectCollection = objectSearcher.Get();
            foreach (ManagementObject mo in objectCollection)
            {
                KillProcessTree(Convert.ToInt32(mo["ProcessID"]));
            }
            Process proc = Process.GetProcessById(procId);
            proc.Kill();
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry($"{ServiceName} started");
            foreach (XmlNode command in Commands.ChildNodes)
            {
                if (Convert.ToBoolean(command.Attributes["enabled"].Value))
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = command.SelectSingleNode("Executable").InnerText;
                    proc.StartInfo.Arguments = command.SelectSingleNode("Arguments").InnerText;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.RedirectStandardOutput = true;
                    if (proc.Start())
                    {
                        ChildProcesses.Add(proc.Id);
                        eventLog1.WriteEntry($"Process {proc.Id} running");
                    } else
                    {
                        string output = proc.StandardOutput.ReadToEnd();
                        eventLog1.WriteEntry(output);
                    }
                }
            }
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            foreach (int procId in ChildProcesses)
            {
                try
                {
                    // Unfortunately due to .NET compatibility issues Process.Kill(true) can't be used
                    KillProcessTree(procId);
                }
                catch (System.ArgumentException)
                {
                    eventLog1.WriteEntry($"Process {procId} not running");
                }
                catch (Exception exc)
                {
                    eventLog1.WriteEntry($"Failed killing {procId}: {exc}");
                }
            }
            ChildProcesses.Clear();
            eventLog1.WriteEntry($"{ServiceName} stopped");
        }

    }

}