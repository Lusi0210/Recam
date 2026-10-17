using System;

namespace Remp.Service.Interfaces;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string body);
}
