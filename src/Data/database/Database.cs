using System;
using System.Diagnostics;
using System.Configuration;

namespace Model.database
{
    public class cDatabase
    {
        private BlastDataContext _blast;
        public BlastDataContext Blast
        {
            get { return this._blast; }
        }

        public cDatabase()
        {
            try
            {
                string connexion;
#if DEBUG
                connexion = Properties.Settings.Default.BlastConnectionStringDEV;
#else
                connexion = Properties.Settings.Default.BlastConnectionString;
#endif
                if (!string.IsNullOrEmpty(connexion))
                    this._blast = new BlastDataContext(connexion);
            }
            catch (Exception ex)
            {
                Tools.EventLogger.WriteEntry("Data::Database: " + ex.Message, EventLogEntryType.Error);
            }
        }
    }
}