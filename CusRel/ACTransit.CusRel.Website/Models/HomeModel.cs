using System;
using System.Linq;
using System.Runtime.Serialization;
using ACTransit.Contracts.Data.CusRel.UserContract.Result;
using ACTransit.CusRel.Services;
using Newtonsoft.Json;

namespace ACTransit.CusRel.Models
{
    [DataContract, JsonObject(MemberSerialization.OptIn)]
    public class HomeModel: UsersResult
    {
        #region model Builder Objects
        #endregion

        #region SendToClient

        [DataMember]
        public string HomeContent { get; set; }

        #endregion

        #region ServerSideOnly
        #endregion

        #region Constructors

        public HomeModel(ServicesProxy servicesProxy)
        {
            try
            {
                if (servicesProxy == null)
                    throw new Exception("Services not available");
                if (servicesProxy.SettingsService == null)
                    throw new Exception("Settings not available");
                var setting = servicesProxy.SettingsService.GetSetting("HomeContent");
                if (!setting.OK)
                    throw new Exception("HomeContent not available: " + setting.Errors.Aggregate((i,j) => i + "<br/> " + j));
                HomeContent = setting.Setting.Value;
            }
            catch (Exception e)
            {
                HomeContent = "<p>Error: " + e.Message + "</p>";
                HomeContent += "<p>" + e.StackTrace + "</p>";
                var innerE = e.InnerException;
                while (innerE != null)
                {
                    HomeContent += "<p>" + innerE.Message + "</p>";
                    innerE = innerE.InnerException;
                }

            }
            
        }

        #endregion

    }
}