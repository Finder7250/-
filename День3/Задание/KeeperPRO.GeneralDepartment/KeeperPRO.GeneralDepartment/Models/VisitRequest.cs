using System;

namespace KeeperPRO_GeneralDepartment.Models
{
    public class VisitRequest
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Purpose { get; set; }
        public int DepartmentId { get; set; }
        public int EmployeeId { get; set; }
        public string Status { get; set; }
        public string RejectionReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeFullName { get; set; }
        public string VisitorName { get; set; }
        public string Passport { get; set; }
        public string Phone { get; set; }
        public string VisitorEmail { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsBlacklisted { get; set; }
    }
}