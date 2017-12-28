using System.Collections.Generic;
using System.Linq;
using ACTransit.Contracts.Data.CusRel.LookupContract;

namespace ACTransit.CusRel.Repositories.Mapping
{
    public static class SettingConversionExtensions
    {
        public static List<Setting> FromEntities(this List<Entities.CustomerRelations.Settings> Settings)
        {
            return Settings.Select(code => code.FromEntities()).ToList();
        }

        public static Setting FromEntities(this Entities.CustomerRelations.Settings Settings)
        {
            return Settings == null ? null : new Setting
            {
                SettingId = Settings.SettingId,
                GroupdId = Settings.GroupId,
                Name = Settings.Name.Trim(),
                Value = Settings.Value.Trim()
            };
        }

        public static Entities.CustomerRelations.Settings ToEntities(this Setting Setting)
        {
            return Setting == null ? null : new Entities.CustomerRelations.Settings
            {
                SettingId = Setting.SettingId,
                GroupId = Setting.GroupdId,
                Name = Setting.Name.Trim(),
                Value = string.IsNullOrEmpty(Setting.Value) ? string.Empty : Setting.Value.Trim()
            };
        }


    }
}