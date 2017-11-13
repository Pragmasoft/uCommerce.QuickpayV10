using System.Collections.Generic;

namespace Pragmasoft.QuickpayV10.Extensions.Models.Callback
{
    public class Metadata
    {
        public object type { get; set; }
        public object origin { get; set; }
        public object brand { get; set; }
        public object bin { get; set; }
        public object last4 { get; set; }
        public object exp_month { get; set; }
        public object exp_year { get; set; }
        public object country { get; set; }
        public object is_3d_secure { get; set; }
        public object hash { get; set; }
        public object number { get; set; }
        public object customer_ip { get; set; }
        public object customer_country { get; set; }
        public bool fraud_suspected { get; set; }
        public List<object> fraud_remarks { get; set; }
        public object nin_number { get; set; }
        public object nin_country_code { get; set; }
        public object nin_gender { get; set; }
    }
}
