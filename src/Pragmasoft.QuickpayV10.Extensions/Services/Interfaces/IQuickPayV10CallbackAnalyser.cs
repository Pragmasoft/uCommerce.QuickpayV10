using System.Web;
using Pragmasoft.QuickpayV10.Extensions.Models.Callback;

namespace Pragmasoft.QuickpayV10.Extensions.Services.Interfaces
{
    public interface IQuickPayV10CallbackAnalyser
    {
        bool IsTestMode(QuickpayApiResponseDto quickPayApiResponseDto);
        QuickpayApiResponseDto ReadCallbackBody(HttpContext currentHttpContext);
    }
}
