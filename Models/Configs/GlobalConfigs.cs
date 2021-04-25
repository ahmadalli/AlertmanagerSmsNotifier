using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertmanagerSmsNotifier.Models.Configs
{
    public class GlobalConfigs
    {
        public SmsProvider SmsProvider { get; set; }
        public List<string> DefaultRecipients { get; set; }
        public Dictionary<string, List<string>> RecipientGroups { get; set; }
    }

    public enum SmsProvider
    {
        SmsIr,
        Armaghan
    }
}
