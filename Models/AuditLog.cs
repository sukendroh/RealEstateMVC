using System;
using System.ComponentModel.DataAnnotations;

namespace RealEstateMVC.Models
{
    public class AuditLog
    {
        [Key]  // Explicitly tells EF Core this is the primary key
        public int LogID { get; set; }
        public int AgentID { get; set; }
        public string Action { get; set; }
        public DateTime ActionDate { get; set; }
    }
}
