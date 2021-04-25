using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlertmanagerSmsNotifier.Models.Configs
{
    public class ArmaghanConfigs
    {
        public const string Position = "Providers:Armaghan";


        public string Originator { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
