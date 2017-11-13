using System;

namespace Pragmasoft.QuickpayV10.Extensions.Services.AppCenter
{
    /// <summary>
    /// A simplied version of the api response.
    /// </summary>
    public struct AppVerificationResult
    {
        public bool Valid { get; set; }
        public string Message { get; set; }

        public AppVerificationResult(bool valid, string message):this()
        {
            Valid = valid;
            Message = message ?? String.Empty;
        }
    }
}
