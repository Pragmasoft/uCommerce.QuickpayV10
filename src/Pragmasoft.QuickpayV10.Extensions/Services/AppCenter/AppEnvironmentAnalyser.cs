using System;
using System.IO;
using System.Reflection;

namespace Pragmasoft.QuickpayV10.Extensions.Services.AppCenter
{
    public class AppEnvironmentAnalyser : IAppEnvironmentAnalyser
    {
        public AppEnvironmentAnalyser()
        {
            
        }

        public Cms Cms()
        {
            //we could use a more solid implementation here
            if(Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"/umbraco"))
                return AppCenter.Cms.Umbraco;
            return AppCenter.Cms.SiteCore;
        }

        public string UCommerceVersion()
        {
            var binPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
            var uCommerceDllPath = string.Format("{0}\\uCommerce.dll", binPath);
            if (File.Exists(uCommerceDllPath))
                return AssemblyName.GetAssemblyName(uCommerceDllPath).Version.ToString();
            return null;
        }

        public string AppVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Name + ", " +
                   Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

    }
}
