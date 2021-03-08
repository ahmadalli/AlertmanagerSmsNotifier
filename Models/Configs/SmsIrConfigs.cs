using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertmanagerSmsNotifier.Models.Configs
{
    public class SmsIrConfigs
    {
        public const string Position = "Providers:SmsIr";


        public string LineNumber { get; set; }
        public string UserApiKey { get; set; }
        public string SecretKey { get; set; }
    }
}
