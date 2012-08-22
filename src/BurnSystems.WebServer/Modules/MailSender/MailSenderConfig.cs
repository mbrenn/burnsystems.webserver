//-----------------------------------------------------------------------
// <copyright file="MailSenderCore.cs" company="Martin Brenn">
//     Alle Rechte vorbehalten. 
// 
//     Die Inhalte dieser Datei sind ebenfalls automatisch unter 
//     der AGPL lizenziert. 
//     http://www.fsf.org/licensing/licenses/agpl-3.0.html
//     Weitere Informationen: http://de.wikipedia.org/wiki/AGPL
// </copyright>
//-----------------------------------------------------------------------

namespace BurnSystems.WebServer.Modules.MailSender
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Xml.Linq;
    using BurnSystems.Logging;
    using BurnSystems.Test;

    /// <summary>
    /// Dies ist die zentrale Klasse, über die der Mailversand organisiert
    /// wird. 
    /// </summary>
    public class MailSenderConfig
    {
        /// <summary>
        /// Stores the host of the smtpserver
        /// </summary>
        private string smtpHost = "localhost";

        /// <summary>
        /// Stores the user of the smtpserver
        /// </summary>
        private string smtpUser = string.Empty;

        /// <summary>
        /// Stores the password of the smtpserver
        /// </summary>
        private string smtpPassword = string.Empty;

        /// <summary>
        /// Stores the prefix of the subject
        /// </summary>
        private string subjectPrefix = string.Empty;

        /// <summary>
        /// Stores the postfix of the content
        /// </summary>
        private string contentPostfix = string.Empty;

        /// <summary>
        /// Stores the sender of the mail
        /// </summary>
        private string sender = "BurnSystems Webserver <server@depon.net>";

        /// <summary>
        /// Initializes a new instance of the MailSenderCore class.
        /// </summary>
        public MailSenderConfig()
        {
            this.IsActive = true;
        }

        /// <summary>
        /// Gets or sets the sender of the mails
        /// </summary>
        public string Sender
        {
            get { return this.sender; }
            set { this.sender = value; }
        }

        /// <summary>
        /// Gets or sets the host of the smtp-server
        /// </summary>
        public string SMTPHost
        {
            get { return this.smtpHost; }
            set { this.smtpHost = value; }
        }

        /// <summary>
        /// Gets or sets the user of the smtp-server
        /// </summary>
        public string SMTPUser
        {
            get { return this.smtpUser; }
            set { this.smtpUser = value; }
        }

        /// <summary>
        /// Gets or sets the password of the smtp-server
        /// </summary>
        public string SMTPPassword
        {
            get { return this.smtpPassword; }
            set { this.smtpPassword = value; }
        }

        /// <summary>
        /// Gets or sets the prefix of the mail subject
        /// </summary>
        public string SubjectPrefix
        {
            get { return this.subjectPrefix; }
            set { this.subjectPrefix = value; }
        }

        /// <summary>
        /// Gets or sets the postfix of the content
        /// </summary>
        public string ContentPostfix
        {
            get { return this.contentPostfix; }
            set { this.contentPostfix = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the mailsender is active
        /// </summary>
        public bool IsActive
        {
            get;
            set;
        }
    }
}
