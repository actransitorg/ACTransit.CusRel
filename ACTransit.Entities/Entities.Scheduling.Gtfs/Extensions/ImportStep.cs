using ACTransit.Framework.Interfaces;

namespace ACTransit.Entities.Scheduling.Gtfs
{
    public partial class ImportStep : IAuditableEntity
    {
        public enum Values 
        {
            Initialize = 1,
            Extract = 2,
            Import = 3,
            CopyToServer = 4,
            Finalize = 5
        }
    }
}