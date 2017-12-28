using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.LookupContract;
using ACTransit.Contracts.Data.CusRel.LookupContract.Result;
using ACTransit.CusRel.Services;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class SettingsModel : SettingsResult
    {
        #region model Builder Objects
        #endregion

        #region SendToClient

        [DataMember]
        public Setting Header { get; set; }

        #endregion

        #region ServerSideOnly

        public Setting HomePageSetting { get; private set; } 

        public List<Setting> MainSettings { get; private set; } 

        #endregion

        #region Constructors

        public SettingsModel() { }

        public SettingsModel(ServicesProxy servicesProxy)
        {
            Header = new Setting();
            var getSettings = servicesProxy.SettingsService.GetSettings();
            MergeResults(getSettings);
            Settings = getSettings.Settings;
            HomePageSetting = Settings.FirstOrDefault(s => s.Name == "HomeContent");
            MainSettings = Settings.Where(s => s.Name != "HomeContent").ToList();
        }

        #endregion

    }
}