using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;

namespace TMailAgentWinService.Utility
{
    public static class InstallerExtension
    {
        public static void WriteInstallProperties(this TransactedInstaller tInstaller, string msg) 
        {
            if (tInstaller != null) 
            {
                tInstaller.Context.LogMessage(msg);
            }
        }

        public static void WriteInstallerCommandOption(this TransactedInstaller tInstaller) 
        {
            if (tInstaller != null) 
            {
                foreach(string key in tInstaller.Context.Parameters.Keys){
                        tInstaller.Context.LogMessage(string.Concat("Installer has command line options: [",
                                                   key,
                                                   "] values [",
                                                   tInstaller.Context.Parameters[key],"]"));
                 }
            }
        }

        public static void WriteInstallerContextProperties(this TransactedInstaller tInstaller, 
                                                           IDictionary savedState)
        {
            foreach (string key in savedState)
            {
                tInstaller.Context.LogMessage(string.Concat("Installer has properties options: [",
                                       key,
                                       "] values [",
                                       savedState[key], "]"));
            }
        }
    }
}
