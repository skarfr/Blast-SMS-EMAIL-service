using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Model.database;

namespace Model
{
    public class Provider
    {
        #region private properties
        /// <summary>
        /// Row from the table Blast.Providers
        /// </summary>
        private readonly Providers _provider;

        /// <summary>
        /// Database instance
        /// </summary>
        private readonly cDatabase _db = new cDatabase();

        /// <summary>
        /// Available list of providers statuses.
        /// </summary>
        private readonly List<ProvidersStatus> _listStatus;
        #endregion

        #region Public Accessors
        /// <summary>
        /// Accessor to the MessageType
        /// </summary>
        public string MessageType
        {
            get { return this._provider.MessageType; }
        }
        #endregion

        #region private Accessors
        /// <summary>
        /// Accessor to the Name/Provider property
        /// </summary>
        private string name
        {
            get { return this._provider.Provider; }
        }

        /// <summary>
        /// Accessor to the url
        /// </summary>
        private string url
        {
            get { return this._provider.Url; }
        }

        /// <summary>
        /// Accessor to the parameters property
        /// </summary>
        private string parameters
        {
            get { return this._provider.Parameters; }
        }
        
        /// <summary>
        /// Accessor to the request encoding
        /// </summary>
        private string requestEncoding
        {
            get { return this._provider.RequestEncoding; }
        }

        /// <summary>
        /// Accessor to the request MIME type
        /// </summary>
        private string requestMIMEType
        {
            get { return this._provider.RequestMIMEType; }
        }

        /// <summary>
        /// Accessor to the regex success
        /// </summary>
        private string regexSuccess
        {
        get { return this._provider.RegexSuccess; }
        }
        #endregion

        /// <summary>
        /// Provider contructor
        /// </summary>
        /// <param name="provider">Provider row from database.Providers</param>
        public Provider(Providers provider)
        {
            if (provider == null)
                throw new Exception("Model::Provider: The Provider can not be Null");
            this._provider = provider;

            var result = from s in _db.Blast.ProvidersStatus
                         where s.Provider == this.name
                         select s;

            if (result.Any())
                this._listStatus = result.ToList();
        }

        /// <summary>
        /// Send the given message (HttpWebRequest) using the current provider
        /// </summary>
        /// <param name="message"></param>
        public void Send(Message message)
        {
            if (message.IsValidSms() || message.IsValidEmailing())  // if everything is correct, we are ready to send !
            {
                message.Sending(this.name); // update the message status to "Sending"

                #region Create the url, handle optional Params & send the Request
                //parameters can whether be in the URL field (and will be passed when we will call that url with parameters)
                //or parameters can be in the parameters field, and be send to the url through the body of a post request (streamwriter)
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(message.ToRequestUri(this.url, this.requestEncoding));

                if (!string.IsNullOrEmpty(this.parameters)) //if we have parameters, we try to send them with a streamWriter
                {
                    string body = message.ToRequestUri(this.parameters, this.requestEncoding);
                    request.Method = "POST";
                    request.ContentType = this.requestMIMEType;
                    request.ContentLength = body.Length;

                    StreamWriter writer = new StreamWriter(request.GetRequestStream());
                    writer.Write(body);
                    writer.Close();
                }
                #endregion

                #region Get the Request Response, check it & update the Message Status accordingly
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    StreamReader reader = new StreamReader(stream);//, System.Text.Encoding.GetEncoding("utf-8"));
                    string answer = reader.ReadToEnd();
                    reader.Close();
                    response.Close();

                    Match match = new Regex(this.regexSuccess).Match(answer); //we try to extract the ID from the response
                    if (match.Success) {
                        if (match.Groups[1] != null)
                            message.Sent(answer, match.Groups[1].Value);
                        else
                            message.Sent(answer, String.Empty);              //may happen if the regex doesn't contain any group (aka (...) )
                    }
                    else
                        message.NotSent(this.getProviderReason(answer, "send") + ": " + answer); //if you want a log entry, just throw a new exception
                }
                #endregion
            }
            else { throw new Exception("Model::Provider: The MessageType value is not recognized"); }
        }

        /// <summary>
        /// Based on the given answer resulting from the request and the given action name, 
        /// this function will query the table ProvidersStatus and look for the "reading friendly" reason
        /// OR return a String.Empty
        /// </summary>
        /// <param name="answer">the request answer</param>
        /// <param name="action">the action sent</param>
        /// <returns></returns>
        private string getProviderReason(string answer, string action)
        {
            ProvidersStatus status = this._listStatus.FirstOrDefault(s => s.Action == action && s.Id == answer);
            return status == null ? String.Empty : status.Description;
        }
    }
}
