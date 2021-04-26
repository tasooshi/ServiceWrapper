using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Xml;

namespace ServiceWrapper
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            XmlDocument configuration = new XmlDocument();
            string executablePath = Path.GetDirectoryName(Path.GetFullPath(System.Reflection.Assembly.GetExecutingAssembly().Location));
            configuration.Load(Path.Combine(executablePath, "ServiceWrapper.xml"));
            string serviceName = configuration.SelectSingleNode("//Name").InnerText;
            string serviceDescription = configuration.SelectSingleNode("//Description").InnerText;
            string runAs = configuration.SelectSingleNode("//RunAs").InnerText;
            InitializeComponent(serviceName, serviceDescription, runAs);
        }
    }
}
