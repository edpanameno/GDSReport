using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mail;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Collections;
using GDSReport.Models;

namespace GDSReport.Infrastructure
{
    /// <summary>
    /// Used to send out reports to the desired email recipients.
    /// </summary>
    public class Notifications
    {
        public MailMessage Message { get; set; }
        public SmtpClient SmtpServer { get; set; }
        public string MailRecipients { get; set; }
        public string ReplyEmail { get; set; }
        public MailAddressCollection Recipients { get; set; }

        public Notifications()
        {
            SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["mail-server"].ToString());
            ReplyEmail = ConfigurationManager.AppSettings["reply-email"].ToString();
            Recipients = ParseEmailRecipients();
        }

        /// <summary>
        /// Sends out the monthly GDS report to the desired recipients. This reports shows a summary
        /// of the user count by department. In addition, an excel file with the list of all users 
        /// is also attached.
        /// </summary>
        /// <param name="attachment"></param>
        public void SendMonthlyReport(IEnumerable<CompanySummary> companySummary, MemoryStream attachment = null)
        {
            Message = new MailMessage();
            Message.From = new MailAddress(ReplyEmail);
            Message.Subject = "GDS Report for " + DateTime.Now.ToString("MM-dd-yyyy");

            foreach(var email in Recipients)
            {
                Message.To.Add(email);
            }

            attachment.Position = 0;
            Attachment excelFile = new Attachment(attachment, "GDS Report-" + DateTime.Now.ToString("MM-dd-yyyy") + ".xlsx", "application/vnd.ms-excel");
            Message.Attachments.Add(excelFile);

            Message.IsBodyHtml = true;
            StringBuilder msg = new StringBuilder();

            msg.AppendLine("<p>See attached for the GDS Report generated on " + DateTime.Now.ToString("MMMM dd, yyyy h:mm tt") + "</p>");
            msg.AppendLine("<table><tr><th style='text-align: left;'>Company</th><th>User Count</th></tr>");
            
            foreach(var company in companySummary)
            {
                msg.AppendLine("<tr><td>" + company.CompanyName + "</td><td style='text-align: right'>" + company.UserCount.ToString() + "</td></tr>");
            }

            msg.AppendLine("<tr style='font-weight: bold;'><td>Total</td><td style='text-align: right;'>" + companySummary.Sum(c => c.UserCount).ToString() + "</td></tr>");
            msg.AppendLine("</table>");

            Message.Body = msg.ToString();

            try
            {
                SmtpServer.Send(Message);
            }
            catch(Exception e)
            {
            }
            finally
            {
                SmtpServer.Dispose();
                excelFile.Dispose();
            }
        }

        /// <summary>
        /// This method will be used to parse and generate the list of email 
        /// addresses that the notification will be sent to. The list of 
        /// email addresses comes from the app.config file.
        /// </summary>
        /// <returns></returns>
        private MailAddressCollection ParseEmailRecipients()
        {
            MailAddressCollection emailRecipeints = new MailAddressCollection();
            List<string> recipients = ConfigurationManager.AppSettings["mail-recipients"].ToString().Split(';').ToList();
            
            // Used to validate email address to make sure no invalid email addresses
            // are added to the MailAddressCollection. This will prevent Execeptions to
            // be thrown when sending out the report.
            Regex emailRegEx = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");

            foreach(var recipient in recipients)
            {
                if(emailRegEx.IsMatch(recipient))
                {
                    emailRecipeints.Add(recipient);
                }
            }

            return emailRecipeints;
        }
    }
}
