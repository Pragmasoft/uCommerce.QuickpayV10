using System;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Pragmasoft.QuickpayV10.Extensions.Services.AppCenter;
using Pragmasoft.QuickpayV10.Extensions.Services.Interfaces;
using UCommerce.EntitiesV2;
using UCommerce.Infrastructure;
using UCommerce.Infrastructure.Environment;
using UCommerce.Transactions.Payments;
using UCommerce.Transactions.Payments.Common;

namespace Pragmasoft.QuickpayV10.Extensions.Services
{
    /// <summary>
	/// Implementation of the http://quickpay.dk payment provider(v10).
	/// </summary>
    public class QuickpayV10PaymentMethodService : ExternalPaymentMethodService
    {
        private readonly IWebRuntimeInspector _webRuntimeInspector;
        private readonly IQuickPayV10CallbackAnalyser _callbackAnalyser;
        private readonly IQuickPayV10Logger _logger;
        private readonly QuickpayV10Repository _quickpayRepository;
        private readonly AbstractPageBuilder _pageBuilder;
        private readonly PragmasoftAppCenterService _appCenterService;

        public QuickpayV10PaymentMethodService(QuickpayV10PageBuilder pageBuilder, QuickpayV10Repository quickpayV10Repository,
            IWebRuntimeInspector webRuntimeInspector, IQuickPayV10CallbackAnalyser callbackAnalyser, IQuickPayV10Logger logger)
        {
            _webRuntimeInspector = webRuntimeInspector;
            _callbackAnalyser = callbackAnalyser;
            _logger = logger;
            _pageBuilder = pageBuilder;
            _quickpayRepository = quickpayV10Repository;

            //this initialization is hardcoded here, to avoid users overriding the services in the IoC container
            _appCenterService = new PragmasoftAppCenterService(new Guid("d66a8d60-1f56-4b12-8231-6930396bfa40"),
                                new AppEnvironmentAnalyser(), new QuickPayV10AppVerifier(_logger));
        }

        private void PragmasoftAppCenterValidation()
        {
            //currently we don't have a license for this app
            _appCenterService.VerifyApp(Guid.Empty.ToString("D"));
        }

        /// <summary>
        /// Renders the page with the information needed by the payment provider.
        /// </summary>
        /// <param name="paymentRequest">The payment request.</param>
        /// <returns>The html rendered.</returns>
        public override string RenderPage(PaymentRequest paymentRequest)
        {
            PragmasoftAppCenterValidation();
            return _pageBuilder.Build(paymentRequest);
        }

        /// <summary>
        /// Processes the callback and excecutes a pipeline if there is one specified for this paymentmethodservice.
        /// </summary>
        /// <param name="payment">The payment to process.</param>
        public override void ProcessCallback(Payment payment)
        {
            try
            {
                Guard.Against.MissingHttpContext(_webRuntimeInspector);
                PragmasoftAppCenterValidation();

                var callbackObject = _callbackAnalyser.ReadCallbackBody(HttpContext.Current);
                if (!(callbackObject.State == "processed" || callbackObject.State == "new"))
                {
                    return;
                }
                Guard.Against.PaymentNotPendingAuthorization(payment);
                var paymentProperties = _quickpayRepository.GetPaymentProperties(payment);
                if (_quickpayRepository.ValidatePayment(paymentProperties, callbackObject))
                {
                    if (callbackObject.test_mode && paymentProperties.CancelTestCardOrders)
                    {

                        _quickpayRepository.ChangeOrderStatus(payment, "Cancelled",
                            PaymentStatus.Get((int)PaymentStatusCode.Cancelled));
                        _logger.Log("Order (" + payment.PurchaseOrder.OrderNumber + ") was canceled because the payment was in testmode.");
                        return;
                    }


                    payment.PaymentStatus = PaymentStatus.Get((int)PaymentStatusCode.Authorized);
                    ProcessPaymentRequest(new PaymentRequest(payment.PurchaseOrder, payment));

                    if (paymentProperties.AutoCapture)
                    {
                        _quickpayRepository.ChangeOrderStatus(payment, "Completed order",
                            PaymentStatus.Get((int)PaymentStatusCode.Acquired));
                    }
                }
                else
                {
                    _logger.Log("Payment for order '" + payment.PurchaseOrder.OrderNumber + "' not validated");
                    payment.PaymentStatus = PaymentStatus.Get((int)PaymentStatusCode.Declined);
                    payment.Save(); //Save payment to ensure transactionId not lost.
                }
            }
            catch (Exception ex)
            {
                _logger.LogException(ex);
            }
        }

        /// <summary>
		/// Cancels the payment from the payment provider. This is often used when you need to call external services to handle the cancel process.
		/// </summary>
		/// <param name="payment">The payment.</param>
		/// <param name="status">The status.</param>
		/// <returns></returns>
        protected override bool CancelPaymentInternal(Payment payment, out string status)
        {
            PragmasoftAppCenterValidation();
            var responseDto = _quickpayRepository.CancelPayment(payment);
            status = responseDto.StatusMessage;
            return responseDto.ResponseAccepted;
        }

        /// <summary>
		/// Acquires the payment from the payment provider. This is often used when you need to call external services to handle the acquire process.
		/// </summary>
		/// <param name="payment">The payment.</param>
		/// <param name="status">The status.</param>
		/// <returns></returns>
        protected override bool AcquirePaymentInternal(Payment payment, out string status)
        {
            PragmasoftAppCenterValidation();
            var responseDto = _quickpayRepository.CapturePayment(payment);
            status = responseDto.StatusMessage;
            return responseDto.ResponseAccepted;
        }

        /// <summary>
		/// Refunds the payment from the payment provider. This is often used when you need to call external services to handle the refund process.
		/// </summary>
		/// <param name="payment">The payment.</param>
		/// <param name="status">The status.</param>
		/// <returns></returns>
        protected override bool RefundPaymentInternal(Payment payment, out string status)
        {
            PragmasoftAppCenterValidation();
            var responseDto = _quickpayRepository.RefundPayment(payment);
            status = responseDto.StatusMessage;
            return responseDto.ResponseAccepted;
        }
    }
}
