//-----------------------------------------------------------------------
// <copyright file="MailSenderHandler.cs" company="Martin Brenn">
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
    using System.Collections.Generic;
    using System.Net.Mail;
    using BurnSystems.Extensions;
using BurnSystems.Logging;
    using BurnSystems.Test;
    using BurnSystems.ObjectActivation;
    using System;
    using System.Net;

    /// <summary>
    /// This handler sends the mail to an smtpserver
    /// </summary>
    public class MailSenderHandler : IMailSender
    {
        private ClassLogger logger = new ClassLogger(typeof(MailSenderConfig));

        /// <summary>
        /// Stores the config
        /// </summary>
        private MailSenderConfig config;

        [Inject]
        public MailSenderHandler(MailSenderConfig config)
        {
            Ensure.IsNotNull(config);
            this.config = config;
        }

        /// <summary>
        /// Sends a mail to the recipient
        /// </summary>
        /// <param name="recipient">Name of the recipient, who shall
        /// receive the message. This is most often a playername</param>
        /// <param name="subject">Subject of the message</param>
        /// <param name="message">Message to be send</param>
        public void SendMail(string recipient, string subject, string message)
        {
            var mailMessage = new MailMessage(
                this.config.Sender, 
                recipient);
            mailMessage.Body = message;
            mailMessage.Subject = subject;
            this.SendMail(mailMessage);
        }
        
        /// <summary>
        /// Sends a mail
        /// </summary>
        /// <param name="mail">Mail to be sent</param>
        public void SendMail(System.Net.Mail.MailMessage mail)
        {
            if (!this.config.IsActive)
            {
                // Not Active
                var message = string.Format(
                    Localization_WebServer.SkippingMail,
                    mail.To.ToString());
                logger.LogEntry(new LogEntry(message, LogLevel.Notify));
                return;
            }

            mail.Subject = String.Format("{0}{1}", this.config.SubjectPrefix, mail.Subject);
            mail.Body += this.config.ContentPostfix;
            mail.From = new MailAddress(this.config.Sender);

            var client = new SmtpClient(this.config.SMTPHost);
            client.Credentials =
                new NetworkCredential(this.config.SMTPUser, this.config.SMTPPassword);
            client.Send(mail);

            logger.LogEntry(LogEntry.Format(LogLevel.Message, Localization_WebServer.MailSent, mail.Subject, mail.To.ToString()));
        }
    }
}
