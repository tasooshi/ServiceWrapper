using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace ServiceWrapper
{
    partial class ProjectInstaller
    {
        private System.ComponentModel.IContainer components = null;
        private Dictionary<string, ServiceAccount> serviceAccountMapping = new Dictionary<string, ServiceAccount> {
            {"LocalSystem", ServiceAccount.LocalSystem},
            {"LocalService", ServiceAccount.LocalService},
            {"NetworkService", ServiceAccount.NetworkService},
            {"User", ServiceAccount.User},
        };
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent(string serviceName, string serviceDescription, string runAs)
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            this.serviceProcessInstaller1.Account = serviceAccountMapping[runAs];
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            this.serviceInstaller1.ServiceName = serviceName;
            this.serviceInstaller1.Description = serviceDescription;
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.serviceInstaller1});
        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
    }
}