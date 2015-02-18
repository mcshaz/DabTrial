using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DabTrial.Domain.Services;
using Moq;
using Postal;
using System.Diagnostics;
using System.Net.Mail;
using System.Linq;
using System.Web.Mvc;

namespace DabTrialTests
{
    [TestClass]
    public class EmailTest
    {
        CreateEmailService _emailService;
        Mock<IEmailService> _postalMock;
        Email _email;
        EmailService _mailService;
        MailMessage _message;
        const string HardPath = @"C:\Users\OEM\Documents\Visual Studio 2013\Projects\DabTrial2013\DabTrial\Views\Emails";
        EmailService MailService
        {
            get
            {
                if (_mailService==null)
                {
                    var engines = new ViewEngineCollection();
                    engines.Add(new FileSystemRazorViewEngine(HardPath));
                    _mailService = new EmailService(engines);
                }
                return _mailService;
            }
        }
        MailMessage Message
        {
            get
            {
                if (_message==null)
                {
                    Assert.IsNotNull(_email, "Neither Send nor CreateMailService methods were called");
                    _message = MailService.CreateMailMessage(_email);
                }
                return _message;
            }
        }
        [TestInitialize]
        public void Setup()
        {
            _postalMock = new Mock<IEmailService>();
            _emailService = new CreateEmailService(_postalMock.Object);
            _emailService.ViewsPath = HardPath;
            _postalMock.Setup(x => x.Send(It.IsAny<Email>()))
                .Callback<Email>(e => _email = e);
            _postalMock.Setup(x => x.CreateMailMessage(It.IsAny<Email>()))
                .Callback<Email>(e => { _email = e; _message = null; })
                .Returns(()=> Message);
        }
        

        [TestCleanup] 
        public void TearDown()
        {
            _emailService = null;
            _postalMock = null;
            _email = null;
            _message = null;
        }
        [TestMethod]
        public void TestForwardMail()
        {
            string from = "testEnquirer@test.com";
            string to = "testInvestigator@adhb.govt.nz";
            string subj = "testing 1,2,3";
            string body = "This is purely a test \r\n how are you?";
            _emailService.ForwardWebMessage(from, to, subj, body);
            Assert.AreEqual(Message.From.Address, from);
            Assert.AreEqual(Message.To.Count, 1);
            Assert.AreEqual(Message.To[0].Address, to);
            Assert.IsTrue(Message.Subject.Contains(subj),"Email does not contain subject");
            Assert.IsTrue(body.Split(new string[] { "\r\n" }, StringSplitOptions.None).All(m=>Message.Body.Contains(m)), "Email does not contain relevant components of message  body");
            Assert.IsFalse(Message.Body.Contains("&lt;"));
        }
        [TestMethod]
        public void TestWelcomeNewUser()
        {
            _emailService.WelcomeNewUser("TestUser","TestUser@test.com","TestPassword", DabTrial.Models.PasswordPresentations.Obfuscated);
        }
        [TestMethod]
        public void TestMissingDataEmail()
        {
            _emailService.EmailInvestigatorsReMissingData();
            HtmlToTextTests.DisplayText(Message.Body);
        }
        [TestMethod]
        public void TestDeathEmail()
        {
            _emailService.NotifyParticipantDeath(62);
        }
    }
}
