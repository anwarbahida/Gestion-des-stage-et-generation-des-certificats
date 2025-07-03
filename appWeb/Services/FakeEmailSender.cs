using Microsoft.AspNetCore.Identity.UI.Services; // ✅ très important
using System.Threading.Tasks;
using System;

namespace appWeb.Services
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Console.WriteLine($"Fake email envoyé à {email} avec sujet '{subject}'");
            return Task.CompletedTask;
        }
    }
}
