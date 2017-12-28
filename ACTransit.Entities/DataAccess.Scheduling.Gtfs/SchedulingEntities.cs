using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using ACTransit.Entities.Scheduling;

//put the changes here!!
namespace ACTransit.DataAccess.Scheduling.Gtfs
{
    public partial class SchedulingGtfsEntities
    {

        // Note: this function in the main class is returning int and need to be deleted.
//        public virtual ObjectResult<HastusStop> GetStopsWithinProximity(Nullable<double> originatingLatitude, Nullable<double> originatingLongitude, string unitOfMeasure, Nullable<double> distance, string routeName)
//        {
//            var originatingLatitudeParameter = originatingLatitude.HasValue ?
//                new ObjectParameter("OriginatingLatitude", originatingLatitude) :
//                new ObjectParameter("OriginatingLatitude", typeof(double));
//
//            var originatingLongitudeParameter = originatingLongitude.HasValue ?
//                new ObjectParameter("OriginatingLongitude", originatingLongitude) :
//                new ObjectParameter("OriginatingLongitude", typeof(double));
//
//            var unitOfMeasureParameter = unitOfMeasure != null ?
//                new ObjectParameter("UnitOfMeasure", unitOfMeasure) :
//                new ObjectParameter("UnitOfMeasure", typeof(string));
//
//            var distanceParameter = distance.HasValue ?
//                new ObjectParameter("Distance", distance) :
//                new ObjectParameter("Distance", typeof(double));
//
//            var routeNameParameter = routeName != null ?
//                new ObjectParameter("RouteName", routeName) :
//                new ObjectParameter("RouteName", typeof(string));
//
//            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<HastusStop>("GetStopsWithinProximity", originatingLatitudeParameter, originatingLongitudeParameter, unitOfMeasureParameter, distanceParameter, routeNameParameter);
//        }
//
//        public virtual ObjectResult<HastusStop> GetStopsWithinProximity(Nullable<double> originatingLatitude, Nullable<double> originatingLongitude, string unitOfMeasure, Nullable<double> distance, string routeName, MergeOption mergeOption)
//        {
//            var originatingLatitudeParameter = originatingLatitude.HasValue ?
//                new ObjectParameter("OriginatingLatitude", originatingLatitude) :
//                new ObjectParameter("OriginatingLatitude", typeof(double));
//
//            var originatingLongitudeParameter = originatingLongitude.HasValue ?
//                new ObjectParameter("OriginatingLongitude", originatingLongitude) :
//                new ObjectParameter("OriginatingLongitude", typeof(double));
//
//            var unitOfMeasureParameter = unitOfMeasure != null ?
//                new ObjectParameter("UnitOfMeasure", unitOfMeasure) :
//                new ObjectParameter("UnitOfMeasure", typeof(string));
//
//            var distanceParameter = distance.HasValue ?
//                new ObjectParameter("Distance", distance) :
//                new ObjectParameter("Distance", typeof(double));
//
//            var routeNameParameter = routeName != null ?
//                new ObjectParameter("RouteName", routeName) :
//                new ObjectParameter("RouteName", typeof(string));
//
//            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<HastusStop>("GetStopsWithinProximity", mergeOption, originatingLatitudeParameter, originatingLongitudeParameter, unitOfMeasureParameter, distanceParameter, routeNameParameter);
//        }
//
//        public virtual ObjectResult<NearestStop_Result> NearestStop(Nullable<decimal> lat, Nullable<decimal> @long, Nullable<int> top, Nullable<bool> isPublic, Nullable<bool> allowAlighting, Nullable<bool> allowBoarding, string corner, Nullable<bool> isBart, Nullable<bool> isTransitCenter, Nullable<bool> isInService, Nullable<bool> isGPSValidated, Nullable<bool> avaStatus)
//        {
//            var latParameter = lat.HasValue ?
//                new ObjectParameter("Lat", lat) :
//                new ObjectParameter("Lat", typeof(decimal));
//
//            var longParameter = @long.HasValue ?
//                new ObjectParameter("Long", @long) :
//                new ObjectParameter("Long", typeof(decimal));
//
//            var topParameter = top.HasValue ?
//                new ObjectParameter("Top", top) :
//                new ObjectParameter("Top", typeof(int));
//
//            var isPublicParameter = isPublic.HasValue ?
//                new ObjectParameter("IsPublic", isPublic) :
//                new ObjectParameter("IsPublic", typeof(bool));
//
//            var allowAlightingParameter = allowAlighting.HasValue ?
//                new ObjectParameter("AllowAlighting", allowAlighting) :
//                new ObjectParameter("AllowAlighting", typeof(bool));
//
//            var allowBoardingParameter = allowBoarding.HasValue ?
//                new ObjectParameter("AllowBoarding", allowBoarding) :
//                new ObjectParameter("AllowBoarding", typeof(bool));
//
//            var cornerParameter = corner != null ?
//                new ObjectParameter("Corner", corner) :
//                new ObjectParameter("Corner", typeof(string));
//
//            var isBartParameter = isBart.HasValue ?
//                new ObjectParameter("IsBart", isBart) :
//                new ObjectParameter("IsBart", typeof(bool));
//
//            var isTransitCenterParameter = isTransitCenter.HasValue ?
//                new ObjectParameter("IsTransitCenter", isTransitCenter) :
//                new ObjectParameter("IsTransitCenter", typeof(bool));
//
//            var isInServiceParameter = isInService.HasValue ?
//                new ObjectParameter("IsInService", isInService) :
//                new ObjectParameter("IsInService", typeof(bool));
//
//            var isGPSValidatedParameter = isGPSValidated.HasValue ?
//                new ObjectParameter("IsGPSValidated", isGPSValidated) :
//                new ObjectParameter("IsGPSValidated", typeof(bool));
//
//            var avaStatusParameter = avaStatus.HasValue ?
//                new ObjectParameter("AvaStatus", avaStatus) :
//                new ObjectParameter("AvaStatus", typeof(bool));
//
//            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<NearestStop_Result>("NearestStop", latParameter, longParameter, topParameter, isPublicParameter, allowAlightingParameter, allowBoardingParameter, cornerParameter, isBartParameter, isTransitCenterParameter, isInServiceParameter, isGPSValidatedParameter, avaStatusParameter);
//        }
    }
}
