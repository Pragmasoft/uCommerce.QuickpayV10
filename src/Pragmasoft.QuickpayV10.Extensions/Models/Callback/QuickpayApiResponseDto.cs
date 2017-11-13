using System;
using System.Collections.Generic;

namespace Pragmasoft.QuickpayV10.Extensions.Models.Callback
{
    public class QuickpayApiResponseDto
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public List<Operation> Operations { get; set; }
        public Metadata MetaData { get; set; }
        public string StatusMessage { get; set; }
        public string State { get; set; }

        public bool test_mode{get;set;}

        public bool Accepted { get; set; }

    }
}
