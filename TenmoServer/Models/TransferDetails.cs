using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class TransferDetails
    {
        public int TransferId { get; set; }
        public string FromUsername { get; set; }
        public int FromUserId { get; set; }
        public string ToUserName { get; set; }
        public int ToUserId { get; set; }
        public decimal TransferAmount { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }
}
