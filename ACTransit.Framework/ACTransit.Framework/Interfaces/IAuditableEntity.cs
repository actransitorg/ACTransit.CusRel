using System;

namespace ACTransit.Framework.Interfaces
{
    public interface IAuditableEntity
    {
        DateTime AddDateTime { get; set; }

        string AddUserId { get; set; }

        DateTime? UpdDateTime { get; set; }

        string UpdUserId { get; set; }
    }
}