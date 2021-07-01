using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class API_Transfer
    {
        public int ReceiveId { get; set; }
        public int SendId { get; set; }
        public decimal Amount { get; set; }
    }
}
