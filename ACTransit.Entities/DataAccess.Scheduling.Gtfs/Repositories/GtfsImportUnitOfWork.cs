﻿using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using ACTransit.Framework.Infrastructure;

namespace ACTransit.DataAccess.Scheduling.Gtfs
{
    public class GtfsImportUnitOfWork : SchedulingGtfsUnitOfWork
    {
        /*  
            NOTE: 
                Gtfs Import functionality is using EntityFramework's context only for the Connection String.
                The imports take quite awhile since they deal with such large quantities of records, so we 
                are using ADO.NET's BulkCopy functionality to significantly speed up the import process.
          
                The remainder of the UnitOfWork will use the context as expected.
         */

        private const int MaxTimeoutInSeconds = 300;

        private SqlConnection _connection;
        protected SqlConnection Connection
        {
            get
            {
                if (_connection == null)
                    _connection = new SqlConnection(Context.Database.Connection.ConnectionString);

                if (_connection.State == ConnectionState.Closed)
                    _connection.Open();

                return _connection;
            }
        }

        public void DeleteGTFSDataForBooking(string bookingId)
        {
            using (var cmd = new SqlCommand("EXEC [dbo].[DeleteSignupData] @BookingId", Connection))
            {
                cmd.CommandTimeout = MaxTimeoutInSeconds; //This functionality may take awhile, increase the max timeout.
                cmd.Parameters.AddWithValue("@BookingId", bookingId);
                cmd.ExecuteNonQuery();
            }
        }

        public void ImportAgency(DataTable data)
        {
            using (var copy = new SqlBulkCopy(Connection))
            {
                copy.DestinationTableName = "dbo.Agency";
                copy.BulkCopyTimeout = MaxTimeoutInSeconds;
                copy.WriteToServer(data);
            }
        }

        public void ImportCalendar(DataTable data)
        {
            using (var copy = new SqlBulkCopy(Connection))
            {
                copy.DestinationTableName = "dbo.Calendar";
                copy.BulkCopyTimeout = MaxTimeoutInSeconds;
                copy.WriteToServer(data);
            }
        }

        public void ImportCalendarDate(DataTable data)
        {
            using (var copy = new SqlBulkCopy(Connection))
            {
                copy.DestinationTableName = "dbo.CalendarDate";
                copy.BulkCopyTimeout = MaxTimeoutInSeconds;
                copy.WriteToServer(data);
            }

        }

        public void ImportRoute(DataTable data)
        {
            using (var copy = new SqlBulkCopy(Connection))
            {
                copy.DestinationTableName = "dbo.Route";
                copy.BulkCopyTimeout = MaxTimeoutInSeconds;
                copy.WriteToServer(data);
            }

        }

        public void ImportShape(DataTable data)
        {
            using (var copy = new SqlBulkCopy(Connection))
            {
                copy.DestinationTableName = "dbo.Shape";
                copy.BulkCopyTimeout = MaxTimeoutInSeconds;
                copy.WriteToServer(data);
            }
        }

        public void ImportTrip(DataTable data)
        {
            using (var copy = new SqlBulkCopy(Connection))
            {
                copy.DestinationTableName = "dbo.Trip";
                copy.BulkCopyTimeout = MaxTimeoutInSeconds;
                copy.WriteToServer(data);
            }
        }

        public void ImportStop(DataTable data)
        {
            using (var copy = new SqlBulkCopy(Connection))
            {
                copy.DestinationTableName = "dbo.Stop";
                copy.BulkCopyTimeout = MaxTimeoutInSeconds;
                copy.WriteToServer(data);
            }
        }

        public void ImportStopTime(DataTable data)
        {
            using (var copy = new SqlBulkCopy(Connection))
            {
                copy.DestinationTableName = "dbo.StopTime";
                copy.BulkCopyTimeout = MaxTimeoutInSeconds; //This is a large import, increase the max timeout.
                copy.WriteToServer(data);
            }
        }

        public void GenerateTripIdMap(string bookingId)
        {
            using (var cmd = new SqlCommand("EXEC dbo.GenerateTripIdMap @BookingId", Connection))
            {
                cmd.Parameters.AddWithValue("@BookingId", bookingId);
                cmd.CommandTimeout = MaxTimeoutInSeconds;
                cmd.ExecuteNonQuery();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connection != null)
                {
                    if (_connection.State != ConnectionState.Closed)
                        _connection.Close();

                    _connection.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}