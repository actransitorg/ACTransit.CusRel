using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using ACTransit.Contracts.Data.Common.PublicSite;
using ACTransit.Contracts.Data.Schedules.PublicSite;
using ACTransit.Framework.Algorithm.Sort;
using sch_lines = ACTransit.Entities.MapsSchedules.sch_lines;
using sch_schedule_versions = ACTransit.Entities.MapsSchedules.sch_schedule_versions;

namespace ACTransit.DataAccess.MapsSchedules.Repositories
{
    public class LinesRepository : IDisposable
    {
        private readonly MapsEntities context;
        public string ErrorMessage { get; private set; }

        //public int CurrentVersionId { get; private set; }

        public LinesRepository(MapsEntities context)
        {
            this.context = context;
        }

        public LinesRepository(MapsEntities context, int currentVersion)
        {
            this.context = context;
        }

        // =============================================================

        #region Bookkeeping

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed && disposing)
                context.Dispose();
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        // =============================================================

        #region Lookup Tables

        public async Task<sch_schedule_versions> CurrentVersion(DateTime? StartDate = null)
        {
            try
            {
                return await (
                    from sv in context.sch_schedule_versions
                    where sv.version_start_date <= (StartDate ?? DateTime.Now)
                    orderby sv.version_start_date descending
                    select sv).FirstOrDefaultAsync();
            }
            catch (EntityException e)
            {
                ErrorMessage = e.Message;
                throw;
            }
        }

        public async Task<sch_schedule_versions> NextVersion(DateTime? StartDate = null)
        {
            var version = await CurrentVersion(StartDate);
            return await (
                from sv in context.sch_schedule_versions
                where sv.version_id - 1 == version.version_id
                select sv).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DayCode>> DayCodes()
        {
            var codes = context.sch_day_codes.OrderBy(i => i.sort_order).ToListAsync();
            var codes2 = codes.ContinueWith(c => c.Result.Select(item => new DayCode
            {
                KeyValue = new KeyValuePair<string, string>(item.day_code, item.adjectives),
            }).ToList());
            return await codes2;
        }

        public async Task<IEnumerable<DirectionCode>> DirectionCodes()
        {
            var codes = context.sch_direction_codes.ToListAsync();
            var codes2 = codes.ContinueWith(c => c.Result.Select(item => new DirectionCode
            {
                KeyValue = new KeyValuePair<string, string>(item.direction_code, item.direction_names),
                Description = item.direction_replace
            }).ToList());
            return await codes2;
        }

        #endregion

        // =============================================================

        #region Categories and Common Types

        public async Task<FeatureSettings> FeatureSettings(RequestState RequestState)
        {
            var version = await CurrentVersion();
            return await Task.Run(() => new FeatureSettings
            {
                Version = version.version_id,
                MainSiteUrl = RequestState.MainSiteUrl
            });
        }

        public async Task<LineCategories> CategoryLines(string id)
        {
            var categories = new LineCategories();
            var lines = await Lines();
            if (id != null)
            {
                var category = categories.FirstOrDefault(c => c.KeyValue.Value == id);
                if (category == null) return categories;
                category.Lines = lines
                    .Where(l => l.CategoryKey.GetValueOrDefault() == category.KeyValue.Key && !string.IsNullOrEmpty(l.KeyValue.Value))
                    .OrderBy(x => x.KeyValue.Value, new AlphaNumericComparer()).ToList();
                for (var i = categories.Count - 1; i >= 0; i--)
                {
                    if (categories[i].KeyValue.Key != category.KeyValue.Key)
                        categories.Remove(categories[i]);
                }
            }
            else
            {
                foreach (var category in categories.Where(category => category.Lines == null && category.IsEnabled))
                {
                    category.Lines = lines
                        .Where(l => l.CategoryKey.GetValueOrDefault() == category.KeyValue.Key)
                        .OrderBy(x => x.KeyValue.Value, new AlphaNumericComparer()).ToList();
                }

            }

            if (id != null && id != "All Lines") return categories;

            // add everything to 'All Lines' category
            var allLineCategory = (from c in categories
                                   where c.KeyValue.Value == "All Lines"
                                   select c).FirstOrDefault();
            if (allLineCategory != null)
                allLineCategory.Lines =
                    lines.Where(l => !string.IsNullOrEmpty(l.KeyValue.Value))
                        .OrderBy(x => x.KeyValue.Value, new AlphaNumericComparer())
                        .ToList();

            return categories;
        }

        #endregion

        // =============================================================

        #region Lines and Routes

        public async Task<List<Line>> Lines(DateTime? StartDate = null)
        {
            var version = await CurrentVersion(StartDate);
            var lines = context.sch_lines.Where(l => l.version_id == version.version_id).ToListAsync();
            return await lines.ContinueWith(l => l.Result.Select(item => new Line
            {
                KeyValue = new KeyValuePair<int, string>(item.line_id, item.line_name),
                CategoryKey = item.category_id
            }).OrderBy(x => x.KeyValue.Value, new AlphaNumericComparer()).ToList());
        }

        public async Task<Line> GetLine(string LineValue)
        {
            if (LineValue == null)
                throw new ValidationException("LineValue must be a valid Line/Route.");
            var version = await CurrentVersion();
            var line = context.sch_lines.FirstOrDefaultAsync(l => l.version_id == version.version_id && l.line_name == LineValue);
            return await line.ContinueWith(l =>
            {
                if (l.Result == null) return null;
                return new Line
                {
                    KeyValue = new KeyValuePair<int, string>(l.Result.line_id, l.Result.line_name),
                    CategoryKey = l.Result.category_id
                };
            });
        }

        public async Task<Line> LineRoute(int LineKey, DateTime? StartDate = null)
        {
            if (LineKey <= 0)
                throw new ValidationException("LineKey must be greater than zero.");
            var line = await LinesRoutes(LineKey, null, StartDate);
            return line.FirstOrDefault();
        }

        public async Task<Line> LineRoute(string LineValue, DateTime? StartDate = null)
        {
            if (LineValue == null)
                throw new ValidationException("LineValue must be a valid Line/Route.");
            var line = await LinesRoutes(0, LineValue, StartDate);
            return line.FirstOrDefault();
        }

        public async Task<List<Line>> LinesRoutes(int LineKey = 0, string LineValue = null, DateTime? StartDate = null)
        {
            var version = await CurrentVersion(StartDate);
            var linesInfo = await (
                from l in context.sch_lines
                where l.version_id == version.version_id
                      && (LineKey <= 0 || l.line_id == LineKey)
                      && (LineValue == null || l.line_name == LineValue)
                join sl in context.sch_schedule_lines
                    on l.line_id equals sl.line_id
                join s in context.sch_schedule
                    on sl.schedule_id equals s.schedule_id
                join dc in context.sch_day_codes
                    on s.day_code equals dc.day_code
                join drc in context.sch_direction_codes
                    on s.direction_code equals drc.direction_code
                orderby l.line_name, dc.sort_order, drc.direction_names
                select new {l, s, dc, drc}).ToListAsync();

            var sortOrder = 0;
            var list = new List<Line>();
            var lines = linesInfo.Select(li => li.l).Distinct().ToList();

            foreach (var line in lines)
            {
                sortOrder++;
                var newLine = new Line
                {
                    KeyValue = new KeyValuePair<int, string>(line.line_id, line.line_name),
                    Routes = new List<Route>(),
                    CategoryKey = line.category_id
                };

                var line1 = line;
                var routes = (
                    from li in linesInfo
                    where li.l == line1
                    select new
                    {
                        s = li.s,
                        dc = li.dc,
                        drc = li.drc
                    }).ToList();

                foreach (var route in routes)
                {
                    newLine.Routes.Add(new Route
                    {
                        KeyValue =
                            new KeyValuePair<int, string>(route.s.schedule_id,
                                route.drc.direction_replace.Replace(",", "")),
                        DirectionCode = new DirectionCode
                        {
                            KeyValue = new KeyValuePair<string, string>(route.s.direction_code, route.drc.direction_names),
                            Description = route.drc.direction_replace.Replace(",", "")
                        },
                        DayCode = new DayCode
                        {
                            KeyValue = new KeyValuePair<string, string>(route.s.day_code, route.dc.adjectives),
                            Description = route.dc.longer_names
                        },
                        SortOrder = sortOrder
                    });
                    sortOrder++;
                }
                list.Add(newLine);
            }

            return list;
        }

        public List<Line> AllLines()
        {
            var lines = context.sch_lines.Where(l => !string.IsNullOrEmpty(l.line_name)).GroupBy(l => l.line_name).OrderBy(l => l.Key).Select(l => l.FirstOrDefault()).ToList();
            return lines.Select(line => new Line
            {
                KeyValue = new KeyValuePair<int, string>(line.line_id, line.line_name),
                Routes = null,
                CategoryKey = line.category_id
            }).ToList();
        }

        #endregion

        // =============================================================

        #region Stops, Trips and Schedules

        public async Task<Line> FillStopsAsync(Line Line)
        {
            return await Task.FromResult(FillStops(Line));
        }

        public Line FillStops(Line Line)
        {
            if (Line == null || Line.Routes == null)
                throw new ArgumentNullException("Line cannot be null.");

            var sch_id = (from sch in Line.Routes select (int?)sch.KeyValue.Key).ToList();

            var stopPoints = (
                from t in context.sch_timepoints
                where (from s in sch_id select s).Contains(t.schedule_id)
                select t).ToList();

            var stops =
                (from t in stopPoints
                 from sch in Line.Routes
                 where t.schedule_id == sch.KeyValue.Key
                 orderby sch.SortOrder, t.sort_order
                 select new
                 {
                     schedule_id = t.schedule_id,
                     key = t.timepoint_id,
                     value = t.timepoint_name,
                     code = t.timepoint_code,
                     city = t.city
                 }).ToList();

            if (!stops.Any()) return Line;

            foreach (var route in Line.Routes)
            {
                route.Stops = new List<Stop>();
                var route1 = route;
                var routeStops =
                    from s in stops
                    where s.schedule_id == route1.KeyValue.Key
                    select s;
                foreach (var stop in routeStops)
                {
                    route.Stops.Add(new Stop
                    {
                        KeyValue = new KeyValuePair<int, string>(stop.key, stop.value),
                        Code = stop.code,
                        City = stop.city
                    });
                }
            }

            return Line;
        }

        public async Task<Line> FillTripsAsync(Line Line)
        {
            return await Task.FromResult(FillTrips(Line));
        }

        public Line FillTrips(Line Line)
        {
            if (Line == null || Line.Routes == null)
                throw new ArgumentNullException();

            if (Line.Routes.Any(r => r.Stops == null))
                Line = FillStops(Line);

            var trips = (
                from tr in context.sch_trips
                where tr.line_id == Line.KeyValue.Key
                select tr).ToList();

            var stopTrips = (
                from sch in Line.Routes
                from stop in sch.Stops
                from tr in trips
                where tr.timepoint_id == stop.KeyValue.Key
                select new
                {
                    tr = tr,
                    stop = stop,
                    sch = sch
                }).ToList();

            if (!trips.Any()) return Line;

            // populate stop times
            foreach (var stop in Line.Routes.SelectMany(route => route.Stops))
            {
                stop.StopTimes = new List<StopTime>();
                var stop1 = stop;
                var stopTimes =
                    from t in stopTrips
                    where t.tr.timepoint_id == stop1.KeyValue.Key
                    select t;
                foreach (var time in stopTimes)
                {
                    var atTime = time.tr.stop_time.ToDateTime();
                    if (atTime != null)
                        stop.StopTimes.Add(new StopTime
                        {
                            Time = atTime,
                            TripId = time.tr.trip_id
                        });
                }
            }

            // populate trips
            foreach (var route in Line.Routes)
            {
                var tripsInSchedule = (from t in stopTrips
                                       where t.sch == route
                                       orderby t.tr.trip_id
                                       select t).ToList();
                var lastSortOrder = -1;
                route.Trips = new List<Trip>();
                var trip = new Trip();
                foreach (var stop in tripsInSchedule)
                {
                    var sortOrder = stop.tr.sort_order.GetValueOrDefault();
                    if (lastSortOrder > sortOrder)  // only way to detect trip deltas is in sort order reset (trip_id is linear)
                    {
                        route.Trips.Add(trip);
                        trip = new Trip();
                    }
                    trip.KeyValue = new KeyValuePair<int, string>(stop.tr.trip_id, stop.sch.KeyValue.Value);
                    var newStop = new Stop
                    {
                        KeyValue = stop.stop.KeyValue,
                        Code = stop.stop.Code,
                        City = stop.stop.City,
                        StopTimes = new List<StopTime>()
                        {
                            new StopTime
                            {
                                Time = stop.tr.stop_time.ToDateTime(),
                                TripId = stop.tr.trip_id
                            }
                        }
                    };
                    trip.Stops.Add(newStop);
                    lastSortOrder = sortOrder;
                }
                route.Trips.Add(trip);
            }
            return Line;

        }

        public async Task<Line> FillSchedulesAsync(Line Line)
        {
            return await Task.FromResult(FillSchedules(Line));
        }

        public Line FillSchedules(Line Line)
        {
            if (Line == null || Line.Routes == null)
                throw new ArgumentNullException();
            if (Line.Routes.Any(r => r.Trips == null))
                Line = FillTrips(Line);

            foreach (var route in Line.Routes)
            {
                var table = new DataTable(route.KeyValue.Value);

                // get column list
                var columnNames = new List<string>();
                foreach (var stop in route.Stops)
                {
                    var key = stop.KeyValue.Value;
                    var columnDuplicate = columnNames.Contains(key);
                    if (columnDuplicate)
                    {
                        var col1 = key + " (arrival)";
                        var idx = columnNames.IndexOf(key);
                        key += " (departure)";
                        columnNames.RemoveAt(idx);
                        columnNames.Insert(idx, col1);

                    }
                    columnNames.Add(key);
                }

                // set table columns
                foreach (var column in columnNames)
                    table.Columns.Add(column);

                // add rows and data
                foreach (var trip in route.Trips)
                {
                    var row = table.NewRow();
                    var idx = 0;
                    foreach (var stop in trip.Stops)
                    {
                        var stopTime = stop.StopTimes.FirstOrDefault();
                        var value = "-";
                        if (stopTime != null && stopTime.Time.HasValue)
                            value = stopTime.Time.GetValueOrDefault().ToString("hmmt").ToLower();
                        row[idx] = value;
                        idx++;
                    }
                    table.Rows.Add(row);
                }
                route.Schedule = table;
            }
            return Line;
        }

        public Line ClearStops(Line Line)
        {
            if (Line == null)
                throw new ArgumentNullException();
            foreach (var route in Line.Routes)
                route.Stops = null;
            return Line;
        }

        public Line ClearTrips(Line Line)
        {
            if (Line == null)
                throw new ArgumentNullException();
            foreach (var route in Line.Routes)
                route.Trips = null;
            return Line;
        }

        public async Task<Line> GetOnlySchedule(string LineValue)
        {
            var line = await LineRoute(LineValue);
            return await Task.FromResult(ClearTrips(ClearStops(FillSchedules(line))));
        }

        #endregion

    }
}
