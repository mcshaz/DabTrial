using System;
using System.Collections.Generic;
using System.Web;
using System.Net.Mail;
using System.IO;
using System.Text;
using DabTrial.Utilities;

namespace DabTrial.Utilities
{
    public static class Email
    {
        public const string TrialEmail = "dabtrial@dabtrial.com";
        public static Dictionary<string, string> EventDetails(object eventType,
                                                              object userName,
                                                              object studyCentre,
                                                              object loggedTime,
                                                              object participantId,
                                                              object eventTime,
                                                              object eventDetails)
        {
            var returnVal = new Dictionary<string, string>();
            returnVal.Add("eventType", eventType.ToString());
            returnVal.Add("userName", userName.ToString());
            returnVal.Add("studyCentre", studyCentre.ToString());
            returnVal.Add("loggedTime", eventTime.ToString());
            returnVal.Add("participantId", participantId.ToString());
            returnVal.Add("eventTime", eventTime.ToString());
            returnVal.Add("eventDetails", eventDetails.ToString());
            return returnVal;
        }
        public static Dictionary<string, string> EnrollDetails(object enrollingClinician,
                                                              object timeEnrolled,
                                                              object participantId)
        {
            var returnVal = new Dictionary<string, string>();
            returnVal.Add("userName", enrollingClinician.ToString());
            returnVal.Add("timeRandomised", timeEnrolled.ToString());
            returnVal.Add("participantId", participantId.ToString());
            return returnVal;
        }
        public static void Send(string toAddress, string subject, string fileName, IDictionary<string,string> replacements, string replyAddress=null)
        {
            List<string> keys = new List<string>(replacements.Keys);
            foreach (string key in keys )
            {
                string replace = replacements[key];
                if (replace[0] == '~')
                {
                    replacements[key] = UriStringTools.GetAbsoluteUri() + replace.Substring(1, replace.Length - 2);
                }
            }
            string body = ReadEmailFile(fileName, replacements);

            Send(toAddress, subject, body, replyAddress);
        }
        public static void Send(string toAddress, string subject, string body, string replyAddress)
        {
            var message = new MailMessage()
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            if (replyAddress != null) { message.ReplyToList.Add(replyAddress); }
#if !DEBUG
            if (System.Configuration.ConfigurationManager.AppSettings["EnvironmentName"] != "Production")
            {
                //avoid peppering email box with dummy data
                if(!string.IsNullOrEmpty(replyAddress)) 
                {
                    toAddress = replyAddress; 
                }
                else if (toAddress.IndexOf(',')!=-1)
                {
                    return;
                }
            }
#endif
            message.To.Add(toAddress);
            var mailClient = new SmtpClient();
            mailClient.Send(message);
        }
        private const string EmailPath = "~/EmailTemplates/";
        private static string ReadEmailFile(string fileName, IDictionary<string, string> replacements)
        {
            
            string physicalPath = HttpContext.Current.Server.MapPath(EmailPath + fileName);
            if (physicalPath == null) throw new FileNotFoundException(fileName);
            FileStream fs = new FileStream(physicalPath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            StringBuilder sb = new StringBuilder((int)(fs.Length*1.3));
            string line;
            while ( ( line = sr.ReadLine() ) != null ) {
                if (String.IsNullOrWhiteSpace(line))
                {
                    sb.Append("<br/>"); //Environment.NewLine
                }
                else
                {
                    int replaceStart = 0;
                    while ((replaceStart = line.IndexOf("<%", replaceStart)) > -1)
                    {
                        int replaceEnd = line.IndexOf("%>", replaceStart) + 2;
                        int replaceLength = replaceEnd - replaceStart -4;
                        string replaceKey = line.Substring(replaceStart + 2, replaceLength).Trim();
                        line = line.Substring(0, replaceStart) + replacements[replaceKey] + line.Substring(replaceEnd);
                    }
                    sb.Append("<p>" + line + "</p>");
                }
            }
            sr.Close();
            return sb.ToString();
        }
    }
}