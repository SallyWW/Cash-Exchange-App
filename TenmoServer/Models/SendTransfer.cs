using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class SendTransfer
    {
        public int ReceiveId { get; set; }
        public int SendId { get; set; }
        public decimal Amount { get; set; }
    }
}
