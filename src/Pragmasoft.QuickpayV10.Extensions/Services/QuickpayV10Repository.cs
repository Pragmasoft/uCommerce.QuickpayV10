using System;
using System.Globalization;
using System.IO;
using System.Linq;
using UCommerce.Content;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using Pragmasoft.QuickpayV10.Extensions.Extensions;
using Pragmasoft.QuickpayV10.Extensions.Models;
using Pragmasoft.QuickpayV10.Extensions.Models.Callback;
using Pragmasoft.QuickpayV10.Extensions.Services.Interfaces;
using UCommerce.Infrastructure;
using UCommerce.Transactions;
using UCommerce.Transactions.Payments;
using UCommerce.Web;

namespace Pragmasoft.QuickpayV10.Extensions.Services
{
    using UCommerce.EntitiesV2;

    public class QuickpayV10Repository
    {
        
        private const string ApiEndpointUrl = "https://api.quickpay.net/";
        private readonly ICallbackUrl _callbackUrl;
        private readonly IQuickPayV10Logger _logger;
        private readonly IDomainService _domainService;

        public QuickpayV10Repository(IDomainService domainService, ICallbackUrl callbackUrl, IQuickPayV10Logger logger)
        {
            _domainService = domainService;
            _callbackUrl = callbackUrl;
            _logger = logger;
        }

        public string GetPaymentLink(PaymentRequest paymentRequest)
        {
            var payment = paymentRequest.Payment;
            var paymentProperties = GetPaymentProperties(paymentRequest);

            if (string.IsNullOrEmpty(payment.TransactionId))
            {
                var newPayment = CreatePayment(paymentProperties);
                if (!string.IsNullOrEmpty(newPayment.Id))
                {
                    payment.TransactionId = newPayment.Id;
                }
                // Cast exception if no access to quickpay api
                if (payment.TransactionId == null) throw new NotSupportedException(newPayment.StatusMessage);
            }
            payment.Save();
            return GetPaymentLink(payment, payment.TransactionId, paymentProperties);
        }

        public ResponseDto CapturePayment(Payment payment)
        {
            var amount = payment.Amount.ToCents();
            if (amount <= 0) { CancelPayment(payment); return new ResponseDto { StatusMessage = "Cannot capture when amount is zero or less" }; }
            if(payment.PaymentStatus == PaymentStatus.Get((int)PaymentStatusCode.Declined))
                return new ResponseDto { StatusMessage = "Payment was declined." };

            var resource = string.Format("payments/{0}/capture?amount={1}", payment.TransactionId, payment.Amount.ToCents());
            var method = "POST";
            var paymentProperties = GetPaymentProperties(payment);
            // If payment has auto capture then check if payment has been captured
            if (paymentProperties.AutoCapture)
            {
                resource = string.Format("payments/{0}", payment.TransactionId);
                method = "GET";
            }
            var responseDto = GetResponseDto(resource, payment, method);
            if (responseDto.ResponseAccepted == false && paymentProperties.AutoCapture)
            {
                ChangeOrderStatus(payment, "Requires attention", PaymentStatus.Get((int)PaymentStatusCode.AcquireFailed));
            }

            if(responseDto.ResponseAccepted == false && !paymentProperties.AutoCapture) payment.PaymentStatus = PaymentStatus.Get((int)PaymentStatusCode.AcquireFailed);
            return responseDto;
        }

        public Payment ChangeOrderStatus(Payment payment, string orderStatusName, PaymentStatus paymentStatus)
        {
            var newOrderStatus = OrderStatus.All().Single(x => x.Name == orderStatusName);
            var orderService = ObjectFactory.Instance.Resolve<IOrderService>();
            orderService.ChangeOrderStatus(payment.PurchaseOrder, newOrderStatus);
            payment.PaymentStatus = paymentStatus;
            payment.Save();
            return payment;
        }

        public ResponseDto RefundPayment(Payment payment)
        {
            var resource = string.Format("payments/{0}/refund?amount={1}", payment.TransactionId, payment.Amount.ToCents());
            return GetResponseDto(resource, payment, "POST");
        }

        public ResponseDto CancelPayment(Payment payment)
        {
            var resource = string.Format("payments/{0}/cancel", payment.TransactionId);
            return GetResponseDto(resource, payment, "POST");
        }

        private ResponseDto GetResponseDto(string resoruceUrl, Payment payment, string method)
        {
            var response = GetResponse(GetPaymentProperties(payment), resoruceUrl, method);
            var quickpayDto = GetPayment(payment, response);
            return new ResponseDto { ResponseAccepted = quickpayDto.Accepted, StatusMessage = quickpayDto.StatusMessage };
        }

        public bool ValidatePayment(PaymentProperties paymentProperties, QuickpayApiResponseDto callbackObject)
        {
            try
            {
                var currentHttpContext = HttpContext.Current;
                if (ValidateChecksum(currentHttpContext, paymentProperties.PrivateAccountKey))
                {
                    // Get operations to check if payment has been approved
                    var operations = callbackObject.Operations.LastOrDefault();
                    // Check if payment has been approved
                    return operations != null && (operations.qp_status_code == "000" || operations.qp_status_code == "20000") && operations.qp_status_msg.ToLower() == "approved";
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, "ValidatePayment Exception:  ");
            }
            return false;
        }

        private string GetPaymentLink(Payment payment, string transactionId, PaymentProperties paymentProperties)
        {
            var order = payment.PurchaseOrder;
            var resource = string.Format("payments/{0}/link?amount={1}&language={2}", transactionId, paymentProperties.Amount, paymentProperties.Language);

            if (paymentProperties.CallbackUrl != "")
            {
                var callbackurl = _callbackUrl.GetCallbackUrl(paymentProperties.CallbackUrl, payment);
                resource += "&callbackurl=" + HttpUtility.UrlEncode(callbackurl);
            }

            if (paymentProperties.ContinueUrl != "")
                resource += UrlWithOrderId("continueurl", paymentProperties.ContinueUrl, order.OrderId, order.OrderGuid);

            if (paymentProperties.CancelUrl != "")
                resource += UrlWithOrderId("cancelurl", paymentProperties.CancelUrl, order.OrderId, order.OrderGuid);

            if (paymentProperties.AutoCapture)
                resource += "&autocapture=true";

            var response = GetResponse(paymentProperties, resource, "PUT");
            return response.Url;
        }

        private string UrlWithOrderId(string name, string url, int orderId, Guid orderGuid)
        {
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["orderId"] = orderId.ToString(CultureInfo.InvariantCulture);
            query["orderGuid"] = orderGuid.ToString("D", CultureInfo.InvariantCulture);
            uriBuilder.Query = query.ToString();
            return "&" + name + "=" + HttpUtility.UrlEncode(uriBuilder.ToString());
        }

        private QuickpayApiResponseDto CreatePayment(PaymentProperties paymentProperties)
        {
            var resource = string.Format("payments?order_id={0}&amount={1}&currency={2}", paymentProperties.OrderNumber, paymentProperties.Amount, paymentProperties.Currency);
            var response = GetResponse(paymentProperties, resource, "POST");
            return response;
        }

        public PaymentProperties GetPaymentProperties(PaymentRequest paymentRequest)
        {
            return new PaymentProperties(paymentRequest, GetTwoLetterLanguageName());
        }

        public PaymentProperties GetPaymentProperties(Payment payment)
        {
            return new PaymentProperties(payment);
        }

        public QuickpayApiResponseDto GetPayment(Payment payment, QuickpayApiResponseDto quickpayApiResponseDto)
        {
            for (var attempts = 0; attempts < 3; attempts++)
            {
                var resource = string.Format("payments/{0}", payment.TransactionId);
                var responseDto = GetResponse(GetPaymentProperties(payment), resource, "GET");
                if (responseDto.Operations != null)
                {
                    var operation = responseDto.Operations.LastOrDefault();
                    if (operation != null && !operation.pending)
                    {
                        quickpayApiResponseDto = responseDto;
                        break;
                    }
                }
                // Sleep for 1 second before trying again
                Thread.Sleep(1000);
            }
            return quickpayApiResponseDto;
        }

        private bool ValidateChecksum(HttpContext currentHttpContext, string privateAccountKey)
        {
            var request = currentHttpContext.Request;
            var requestCheckSum = request.Headers["QuickPay-Checksum-Sha256"];

            if (requestCheckSum == "") return false;
            var inputStream = request.InputStream;
            var bytes = new byte[inputStream.Length];
            request.InputStream.Position = 0;
            request.InputStream.Read(bytes, 0, bytes.Length);
            request.InputStream.Position = 0;
            var content = Encoding.UTF8.GetString(bytes);
            var calculatedChecksum = Checksum(content, privateAccountKey);

            return requestCheckSum.Equals(calculatedChecksum);
        }

        private string Checksum(string content, string privateKey)
        {
            var e = Encoding.UTF8;
            var hmac = new HMACSHA256(e.GetBytes(privateKey));
            var b = hmac.ComputeHash(e.GetBytes(content));
            var s = new StringBuilder();

            foreach (var t in b)
            {
                s.Append(t.ToString("x2"));
            }
            return s.ToString();
        }

        private static HttpWebRequest CreateWebRequest(string apiKey, string resource, string method)
        {
            var endPointUrl = ApiEndpointUrl + resource;
            var basicAuth = Convert.ToBase64String(Encoding.Default.GetBytes(":" + apiKey));
            var request = (HttpWebRequest)WebRequest.Create(endPointUrl);

            request.Headers["Authorization"] = "Basic " + basicAuth;
            request.Method = method;
            request.Headers.Add("accept-version", "v10");

            return request;
        }

        private QuickpayApiResponseDto GetResponse(PaymentProperties paymentProperties, string resource, string method)
        {
            var request = CreateWebRequest(paymentProperties.ApiKey, resource, method);
            var quickpayDto = new QuickpayApiResponseDto();
            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                {
                    var responseWrite = ReadResponse(response);
                    quickpayDto = JsonConvert.DeserializeObject<QuickpayApiResponseDto>(responseWrite);
                    var operations = quickpayDto.Operations;
                    if (operations != null)
                    {
                        var operation = operations.LastOrDefault();
                        if (operation != null && !string.IsNullOrEmpty(operation.qp_status_code) &&
                            !string.IsNullOrEmpty(operation.type))
                        {
                            quickpayDto.StatusMessage = GetStatusMessage(operation.type, operation.qp_status_code,
                                operation.qp_status_msg);
                            if (operation.qp_status_code.ToLower() != "20000") quickpayDto.Accepted = false;
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                _logger.LogException(ex);

                var response = ex.Response as HttpWebResponse;
                if (response != null)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized ||
                        response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        var response2 = ReadResponse(response);
                        quickpayDto.StatusMessage = JsonConvert.DeserializeObject<RequestFailedDto>(response2).Message;
                    }
                    else
                    {
                        quickpayDto.StatusMessage = ex.Message;
                    }
                }
                else
                {
                    quickpayDto.StatusMessage = ex.Message;
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
                throw;
            }
            return quickpayDto;
        }

        private string ReadResponse(HttpWebResponse response)
        {
            var responseValue = string.Empty;
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                    using (var reader = new StreamReader(responseStream))
                    {
                        responseValue = reader.ReadToEnd();
                    }
            }
            return responseValue;
        }

        private string GetStatusMessage(string type, string statusCode, string statusMessage)
        {
            var message = "";
            type = type.ToLower();
            statusCode = statusCode.ToLower();

            if (statusCode == "20000")
            {
                switch (type)
                {
                    case "authorize":
                        message = "Payment authorized";
                        break;
                    case "capture":
                        message = "Payment captured";
                        break;
                    case "cancel":
                        message = "Payment cancelled";
                        break;
                    case "refund":
                        message = "Payment refunded";
                        break;
                }
            }
            else if (statusCode == "40000")
            {
                switch (type)
                {
                    case "refund":
                        message = "Refund rejected by quickpay (" + statusMessage + ")";
                        break;
                    case "cancel":
                        message = "Cancel rejected by quickpay (" + statusMessage + ")";
                        break;
                    case "capture":
                        message = "Capture rejected by quickpay (" + statusMessage + ")";
                        break;
                    default:
                        message = "Rejected By Acquirer(Check quickpay if user data is valid e.g if card has expired)";
                        break;
                }
            }
            else if (statusCode == "40001")
            {
                message = "Request Data Error";
            }
            else if (statusCode == "50000")
            {
                message = "Gateway Error";
            }
            else if (statusCode == "50300")
            {
                message = "Communications Error (with Acquirer)";
            }
            else
            {
                switch (type)
                {
                    case "refund":
                        message = "Refund payment is pending. Try again later";
                        break;
                    case "cancel":
                        message = "Cancel payment is pending. Try again later";
                        break;
                    case "capture":
                        message = "Capture payment is pending. Try again later";
                        break;
                    default:
                        message = "Unknown Error";
                        break;
                }
            }
            return message;
        }
        private string GetTwoLetterLanguageName()
        {
            var domain = _domainService.GetCurrentDomain();
            return domain != null
                ? domain.Culture.TwoLetterISOLanguageName
                : Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        }
    }
}
