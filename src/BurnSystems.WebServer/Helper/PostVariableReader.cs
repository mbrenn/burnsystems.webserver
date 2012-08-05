﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using BurnSystems.ObjectActivation;
using BurnSystems.Logging;
using System.IO;
using BurnSystems.Collections;
using BurnSystems.Net;
using System.Web;

namespace BurnSystems.WebServer.Helper
{
    public class PostVariableReader
    {
        [Inject]
        public HttpListenerContext ListenerContext
        {
            get;
            set;
        }

        [Inject]
        public PostVariableReaderConfig Config
        {
            get;
            set;
        }

        public NiceDictionary<string, string> PostVariables
        {
            get;
            set;
        }

        public PostVariableReader()
        {
            this.PostVariables = new NiceDictionary<string, string>();
        }

        /// <summary>
        /// Liest die POST-Variablen, die Form-Data
        /// </summary>
        private void ReadPostFromFormData()
        {
            // Holt die Boundary
            var contentTypes =
                this.ListenerContext.Request.ContentType.Split(new[] { ';' });
            var boundary = string.Empty;
            foreach (var part in contentTypes)
            {
                var position = part.IndexOf('=');
                if (position == -1)
                {
                    continue;
                }

                var leftPart = part.Substring(0, position).Trim();
                var rightPart = part.Substring(position + 1).Trim();

                if (leftPart.Equals("boundary", StringComparison.InvariantCultureIgnoreCase))
                {
                    boundary = rightPart;
                }
            }

            // Die Grenze
            if (boundary == string.Empty)
            {
                throw new InvalidOperationException(
                    Localization_WebServer.InvalidMultipart);
            }

            // Liest nun den eigentlichen Stream ein
            using (var stream = this.ListenerContext.Request.InputStream)
            {
                var multipartReader = new MultipartFormDataReader(boundary);
                multipartReader.MaxStreamSize = this.Config.MaxPostLength;

                var data = multipartReader.ReadStream(stream);
                foreach (var part in data.Parts)
                {
                    if (string.IsNullOrEmpty(
                        part.ContentDisposition["name"]))
                    {
                        // Kein Name, kein Eintrag
                        continue;
                    }

                    var name = part.ContentDisposition["name"];
                    if (name[0] == '"')
                    {
                        name = name.Substring(1, name.Length - 2);
                    }

                    if (string.IsNullOrEmpty(
                        part.ContentDisposition["filename"]))
                    {
                        // Normale Formvariable
                        var content = UTF8Encoding.UTF8.GetString(part.Content);
                        this.PostVariables[name] =
                            content.Trim();
                    }
                    else
                    {
                        // Datei 
                        throw new InvalidOperationException(Localization_WebServer.FileUploadNotSupported);
                        /*var webFile = new WebFile(
                            part.ContentDisposition["filename"],
                            part.Content);
                        this.files[name] = webFile;*/
                    }
                }
            }
        }

        /// <summary>
        /// Reads the post variables from inputstream
        /// </summary>
        public void ReadPostVariables()
        {
            if (this.ListenerContext.Request.ContentType != null &&
                this.ListenerContext.Request.ContentType.StartsWith("multipart/form-data"))
            {
                this.ReadPostFromFormData();
            }
            else
            {
                this.ReadPostFromWWWUrlEncoded();
            }

            // Removes .x und .y.
            // These variables are set by some browsers, when the user clicks on imagebuttons
            foreach (var pair in new Dictionary<string, string>(this.PostVariables))
            {
                if (pair.Key.EndsWith(".x") || pair.Value.EndsWith(".y"))
                {
                    var leftPart = pair.Key.Substring(0, pair.Key.Length - 2);
                    this.PostVariables[leftPart]
                        = pair.Value;
                }
            }
        }

        /// <summary>
        /// Reads urlencoded variables
        /// </summary>
        private void ReadPostFromWWWUrlEncoded()
        {
            var requestLength = this.ListenerContext.Request.ContentLength64;

            if (requestLength > this.Config.MaxPostLength)
            {
                var logMessage = new LogEntry(
                    Localization_WebServer.MaximumPostLengthExceeded,
                    LogLevel.Fail);
                Log.TheLog.LogEntry(logMessage);
            }

            var postLength = Convert.ToInt32(Math.Min(this.Config.MaxPostLength, requestLength));

            byte[] bytes = new byte[postLength];
            this.ListenerContext.Request.InputStream.Read(bytes, 0, postLength);

            using (var reader = new StreamReader(
                this.ListenerContext.Request.InputStream))
            {
                // Reads maximum size
                var content = this.ListenerContext.Request.ContentEncoding.GetString(
                    bytes);

                foreach (var part in content.Split('&'))
                {
                    int positionEqual = part.IndexOf('=');
                    if (positionEqual == -1)
                    {
                        // Somethin invalid
                        continue;
                    }

                    var leftPart = part.Substring(0, positionEqual);
                    var rightPart = part.Substring(positionEqual + 1);

                    this.PostVariables[HttpUtility.UrlDecode(leftPart)] =
                        HttpUtility.UrlDecode(rightPart);
                }
            }
        }
    }
}