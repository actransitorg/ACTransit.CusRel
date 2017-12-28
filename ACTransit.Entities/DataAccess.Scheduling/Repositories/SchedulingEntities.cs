using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACTransit.Entities.Scheduling;

namespace ACTransit.DataAccess.Scheduling
{
    public partial class SchedulingEntities: DbContext
    {
        public SchedulingEntities(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public virtual IEnumerable<Stop> GetStopsWithinProximity2(double originatingLatitude, double originatingLongitude, string unitOfMeasure, double? distance, string routeName)
        {
            var result = GetStopsWithinProximity(originatingLatitude, originatingLongitude, unitOfMeasure, distance, routeName);
            return result.Select(FromEntity);
        }

        public Stop FromEntity(GetStopsWithinProximity_Result item)
        {
            return new Stop
            {
                StopId = item.StopId,
                StopDescription = item.StopDescription,
                PlaceId = item.PlaceId,
                DistrictId = item.DistrictId,
                Id511 = item.Id511,
                IsPublic = item.IsPublic,
                Longitude = item.Longitude,
                Latitude = item.Latitude,
                AllowAlighting = item.AllowAlighting,
                AllowBoarding = item.AllowBoarding,
                Corner = item.Corner,
                IsBart = item.IsBart,
                IsTransitCenter = item.IsTransitCenter,
                IsInService = item.IsInService,
                IsGPSValidated = item.IsGPSValidated,
                Site = item.Site,
                AvaStatus = item.AvaStatus,
                AvaDescription = item.AvaDescription,
                Comment = item.Comment,
                LastSchedulerUpdate = item.LastSchedulerUpdate,
                FlagRoute = item.FlagRoute,
                ValidToDate = item.ValidToDate,
                AddUserId = item.AddUserId,
                AddDateTime = item.AddDateTime,
                UpdUserId = item.UpdUserId,
                UpdDateTime = item.UpdDateTime,
                SysRecNo = item.SysRecNo
            };
        }


        // when you get a "Member with the same signature...", comment out auto-generated function in partial class
        //public virtual ObjectResult<Stop> GetStopsWithinProximity2(Nullable<double> originatingLatitude, Nullable<double> originatingLongitude, string unitOfMeasure, Nullable<double> distance, string routeName)
        //{
        //    var originatingLatitudeParameter = originatingLatitude.HasValue ?
        //        new ObjectParameter("OriginatingLatitude", originatingLatitude) :
        //        new ObjectParameter("OriginatingLatitude", typeof(double));

        //    var originatingLongitudeParameter = originatingLongitude.HasValue ?
        //        new ObjectParameter("OriginatingLongitude", originatingLongitude) :
        //        new ObjectParameter("OriginatingLongitude", typeof(double));

        //    var unitOfMeasureParameter = unitOfMeasure != null ?
        //        new ObjectParameter("UnitOfMeasure", unitOfMeasure) :
        //        new ObjectParameter("UnitOfMeasure", typeof(string));

        //    var distanceParameter = distance.HasValue ?
        //        new ObjectParameter("Distance", distance) :
        //        new ObjectParameter("Distance", typeof(double));

        //    var routeNameParameter = routeName != null ?
        //        new ObjectParameter("RouteName", routeName) :
        //        new ObjectParameter("RouteName", typeof(string));

        //    return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Stop>("GetStopsWithinProximity", originatingLatitudeParameter, originatingLongitudeParameter, unitOfMeasureParameter, distanceParameter, routeNameParameter);
        //}

        //public virtual ObjectResult<Stop> GetStopsWithinProximity2(Nullable<double> originatingLatitude, Nullable<double> originatingLongitude, string unitOfMeasure, Nullable<double> distance, string routeName, MergeOption mergeOption)
        //{
        //    var originatingLatitudeParameter = originatingLatitude.HasValue ?
        //        new ObjectParameter("OriginatingLatitude", originatingLatitude) :
        //        new ObjectParameter("OriginatingLatitude", typeof(double));

        //    var originatingLongitudeParameter = originatingLongitude.HasValue ?
        //        new ObjectParameter("OriginatingLongitude", originatingLongitude) :
        //        new ObjectParameter("OriginatingLongitude", typeof(double));

        //    var unitOfMeasureParameter = unitOfMeasure != null ?
        //        new ObjectParameter("UnitOfMeasure", unitOfMeasure) :
        //        new ObjectParameter("UnitOfMeasure", typeof(string));

        //    var distanceParameter = distance.HasValue ?
        //        new ObjectParameter("Distance", distance) :
        //        new ObjectParameter("Distance", typeof(double));

        //    var routeNameParameter = routeName != null ?
        //        new ObjectParameter("RouteName", routeName) :
        //        new ObjectParameter("RouteName", typeof(string));

        //    return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Stop>("GetStopsWithinProximity", mergeOption, originatingLatitudeParameter, originatingLongitudeParameter, unitOfMeasureParameter, distanceParameter, routeNameParameter);
        //}
    }
}
