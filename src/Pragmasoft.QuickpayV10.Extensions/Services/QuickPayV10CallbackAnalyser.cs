using System;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Pragmasoft.QuickpayV10.Extensions.Models.Callback;
using Pragmasoft.QuickpayV10.Extensions.Services.Interfaces;

namespace Pragmasoft.QuickpayV10.Extensions.Services
{
    public class QuickPayV10CallbackAnalyser : IQuickPayV10CallbackAnalyser
    {
        public QuickPayV10CallbackAnalyser()
        {
        }


        public bool IsTestMode(QuickpayApiResponseDto quickPayApiResponseDto)
        {
            if(quickPayApiResponseDto == null) throw new ArgumentNullException("quickPayApiResponseDto");

            return quickPayApiResponseDto.test_mode;
        }

        public QuickpayApiResponseDto ReadCallbackBody(HttpContext currentHttpContext)
        {
            currentHttpContext.Request.InputStream.Position = 0;
            // Get quickpay callback body text - See parameters:http://tech.quickpay.net/api/callback/
            var bodyStream = new StreamReader(currentHttpContext.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            // Get body text
            var bodyText = bodyStream.ReadToEnd();
            currentHttpContext.Request.InputStream.Position = 0;
            // Deserialize json body text 
            return JsonConvert.DeserializeObject<QuickpayApiResponseDto>(bodyText);
        }

    }
}
