//-----------------------------------------------------------------------
// <copyright file="IMailSender.cs" company="Martin Brenn">
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
    using System.Net.Mail;

    /// <summary>
    /// This interface defines the methods to send mails to recipients. 
    /// The mails are sent via SMTP.
    /// </summary>
    public interface IMailSender
    {
        /// <summary>
        /// Sends a mail to the recipient
        /// </summary>
        /// <param name="recipient">Name of the recipient, who shall
        /// receive the message. This is most often a playername</param>
        /// <param name="subject">Subject of the message</param>
        /// <param name="message">Message to be send</param>
        void SendMail(string recipient, string subject, string message);

        /// <summary>
        /// Sends a mail.
        /// </summary>
        /// <param name="mail">Mail to be sent. Recipient, subject
        /// and message has to be set</param>
        void SendMail(MailMessage mail);
    }
}
