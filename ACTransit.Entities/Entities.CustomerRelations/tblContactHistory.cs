//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACTransit.Entities.CustomerRelations
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblContactHistory
    {
        public int FileNum { get; set; }
        public string UserId { get; set; }
        public Nullable<System.DateTime> ContactDateTime { get; set; }
        public string Via { get; set; }
        public string Comment { get; set; }
        public int Id { get; set; }
    
        public virtual tblContacts tblContacts { get; set; }
    }
}
