using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task01_Module01.Models
{
    public interface IEmailSender
    {
        void SendEmail(Message message);
        Task SendEmailAsync(Message message);
    }
}
