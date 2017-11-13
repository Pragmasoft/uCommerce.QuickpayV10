using System;

namespace Pragmasoft.QuickpayV10.Extensions.Models.Callback
{
    public class Operation
    {
        public string id { get; set; }
        public string type { get; set; }
        public string amount { get; set; }
        public bool pending { get; set; }
        public string qp_status_code { get; set; }
        public string qp_status_msg { get; set; }
        public string aq_status_code { get; set; }
        public string aq_status_msg { get; set; }
    }
}
