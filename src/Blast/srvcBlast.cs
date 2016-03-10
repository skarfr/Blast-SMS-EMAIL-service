using System.ServiceProcess;
using System.Timers;
using Tools;
using Controller;

namespace Blast
{
    public partial class srvcBlast : ServiceBase
    {
        private Timer _timer = null;

        internal srvcBlast()
        {
            InitializeComponent();
            this._timer = new Timer(60000); //the timer fire every sixty seconds
            this._timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed); //when fire, call _timer_Elapsed
            Tools.EventLogger.CreateSource();
        }

        #region Service functions override
        protected override void OnStart(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            this.timerStart();
        }

        protected override void OnStop()
        {
            this.timerStop();
        }

        protected override void OnShutdown()
        {
            base.OnShutdown();
            this.timerStop();
        }

        protected override void OnPause()
        {
            base.OnPause();
            this.timerStop();
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            this.timerStart();
        }
        #endregion

        #region Timer functions
        // This method is called when the timer fires
        protected void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.timerStop();
            Batch currentBatch = new Batch();
            this.timerStart();
        }

        private void timerStart()
        {
            this._timer.Start();
            //Tools.EventLogger.WriteEntry("Blast::srvcBlast: Timer started", EventLogEntryType.Information);
        }

        private void timerStop()
        {
            this._timer.Stop();
            //Tools.EventLogger.WriteEntry("Blast::srvcBlast: Timer stoped", EventLogEntryType.Information);
        }
        #endregion
    }
}
