namespace ServiceWrapper
{
    partial class ServiceWrapper
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent(string serviceName)
        {
            this.eventLog1 = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();
            this.ServiceName = serviceName;
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();
        }

        #endregion

        private System.Diagnostics.EventLog eventLog1;
    }
}
