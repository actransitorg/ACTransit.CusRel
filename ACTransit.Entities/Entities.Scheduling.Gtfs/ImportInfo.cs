//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACTransit.Entities.Scheduling.Gtfs
{
    using System;
    using System.Collections.Generic;
    
    public partial class ImportInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ImportInfo()
        {
            this.ImportLogs = new HashSet<ImportLog>();
        }
    
        public int ImportInfoId { get; set; }
        public System.DateTime CreationDateTime { get; set; }
        public string BookingId { get; set; }
        public string FileName { get; set; }
        public Nullable<System.DateTime> EarliestServiceDate { get; set; }
        public Nullable<System.DateTime> LatestServiceDate { get; set; }
        public Nullable<int> LastImportStepId { get; set; }
        public string WorkingDirectory { get; set; }
        public string ImportDirectory { get; set; }
        public string AddUserId { get; set; }
        public System.DateTime AddDateTime { get; set; }
        public string UpdUserId { get; set; }
        public Nullable<System.DateTime> UpdDateTime { get; set; }
    
        public virtual Booking Booking { get; set; }
        public virtual ImportStep ImportStep { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImportLog> ImportLogs { get; set; }
    }
}