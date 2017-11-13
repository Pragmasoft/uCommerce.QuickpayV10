using System;
using System.Text;
using Pragmasoft.QuickpayV10.Extensions.Services.Interfaces;
using UCommerce.Content;
using UCommerce.Transactions.Payments;

namespace Pragmasoft.QuickpayV10.Extensions.Services
{
    public class QuickpayV10PageBuilder: AbstractPageBuilder
    {
        private readonly IQuickPayV10Logger _logger;

        private IDomainService DomainService { get; set; }
        private QuickpayV10Repository QuickpayRepository { get; set; }

        public QuickpayV10PageBuilder(IDomainService domainService, QuickpayV10Repository quickpayV10Repository, IQuickPayV10Logger logger)
        {
            _logger = logger;
            DomainService = domainService;
            QuickpayRepository = quickpayV10Repository;
        }

        protected override void BuildHead(StringBuilder page, PaymentRequest paymentRequest)
        {
            page.Append("<title>QuickpayV10</title>");
            page.Append("<script type=\"text/javascript\" src=\"//ajax.googleapis.com/ajax/libs/jquery/1.4.2/jquery.min.js\"></script>");
            page.Append(@"<script type=""text/javascript"">$(function(){ $('form').submit();});</script>");
        }

        protected virtual string GetTwoLetterLanguageName()
        {
            Domain domain = DomainService.GetCurrentDomain();
            return domain != null
                ? domain.Culture.TwoLetterISOLanguageName
                : System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
        }

        protected override void BuildBody(StringBuilder page, PaymentRequest paymentRequest)
        {
            if(paymentRequest == null) throw new ArgumentNullException("paymentRequest");

            var paymentLink = QuickpayRepository.GetPaymentLink(paymentRequest);

            if (String.IsNullOrWhiteSpace(paymentLink))
            {
                var expMsg = "The payment link url was null or empty.";
                _logger.Log(expMsg);
                throw new NotSupportedException(expMsg);
            }

            page.Append(String.Format("<form id=\"Quickpay\" name=\"Quickpay\" action=\"{0}\">", paymentLink));
            page.Append("</form>");
        }

        
    }
}
