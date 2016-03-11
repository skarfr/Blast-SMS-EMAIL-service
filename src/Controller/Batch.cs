using System;
using System.Linq;
using Model.database;
using Model;
using System.Diagnostics;

namespace Controller
{
    /// <summary>
    /// A Batch object is a list of Messages getting sent
    /// </summary>
    public class Batch
    {
        private cDatabase _db = new cDatabase();

        /// <summary>
        /// A new batch instantiation will try to send all Messages not yet sent (aka SentTime == null)
        /// </summary>
        public Batch()
        {
            try
            {
                var result =from m in _db.Blast.MessagesOuts
                            where m.SentTime == null
                            select m.Id;

                if (result.Any())                           // if any, try to send those messages
                {
                    Tools.EventLogger.WriteEntry("Controller::Batch: " + result.Count() + " message(s) are ready to be sent", EventLogEntryType.Information);
                    foreach (var m in result.ToList())
                    {
                        try {
                            Message message = new Message(m);
                            try { Messenger.SendMessage(message); }
                            catch (Exception ex)
                            {
                                //A new Message has been instantiated. Here we handle exceptions raised by anything else (update message, new provider...)
                                Tools.EventLogger.WriteEntry("Controller::Batch: Message " + message.Id + " not sent. " + ex.Message, EventLogEntryType.Error);
                                message.NotSent(ex.Message);
                                continue;
                            }
                        }
                        catch (Exception ex)
                        {
                            //Here we handle exceptions raised by the instantiation of a new Message
                            Tools.EventLogger.WriteEntry("Controller::Batch: " + ex.Message, EventLogEntryType.Error);
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Tools.EventLogger.WriteEntry("Controller::Batch: " + ex.Message, EventLogEntryType.Error);
            }   
        }
    }
}
