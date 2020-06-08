using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text;
using System.Xml;

namespace ServiceWrapper
{
    public partial class ServiceWrapper : ServiceBase
    {
        private string serviceLogName;
        private string serviceLogSource;
        private XmlNode commands;
        private ArrayList childProcesses = new ArrayList();

        public ServiceWrapper()
        {
            XmlDocument configuration = new XmlDocument();
            string executablePath = Path.GetDirectoryName(Path.GetFullPath(System.Reflection.Assembly.GetExecutingAssembly().Location));
            configuration.Load(Path.Combine(executablePath, "ServiceWrapper.xml"));
            string serviceName = configuration.SelectSingleNode("//Name").InnerText;
            XmlNode logging = configuration.SelectSingleNode("//Logging");
            serviceLogName = logging.SelectSingleNode("ServiceLogName").InnerText;
            serviceLogSource = logging.SelectSingleNode("ServiceLogSource").InnerText;
            commands = configuration.SelectSingleNode("//Commands");

            InitializeComponent(serviceName);

            if (!System.Diagnostics.EventLog.SourceExists(serviceLogSource))
            {
                System.Diagnostics.EventLog.CreateEventSource(serviceLogSource, serviceLogName);
            }
            eventLog1.Source = serviceLogSource;
            eventLog1.Log = serviceLogName;

            foreach (XmlNode command in commands.ChildNodes)
            {
                if (Convert.ToBoolean(command.Attributes["enabled"].Value))
                {
                    Process proc = new Process();
                    proc.StartInfo.FileName = command.SelectSingleNode("Executable").InnerText;
                    proc.StartInfo.Arguments = command.SelectSingleNode("Arguments").InnerText;
                    proc.Start();
                    if (proc != null)
                    {
                        childProcesses.Add(proc.Id);
                    }
                }
            }
        }

        private static void KillProcessTree(int procId)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher($"Select * From Win32_Process Where ParentProcessID={procId}");
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                KillProcessTree(Convert.ToInt32(mo["ProcessID"]));
            }
            Process proc = Process.GetProcessById(procId);
            proc.Kill();
        }

        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry($"{ServiceName} started");
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
        }

        protected override void OnStop()
        {
            foreach (int procId in childProcesses)
            {
                try
                {
                    // Unfortunately due to .NET compatibility issues Process.Kill(true) can't be used
                    KillProcessTree(procId);
                }
                catch (Exception exc)
                {
                    eventLog1.WriteEntry($"Failed killing {procId}: {exc}");
                }
            }
            childProcesses.Clear();
            eventLog1.WriteEntry($"{ServiceName} stopped");
        }

        protected override void OnPause()
        {
            eventLog1.WriteEntry($"{ServiceName} paused");
        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry($"{ServiceName} resumed");
        }

    }

}