using Pragmasoft.QuickpayV10.Extensions.Extensions;
using UCommerce.EntitiesV2;
using UCommerce.Extensions;
using UCommerce.Transactions.Payments;

namespace Pragmasoft.QuickpayV10.Extensions.Models
{
    public class PaymentProperties
    {
        public string ApiKey { get; set; }
        public string PrivateAccountKey { get; set; }
        public string Merchant { get; set; }
        public string AgreementId { get; set; }
        public string Language { get; set; }
        public string OrderNumber { get; set; }
        public string Amount { get; set; }
        public string Currency { get; set; }
        public string ContinueUrl { get; set; }
        public string CancelUrl { get; set; }
        public string CallbackUrl { get; set; }
        public bool AutoCapture { get; set; }
        public bool CancelTestCardOrders { get; set; }

        public PaymentProperties()
        {

        }

        private PaymentProperties(PaymentMethod paymentMethod)
        {
            ApiKey = paymentMethod.DynamicProperty<string>().ApiKey;
            PrivateAccountKey = paymentMethod.DynamicProperty<string>().PrivateAccountKey;
            Merchant = paymentMethod.DynamicProperty<string>().Merchant;
            AgreementId = paymentMethod.DynamicProperty<string>().AgreementId;
            ContinueUrl = paymentMethod.DynamicProperty<string>().ContinueUrl;
            CancelUrl = paymentMethod.DynamicProperty<string>().CancelUrl;
            CallbackUrl = paymentMethod.DynamicProperty<string>().CallbackUrl;
            AutoCapture = paymentMethod.DynamicProperty<bool>().AutoCapture;
            CancelTestCardOrders = paymentMethod.DynamicProperty<bool>().CancelTestCardOrders;
        }

        public PaymentProperties(PaymentRequest paymentRequest, string language) : this(paymentRequest.PaymentMethod)
        {
            PopulatePaymentProperties(paymentRequest.Payment);
            Language = language;
            Currency = paymentRequest.Amount.Currency.ISOCode;
        }

        public PaymentProperties(Payment payment) : this(payment.PaymentMethod)
        {
            PopulatePaymentProperties(payment);
        }

        private void PopulatePaymentProperties(Payment payment)
        {
            Amount = payment.Amount.ToCents().ToString();
            OrderNumber = payment.ReferenceId;
        }
    }
}
