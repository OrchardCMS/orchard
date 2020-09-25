﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Orchard.ContentManagement;
using Orchard.Email.Models;
using Orchard.Email.Services;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.UI.Admin;

namespace Orchard.Email.Controllers {
    [Admin]
    public class EmailAdminController : Controller {
        private readonly ISmtpChannel smtpChannel;
        private readonly IOrchardServices orchardServices;

        public EmailAdminController(ISmtpChannel smtpChannel, IOrchardServices orchardServices) {
            this.smtpChannel = smtpChannel;
            this.orchardServices = orchardServices;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult TestSettings(TestSmtpSettings testSettings) {
            ILogger logger = null;
            try {
                var fakeLogger = new FakeLogger();
                if (smtpChannel is Component smtpChannelComponent) {
                    logger = smtpChannelComponent.Logger;
                    smtpChannelComponent.Logger = fakeLogger;
                }

                // Temporarily update settings so that the test will actually use the specified host, port, etc.
                var smtpSettings = orchardServices.WorkContext.CurrentSite.As<SmtpSettingsPart>();

                smtpSettings.FromAddress = testSettings.FromAddress;
                smtpSettings.FromName = testSettings.FromName;
                smtpSettings.ReplyTo = testSettings.ReplyTo;
                smtpSettings.Host = testSettings.Host;
                smtpSettings.Port = testSettings.Port;
                smtpSettings.EnableSsl = testSettings.EnableSsl;
                smtpSettings.RequireCredentials = testSettings.RequireCredentials;
                smtpSettings.UseDefaultCredentials = testSettings.UseDefaultCredentials;
                smtpSettings.UserName = testSettings.UserName;
                smtpSettings.Password = testSettings.Password;
                smtpSettings.ListUnsubscribe = testSettings.ListUnsubscribe;

                if (!smtpSettings.IsValid()) {
                    fakeLogger.Error("Invalid settings.");
                }
                else {
                    smtpChannel.Process(new Dictionary<string, object> {
                        {"Recipients", testSettings.To},
                        {"Subject", T("Orchard CMS - SMTP settings test email").Text}
                    });
                }

                if (!string.IsNullOrEmpty(fakeLogger.Message)) {
                    return Json(new { error = fakeLogger.Message });
                }

                return Json(new { status = T("Message sent.").Text });
            }
            catch (Exception e) {
                return Json(new { error = e.Message });
            }
            finally {
                if (smtpChannel is Component smtpChannelComponent) {
                    smtpChannelComponent.Logger = logger;
                }

                // Undo the temporarily changed SMTP settings.
                orchardServices.TransactionManager.Cancel();
            }
        }

        private class FakeLogger : ILogger {
            public string Message { get; set; }

            public bool IsEnabled(LogLevel level) => true;

            public void Log(LogLevel level, Exception exception, string format, params object[] args) =>
                Message = exception == null ? format : exception.Message;
        }

        public class TestSmtpSettings {
            public string FromAddress { get; set; }
            public string FromName { get; set; }
            public string ReplyTo { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
            public bool EnableSsl { get; set; }
            public bool RequireCredentials { get; set; }
            public bool UseDefaultCredentials { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public string To { get; set; }
            public string ListUnsubscribe { get; set; }
        }
    }
}