using System;
using System.Collections.Generic;
using System.Data.Linq;
using Model.database;
using System.Text;
using System.Linq;

namespace Model
{
    /// <summary>
    /// The Message class declares all functions used to select/check/update a particular row in the table MessageOut
    /// </summary>
    public class Message
    {
        #region private properties
        /// <summary>
        /// Row from the table Blast.MessagesOut
        /// </summary>
        private MessagesOut _message;

        /// <summary>
        /// Database instance
        /// </summary>
        private readonly cDatabase _db = new cDatabase();

        /// <summary>
        /// Available list of messages statuses
        /// </summary>
        private readonly List<string> _statuses = new List<string>() { "sending", "notsent", "sent" };
        #endregion

        #region Public accessors to MessagesOut properties
        /// <summary>
        /// Accessor to the Id
        /// </summary>
        public int Id
        {
            get { return this._message.Id; }
        }

        /// <summary>
        /// Accessor to the Type
        /// </summary>
        public string MessageType
        {
            get { return this._message.MessageType; }
        }
        #endregion

        #region Private accessors to MessagesOut properties
        /// <summary>
        /// Accessor to the Sender
        /// </summary>
        private string sender
        {
            get { return this._message.Sender; }
        }

        /// <summary>
        /// Accessor to the SenderName
        /// </summary>
        private string senderName
        {
            get { return this._message.SenderName; }
        }

        /// <summary>
        /// Accessor to the Receiver
        /// </summary>
        private string receiver
        {
            get { return this._message.Receiver; }
        }

        /// <summary>
        /// Accessor to the ReceiverName
        /// </summary>
        private string receiverName
        {
            get { return this._message.ReceiverName; }
        }

        /// <summary>
        /// Accessor to the Subject
        /// </summary>
        private string subject
        {
            get { return this._message.Subject; }
        }

        /// <summary>
        /// Accessor to the content
        /// </summary>
        private string content
        {
            get { return this._message.Message; }
        }

        #endregion

        /// <summary>
        /// Constructor of the Message object
        /// </summary>
        /// <param name="message">A row from the table Blast.MessagesOut</param>
        public Message(int id)
        {
            if (id < 0)
                throw new Exception("Model::Message: Message " + id + " not found. The Message ID can not be < 0");

            try
            {
                this._message = this._db.Blast.MessagesOuts.Single(m => m.Id == id);
            }
            catch (InvalidOperationException ioe) //The collection does not contain exactly one element OR the input sequence is empty
            {
                throw new Exception("Model::Message: Message " + id + " not found. " + ioe.Message);
            }
        }

        #region Check data validation according to Type
        /// <summary>
        /// This function checks if the current message is a valid emailing and raises an exception otherwise
        /// </summary>
        /// <returns>True if valid, otherwise it raises an exception</returns>
        internal bool IsValidEmailing()
        {
            if(this.MessageType.ToUpper() != "EMAIL")
                throw new Exception("Model::Message: The Message Type is not an EMAIL");

            if (!Tools.Verif.Email(this.sender))
                throw new Exception("Model::Message: The Sender value is not a valid EMAIL");

            if (string.IsNullOrEmpty(this.senderName.Trim()))
                throw new Exception("Model::Message: The SenderName value can not be Null or Empty for an EMAIL");

            if (!Tools.Verif.Email(this.receiver))
                throw new Exception("Model::Message: The Receiver value is not valid for an EMAIL");

            if (string.IsNullOrEmpty(this.subject.Trim()))
                throw new Exception("Model::Message: The Subject value can not be Null or Empty for an EMAIL");
      
            if (string.IsNullOrEmpty(this.content.Trim()))
                throw new Exception("Model::Message: The Message Content value can not be Null or Empty for an EMAIL");

            return true;
        }
        
        /// <summary>
        /// This function checks if the current message is a valid SMS and raises an exception otherwise
        /// </summary>
        /// <returns>True if valid, otherwise it raises an exception</returns>
        public bool IsValidSms()
        {
            if (this.MessageType.ToUpper() != "SMS")
                throw new Exception("Model::Message: The Message Type is not a SMS");

            if (string.IsNullOrEmpty(this.sender.Trim())) //Can be a number or a String
                throw new Exception("Model::Message: The Sender value can not be Null or Empty for a SMS");

            if (string.IsNullOrEmpty(this.receiver.Trim()) || !Tools.Verif.Numeric(this.receiver, System.Globalization.NumberStyles.Integer))
                throw new Exception("Model::Message: The Receiver value is not correct for a SMS");

            if (string.IsNullOrEmpty(this.content.Trim()))
                throw new Exception("Model::Message: The Message Content value can not be Null or Empty for a SMS");

            return true;
        }
        #endregion

        #region Update Message Status
        /// <summary>
        /// Set the Message to "sending"
        /// </summary>
        /// <param name="provider">The provider name which will send the message</param>
        internal void Sending(string provider)
        { this.update("sending", provider); }

        /// <summary>
        /// Set the Message to "notSent"
        /// </summary>
        /// <param name="reason">The message provider "sending failed" full response</param>
        public void NotSent(string reason)
        { this.update("notsent", null, reason,"",true); } // provider name is set when the message goes to sending

        /// <summary>
        /// Set the Message to "sent"
        /// </summary>
        /// <param name="reason">The message provider "successfuly sent" full response</param>
        /// <param name="idMessageProvider">The message provider "successfuly sent" parsed response</param>
        internal void Sent(string reason, string idMessageProvider)
        { this.update("sent", null, reason, idMessageProvider, true); } // provider name is set when the message goes to sending

        /// <summary>
        /// Private function used to update the message status and related data
        /// </summary>
        /// <param name="status">One of the available status: sending, notsent or sent</param>
        /// <param name="provider">The provider name which will try to send the message</param>
        /// <param name="reason">The message provider "successfuly sent/failed" full response</param>
        /// <param name="idMessageProvider">The message provider "successfuly sent" parsed response</param>
        /// <param name="updateSentTime">Whether it should update (or not) the message Sent Time</param>
        private void update(string status, string provider = "", string reason = "", string idMessageProvider = "", bool updateSentTime = false)
        {
          if (this._statuses.Contains(status))
          {
            try
            {
              this._message.Status = status;

              if (!string.IsNullOrEmpty(provider))
                this._message.Provider = provider;

              if (!string.IsNullOrEmpty(reason))
                this._message.Reason = reason;

              if (!string.IsNullOrEmpty(idMessageProvider))
                this._message.IdMessageProvider = idMessageProvider;

              if (updateSentTime)
                this._message.SentTime = DateTime.Now;

              _db.Blast.SubmitChanges();
              //Tools.EventLogger.WriteEntry("DEBUG::Message: end of status submitChange: " + this._message.Id + ": " + status + " == " + this._message.Status, EventLogEntryType.Warning);
            }
            catch (ChangeConflictException cce)
            { //if a previous SubmitChanges() failed (because the Message has been updated by someone else),
              //an exception will be raised
              //and it will create a conflict inside the object _db.Blast
              //This code resolve the conflict. If we do not do that, the exception will still be raised on the next Blast.SubmitChanges()
              _db.Blast.ChangeConflicts.ResolveAll(RefreshMode.OverwriteCurrentValues); //with OverwriteCurrentValues, we will replace datas with the ones from the databases.
              throw new Exception("Model::Message: Message " + this._message.Id + " status not updated to " + status +"\n"+ cce.Message);
            }
          }
        }
        #endregion

        /// <summary>
        /// Returns a string where all ##keywords## from the given container value, are replaced with their equivalent from the Message
        /// following the given urlEncoding rules
        /// </summary>
        /// <param name="container">the string containing (or not) some ##keywords##</param>
        /// <param name="urlEncoding">the urlEncoding to be used</param>
        /// <returns>a string where all ##keywords## are replaced with their equivalent from the Message</returns>
        internal string ToRequestUri(string container, string urlEncoding)
        {
            StringBuilder sb = new StringBuilder(container);
            sb.Replace("##receiver##", Tools.UrlParameters.Encode(this.receiver, urlEncoding));
            sb.Replace("##receivername##", Tools.UrlParameters.Encode(this.receiverName, urlEncoding));
            sb.Replace("##sender##", Tools.UrlParameters.Encode(this.sender, urlEncoding));
            sb.Replace("##sendername##", Tools.UrlParameters.Encode(this.senderName, urlEncoding));
            sb.Replace("##subject##", Tools.UrlParameters.Encode(this.subject, urlEncoding));
            sb.Replace("##message##", Tools.UrlParameters.Encode(this.content, urlEncoding));
            return sb.ToString();
        }
     
    }
}
