namespace Blast
{
    partial class ProjectInstaller
    {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">True if managed resources should be disposed; otherwise, False.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
            components.Dispose();
        base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.BlastServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
        this.blastServiceInstaller = new System.ServiceProcess.ServiceInstaller();
        // 
        // BlastServiceProcessInstaller
        // 
        this.BlastServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
        this.BlastServiceProcessInstaller.Password = null;
        this.BlastServiceProcessInstaller.Username = null;
        // 
        // blastServiceInstaller
        // 
        this.blastServiceInstaller.Description = "Blast Service sends messages to customers (SMS, emails)";
        this.blastServiceInstaller.DisplayName = "Blast";
        this.blastServiceInstaller.ServiceName = "Blast";
        this.blastServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
        this.blastServiceInstaller.Committed += new System.Configuration.Install.InstallEventHandler(this.blastServiceInstaller_Committed);
        // 
        // ProjectInstaller
        // 
        this.Installers.AddRange(new System.Configuration.Install.Installer[] {
        this.BlastServiceProcessInstaller,
        this.blastServiceInstaller});

    }

    #endregion

    private System.ServiceProcess.ServiceProcessInstaller BlastServiceProcessInstaller;
    private System.ServiceProcess.ServiceInstaller blastServiceInstaller;
    }
}