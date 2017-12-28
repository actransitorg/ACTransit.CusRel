using System;

namespace ACTransit.Framework.Interfaces
{
    public interface ITenant
    {
        Guid TenantId { get; set; }
    }
}