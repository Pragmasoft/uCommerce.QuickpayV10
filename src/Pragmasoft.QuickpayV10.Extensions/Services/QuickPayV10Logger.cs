using System;
using System.Globalization;
using System.IO;
using System.Text;
using Pragmasoft.QuickpayV10.Extensions.Services.Interfaces;
using UCommerce.Infrastructure.Logging;

namespace Pragmasoft.QuickpayV10.Extensions.Services
{

    public class QuickPayV10Logger : IQuickPayV10Logger
    {
        private readonly ILoggingService _defaultLoggingService;

        private readonly string _pragmasoftLogDirectory;
        private readonly string _quickpayLogFilename;

        public QuickPayV10Logger(ILoggingService defaultLoggingService)
        {
            _defaultLoggingService = defaultLoggingService;
            _pragmasoftLogDirectory = @"\App_Data\Logs\Pragmasoft\";
            _quickpayLogFilename = "QuickpayV10.txt";

            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + _pragmasoftLogDirectory);
        }

        public void Log(string line)
        {
            using (var file = new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + _pragmasoftLogDirectory + _quickpayLogFilename, true, Encoding.UTF8) )
            {
                file.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + ": " + line);
            }
        }

        public void LogException(Exception exception, string message = null)
        {
            if (message != null) message = String.Empty;
            
            var logLine = "Exception: ";
            logLine += message;
            logLine += Environment.NewLine + exception.ToString();
            Log(logLine);
            _defaultLoggingService.Log<QuickPayV10Logger>(exception, "Exception occurred in the QuickPay UCommerce app: " + message);
        }

    }
}
