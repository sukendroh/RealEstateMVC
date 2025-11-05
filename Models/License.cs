using System;

namespace RealEstateMVC.Models
{
    public class License
    {
        public int LicenseID { get; set; }
        public int AgentID { get; set; }
        public string LicenseNumber { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpiryDate { get; set; }

        public virtual Agent Agent { get; set; }
    }
}
