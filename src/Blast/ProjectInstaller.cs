using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.ServiceProcess;


namespace Blast
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void blastServiceInstaller_Committed(object sender, InstallEventArgs e)
        {
            new ServiceController(blastServiceInstaller.ServiceName).Start();
            Tools.EventLogger.WriteEntry("Blast::ProjectInstaller: Blast correctly installed and started.", EventLogEntryType.Information);
        }
    }
}
