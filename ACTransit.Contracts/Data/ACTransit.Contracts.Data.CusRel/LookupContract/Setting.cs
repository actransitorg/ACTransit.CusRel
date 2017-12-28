using System;

namespace ACTransit.Contracts.Data.CusRel.LookupContract
{
    public class Setting
    {
        public int SettingId { get; set; }
        public Guid? GroupdId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}