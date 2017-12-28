using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using ACTransit.Contracts.Data.CusRel.LookupContract;
using ACTransit.Contracts.Data.CusRel.LookupContract.Result;
using ACTransit.DataAccess.CustomerRelations;
using ACTransit.CusRel.Repositories.Mapping;

using ACTransit.Framework.Caching;

namespace ACTransit.CusRel.Repositories
{
    public class SettingsRepository: IDisposable
    {
        private CusRelEntities cusRelContext;

        private const string _THISCONTEXTCACHEKEYNAME = "CusRel.Settings";

        #region Constructors / Initialization

        public SettingsRepository()
        {
        }

        public SettingsRepository(CusRelEntities context)
        {
            cusRelContext = context;
        }

        public void InitCusRelContext()
        {
            if (cusRelContext == null)
                cusRelContext = new CusRelEntities();
        }

        #endregion

        // =============================================================

        #region Save/Dispose

        public int SaveChanges()
        {
            return cusRelContext != null ? cusRelContext.SaveChanges() : 0;
        }

        private SettingResult AddOrUpdate(Setting Setting)
        {
            var result = new SettingResult
            {
                Setting = Setting
            };

            try
            {
                var setting = Setting.ToEntities();
                if (setting.SettingId == -1)
                {
                    cusRelContext.Settings.Attach(setting);
                    cusRelContext.Settings.Remove(setting);
                }
                else
                    cusRelContext.Settings.AddOrUpdate(setting);
                var count = SaveChanges();
                if (count > 0)
                    result.SetOK();
            }
            catch (Exception e)
            {
                result.SetFail(e);
            }
            return result;
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                if (cusRelContext != null)
                    cusRelContext.Dispose();
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        // =============================================================

        #region Public Methods

        public int GetSettingsCount()
        {
            try
            {
                InitCusRelContext();
                return cusRelContext.Settings.Count();
            }
            catch (Exception e)
            {
                return -1;
            }            
        }

        public SettingResult GetSetting(int SettingId)
        {
            return GetSetting(new Setting {SettingId = SettingId});
        }

        public SettingResult GetSetting(string Name, Guid? GroupId = null)
        {
            return GetSetting(new Setting { Name = Name, GroupdId = GroupId });
        }

        public SettingResult GetSetting(Setting FilterBy)
        {
            SettingsResult cusRelSettingsResult = GetSettings(FilterBy);

            return new SettingResult
            {
                Errors = cusRelSettingsResult.Errors,
                Id = cusRelSettingsResult.Id,
                OK = cusRelSettingsResult.OK,
                Setting = cusRelSettingsResult.Settings.FirstOrDefault(),
                StatusCode = cusRelSettingsResult.StatusCode
            };
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
            SettingsResult cusRelSettingsResult = new SettingsResult();

            try
            {

                // Implement distributed cache instead since this application will be load balanced

                //var cusRelSettings = DomainCache.GetCache(_THISCONTEXTCACHEKEYNAME) as List<Setting>;

                //if (cusRelSettings == null)
                //{
                //    InitCusRelContext();

                //    if (cusRelContext.Settings != null && cusRelContext.Settings.Count() > 0)
                //    {
                //        cusRelSettings = cusRelContext.Settings.ToList().FromEntities();
                //        DomainCache.AddCache(_THISCONTEXTCACHEKEYNAME, cusRelSettings);
                //    }
                //}

                InitCusRelContext();

                var cusRelSettings = cusRelContext.Settings.ToList().FromEntities();

                if (FilterBy != null)
                {
                    cusRelSettings = cusRelSettings.Where(s => (FilterBy.GroupdId.HasValue && s.GroupdId.Value.Equals(FilterBy.GroupdId.Value)
                                                                || (FilterBy.Name != null && s.Name.Equals(FilterBy.Name))
                                                                || (FilterBy.Value != null && s.Value.Equals(FilterBy.Value)))).ToList();
                }

                cusRelSettingsResult.Settings = cusRelSettings;

            }
            catch (Exception e)
            {
                cusRelSettingsResult.SetFail(e);
            }

            return cusRelSettingsResult;
        }

        public SettingResult SaveSetting(Setting Setting)
        {
            try
            {
                InitCusRelContext();
                var result = AddOrUpdate(Setting);

                // Implement distributed cache instead since this application will be load balanced
                //if (DomainCache.GetCache(_THISCONTEXTCACHEKEYNAME) != null)
                //    DomainCache.Remove(_THISCONTEXTCACHEKEYNAME);

                return result;
            }
            catch (Exception e)
            {
                var result = new SettingResult();
                result.SetFail(e);
                return result;
            }
        }

        public SettingsResult SaveSettings(List<Setting> Settings)
        {
            var result = new SettingsResult {Settings = Settings};
            try
            {
                InitCusRelContext();

                foreach (var addOrUpdateResult in Settings.Select(AddOrUpdate))
                {
                    result.MergeResults(addOrUpdateResult);
                    if (!addOrUpdateResult.OK) 
                        break;
                }

                // Implement distributed cache instead since this application will be load balanced
                //if (DomainCache.GetCache(_THISCONTEXTCACHEKEYNAME) != null)
                //    DomainCache.Remove(_THISCONTEXTCACHEKEYNAME);
            }
            catch (Exception e)
            {
                result.SetFail(e);
            }
            return result;
        }

        public SortedDictionary<string, string[]> GetLostItemNodes()
        {
            var result = new SortedDictionary<string, string[]>();
            var categories = GetSetting(new Setting {Name = "LostProperty"}).Setting.Value.Trim().Split('|');
            foreach (var category in categories)
            {
                if (string.IsNullOrEmpty(category)) continue;
                var nameValue = category.Split(':');
                var key = nameValue[0].Trim();
                var afterKey = nameValue[1].Trim();
                try
                {
                    result.Add(key, afterKey.Split(';').ToList().Select(a => a.Trim()).OrderBy(a => a).ToArray());
                }
                catch(ArgumentException e)
                {
                    throw new ArgumentException("Duplicate Lost Item Category: " + key, e);
                }
            }
            return result;
        }

        public List<string> ParseSettings(string settingName, char separator)
        {
            return GetSetting(new Setting { Name = settingName }).Setting.Value.Trim().Split(separator).ToList();
        }

        public Dictionary<string, string[]> ParseMultipleSettings(string settingName, char settingSeparator, char fieldSeparator, char inlineSeparator)
        {
            var result = new Dictionary<string, string[]>();

            var thisSettings = ParseSettings(settingName, settingSeparator);

            foreach (var entry in thisSettings)
            {
                if (!string.IsNullOrEmpty(entry))
                {
                    string[] config = entry.Split(fieldSeparator);

                    if (config.Length > 0)
                        result.Add(config[0], config[1].Split(inlineSeparator));
                }
            }

            return result;
        }

        #endregion

        // =============================================================

        #region Test Data

        #endregion
    }
}
