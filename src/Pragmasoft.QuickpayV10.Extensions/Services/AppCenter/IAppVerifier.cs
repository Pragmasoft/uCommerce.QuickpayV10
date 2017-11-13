namespace Pragmasoft.QuickpayV10.Extensions.Services.AppCenter
{
    public interface IAppVerifier
    {
        void VerifyApp(PragmasoftAppCenterService.VerificationResponse verificationResponse);
        AppVerificationResult VerifyAppSafely(PragmasoftAppCenterService.VerificationResponse verification);
    }
}