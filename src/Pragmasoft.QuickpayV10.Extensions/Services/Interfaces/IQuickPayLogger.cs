using System;

namespace Pragmasoft.QuickpayV10.Extensions.Services.Interfaces
{
    public interface IQuickPayV10Logger
    {
        void Log(string line);
        void LogException(Exception exception, string messsage = null);
    }
}
