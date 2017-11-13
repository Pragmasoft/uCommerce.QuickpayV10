using System;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Pragmasoft.QuickpayV10.Extensions.Services.AppCenter
{
    public sealed class PragmasoftAppCenterService
    {
        private readonly Guid _appGuid;
        private readonly IAppEnvironmentAnalyser _environmentAnalyser;
        private readonly IAppVerifier _appVerifier;

        public PragmasoftAppCenterService(Guid appGuid, IAppEnvironmentAnalyser environmentAnalyser, IAppVerifier appVerifier)
        {
            _appGuid = appGuid;
            _environmentAnalyser = environmentAnalyser;
            _appVerifier = appVerifier;
        }

        /// <summary>
        /// Verifies the licence key. This method might throw exceptions on failure, depending on the IAppVerifier used.
        /// </summary>
        /// <param name="licenceKey">The Pragmasoft App center license key.</param>
        public void VerifyApp(string licenceKey)
        {
            var verificationResult = RequestAppVerification(licenceKey);
            _appVerifier.VerifyApp(verificationResult);
        }

        /// <summary>
        /// Verifies the licence key and returns a simplifed object with the results.
        /// </summary>
        /// <param name="licenceKey">The Pragmasoft App center license key.</param>
        /// <returns>The result of the verification as a structure.</returns>
        public AppVerificationResult VerifyAppSafely(string licenceKey)
        {
            var verificationResult = RequestAppVerification(licenceKey);
            return _appVerifier.VerifyAppSafely(verificationResult);
        }

        public VerificationResponse RequestAppVerification(string licenceKey)
        {
            VerificationResponse verificationResponse;

            try
            {
                var verificationRequest = new VerificationRequest
                {
                    AppId = _appGuid,
                    LicenseKey = licenceKey,
                    Host = HttpContext.Current.Request.Url.Host,
                    UCommcerVersion = _environmentAnalyser.UCommerceVersion(),
                    AppVersion = _environmentAnalyser.AppVersion(),
                    CMS = _environmentAnalyser.Cms().ToString()
                };

                var javaScriptSerializer = new JavaScriptSerializer();

                using (var webClient = new WebClient())
                {
                    webClient.Headers.Add("Content-Type", "application/json");

                    var uploadString = webClient.UploadString("http://appcenter.pragmasoft.dk/apps/verify", "POST",
                        javaScriptSerializer.Serialize(verificationRequest));

                    verificationResponse = javaScriptSerializer.Deserialize<VerificationResponse>(uploadString);
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return verificationResponse;
        }

        public class VerificationResponse
        {
            public bool UpdateRequired { get; set; }
            public bool Discontinued { get; set; }
            public bool LicenseAccepted { get; set; }
            public string Message { get; set; }
        }

        public class VerificationRequest
        {
            public string UCommcerVersion { get; set; }
            public string AppVersion { get; set; }
            public string Host { get; set; }
            public string CMS { get; set; }
            public Guid AppId { get; set; }
            public string LicenseKey { get; set; }
        }
    }
}
