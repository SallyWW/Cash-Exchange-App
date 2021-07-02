using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class API_TransferDetails
    {
        public int TransferId { get; set; }
        public string FromUsername { get; set; }
        public int FromUserId { get; set; }
        public string ToUserName { get; set; }
        public int ToUserId { get; set; }
        public decimal TransferAmount { get; set; }
        public string TypeSendId { get; set; } = "Send";
        public string StatusApprovedId { get; set; } = "Approved";
    }
}
