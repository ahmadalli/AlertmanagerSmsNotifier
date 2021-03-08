using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertmanagerSmsNotifier.Services.Interfaces
{
    public interface ISmsSender
    {
        Task SendSms(string text, string[] recipients);
    }
}
