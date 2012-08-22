//-----------------------------------------------------------------------
// <copyright file="DummyMailSenderHandler.cs" company="Martin Brenn">
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

    /// <summary>
    /// This class is used to ignore all requests to send a mail. 
    /// The mail is stored in the logicstate
    /// </summary>
    public class DummyMailSenderHandler : IMailSender
    {
        /// <summary>
        /// Stores the list of sent mails
        /// </summary>
        public static List<MailMessage> SentMails = new List<MailMessage>();

        /// <summary>
        /// Sends a mail 
        /// </summary>
        /// <param name="recipient">Recipient of the mail</param>
        /// <param name="subject">Subject of th email</param>
        /// <param name="message">Message of the email</param>
        public void SendMail(string recipient, string subject, string message)
        {
            var mail = new MailMessage("fbk@depon.net", recipient);
            mail.Body = message;
            mail.Subject = subject;
            this.SendMail(mail);
        }

        /// <summary>
        /// Sends a mail
        /// </summary>
        /// <param name="mail">Mail to be sent</param>
        public void SendMail(System.Net.Mail.MailMessage mail)
        {
            SentMails.Add(mail);
        }
    }
}
