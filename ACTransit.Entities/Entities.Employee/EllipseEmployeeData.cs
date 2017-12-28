using System;

namespace ACTransit.Entities.Employee
{
    public class EllipseEmployeeData
    {
        public string ChangeStatus { get; set; }
        public string EmployeeID { get; set; }
        public string PrefFirstName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string PreferredPhone { get; set; }
        public string EmployeeJobStatus { get; set; }
        public string EmployeeJobStatusDesc { get; set; }
        public string HireDate { get; set; }
        public Nullable<System.DateTime> EffectiveDate { get; set; }
        public Nullable<System.DateTime> RehireDate { get; set; }
        public Nullable<System.DateTime> LastWorkDate { get; set; }
        public string DeptId { get; set; }
        public string DeptName { get; set; }
        public string SupervisorId { get; set; }
        public string SupervisorName { get; set; }
        public string JobCode { get; set; }
        public string JobTitle { get; set; }
        public string WorkLocation { get; set; }
        public string UnionCode { get; set; }
        public string EmployeeType { get; set; }
        public string Grade { get; set; }
        public string Step { get; set; }
        public string PositionNumber { get; set; }
        public Nullable<System.DateTime> PositionEntryDate { get; set; }
        public string OperatorQualDate { get; set; }
        public string OperatorQualTime { get; set; }
        public string DriverLicenseNumber { get; set; }
        public string DriverLicenseExpirationDate { get; set; }
    }
}
