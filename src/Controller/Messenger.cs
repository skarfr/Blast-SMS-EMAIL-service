using System;
using System.Collections.Generic;
using System.Linq;
using Model.database;
using Model;

namespace Controller
{
    public sealed class Messenger
    {
        #region Private Static properties
        /// <summary>
        /// Database instance
        /// </summary>
        private static cDatabase _db = new cDatabase();

        /// <summary>
        /// List of available providers
        /// </summary>
        private static List<Provider> _providers = new List<Provider>();
        #endregion

        public Messenger() { }

        /// <summary>
        /// Try to send a message with the latest available & enabled Provider sharing the same MessageType as the given message
        /// </summary>
        /// <param name="message">message to be sent</param>
        public static void SendMessage(Message message)
        {
            if (String.IsNullOrEmpty(message.MessageType))
                throw new Exception("Controller::Messenger: The messageType for MessageId=" + message.Id + " is not specified");

            loadProvidersList();         // we refresh everytime in case someone add/update Providers in the database during a batch run
            Provider provider = _providers.FindLast(p => p.MessageType == message.MessageType);

            if (provider == null)
                throw new Exception("Controller::Messenger: No provider found for MessageType=" + message.MessageType + ", related to MessageId=" + message.Id);

            provider.Send(message);     // if any error, the function will raise an exception and the following catch must update the message status to NotSent
        }

        /// <summary>
        /// Load the list of providers with all the enabled ones
        /// </summary>
        private static void loadProvidersList()
        {
            try
            {
                var q = from p in _db.Blast.Providers
                        where p.Enable == true
                        select p;

                foreach (var p in q.ToList())
                    _providers.Add(new Provider(p));
            }
            catch (Exception ex)
            {
                throw new Exception("Controller::Messenger: loadProvidersList Exception: " + ex.Message);
            }
        }
    }
}

