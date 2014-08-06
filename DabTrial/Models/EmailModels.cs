using DabTrial.Domain.Services;
using Postal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Web;

namespace DabTrial.Models
{
    public abstract class EmailModel : Email
    {
        string _from;
        public string To { get; set; }
        public virtual string From 
        {
            get 
            {
                if (_from==null)
                {
                    var section = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
                    if (section != null) { _from = section.From; }
                }
                return _from;
            }
            set 
            { 
                _from = value; 
            }
        }
    }

    public abstract class GroupEmailModel : EmailModel
    {
        public int StudyCentreId { get; set; }
    }
    public enum PasswordPresentations { PlainText, None, Obfuscated }
    public class UserPasswordEmailModel : EmailModel
    {
	    public string UserName {get; set;}
        public string PlainTextPassword { get; set; }
        public PasswordPresentations PasswordDisplay { get; set; }
    }

    public class NewParticipantEmailModel : GroupEmailModel
    {
	    public string UserName {get; set;}
	    public DateTime DateTimeRandomised {get;set;}
	    public int ParticipantId {get;set;}
    }

    public class EventLoggedEmailModel : GroupEmailModel
    {
	    public string EventType {get; set;}
	    public string UserName {get;set;}
	    public string StudyCentreName {get; set;}
	    public DateTime DateTimeLogged {get; set;}
	    public int ParcipantId {get; set;}
	    public DateTime EventDateTime {get; set;}
	    public string Details {get; set;}
    }

    public class DataUpdateEmailModel : EmailModel
    {
        public int DaysBeforeNotifying { get; set; }
        public IEnumerable<Participant4Update> Participants4Update { get; set; }
    }
    public class Participant4Update
    {
        public int ParticipantId { get; set; }
        public int DaysSinceEnrolled { get; set; }
        public string DataStage { get; set; }
    }
}