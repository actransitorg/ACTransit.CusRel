using System;
using System.Collections.Generic;
using ACTransit.Contracts.Data.CusRel.LookupContract;
using ACTransit.Contracts.Data.CusRel.LookupContract.Result;
using ACTransit.CusRel.Repositories;

namespace ACTransit.CusRel.Services
{
    public class SettingsService
    {
        private readonly SettingsRepository settingsRepository = new SettingsRepository();

        public int GetSettingsCount()
        {
            return settingsRepository.GetSettingsCount();
        }

        public SettingResult GetSetting(int SettingId)
        {
            return settingsRepository.GetSetting(SettingId);
        }

        public SettingResult GetSetting(string Name, Guid? GroupId = null)
        {
            return settingsRepository.GetSetting(Name, GroupId);
        }

        public SettingResult GetSetting(Setting FilterBy)
        {
            return settingsRepository.GetSetting(FilterBy);
        }

        public SettingsResult GetSettings(string Name, Guid? GroupId = null)
        {
            return GetSettings(new Setting { Name = Name, GroupdId = GroupId });
        }

        public SettingsResult GetSettings(Guid GroupId)
        {
            return GetSettings(new Setting { GroupdId = GroupId });
        }

        public SettingsResult GetSettings(Setting FilterBy = null)
        {
            return settingsRepository.GetSettings(FilterBy);
        }

        public SettingResult SaveSetting(Setting Setting)
        {
            return settingsRepository.SaveSetting(Setting);
        }

        public SettingsResult SaveSettings(List<Setting> Settings)
        {
            return settingsRepository.SaveSettings(Settings);
        }

        public SortedDictionary<string, string[]> GetLostItemNodes()
        {
            return settingsRepository.GetLostItemNodes();
        }

        public Dictionary<string, string[]> ParseMultipleSettings(string settingName, char settingSeparator = '|', char fieldSeparator = ':', char inlineSeparator = ',')
        {
            return settingsRepository.ParseMultipleSettings(settingName, settingSeparator, fieldSeparator, inlineSeparator);
        }

        public List<string> ParseSettings(string settingName, char separator = ',')
        {
            return settingsRepository.ParseSettings(settingName, separator);
        }
    }
}
