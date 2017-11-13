using System;
using Pragmasoft.QuickpayV10.Extensions.Services.Interfaces;

namespace Pragmasoft.QuickpayV10.Extensions.Services.AppCenter
{

    public class QuickPayV10AppVerifier : IAppVerifier
    {
        private readonly IQuickPayV10Logger _logger;

        public QuickPayV10AppVerifier(IQuickPayV10Logger logger)
        {
            _logger = logger;
        }

        public AppVerificationResult VerifyAppSafely(PragmasoftAppCenterService.VerificationResponse verification)
        {
            if (verification == null) return new AppVerificationResult(false, String.Empty);
            return new AppVerificationResult(!verification.Discontinued && verification.LicenseAccepted && !verification.UpdateRequired, verification.Message);
        }

        public void VerifyApp(PragmasoftAppCenterService.VerificationResponse verificationResponse)
        {
            if(verificationResponse == null) {
                _logger.Log("No Pragmasoft license verification was performed. ");
                return; //we can't really shut down people's payment providers if the app center is down
            }
            if (verificationResponse.Discontinued) //pray this will never be necessary
            {
                _logger.Log("The QuickpayV10 app has been discontinued. " + verificationResponse.Message);
                throw new NotSupportedException("This app has been discontinued: " + verificationResponse.Message);
            }
            if (verificationResponse.UpdateRequired)
            {
                _logger.Log("The QuickpayV10 app has a critical update. Please update the app: " + verificationResponse.Message);
            }

            if (!verificationResponse.LicenseAccepted)
            {
                _logger.Log("The license for the QuickpayV10 app was not accepted: " + verificationResponse.Message);
            }
        }
    }
}
