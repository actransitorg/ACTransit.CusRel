//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ACTransit.Entities.Transportation
{
    using System;
    using System.Collections.Generic;
    
    public partial class VehicleInfo
    {
        public Nullable<int> vehicle_id { get; set; }
        public Nullable<bool> logon_state { get; set; }
        public Nullable<bool> ignition_state { get; set; }
        public Nullable<byte> primary_data_channel_id { get; set; }
        public Nullable<byte> secondary_data_channel_id { get; set; }
        public Nullable<bool> gps_failed_state { get; set; }
        public Nullable<int> incident_log_id { get; set; }
        public string vehicle_identification_nbr { get; set; }
        public string license_nbr { get; set; }
        public short model_year { get; set; }
        public short passenger_capacity { get; set; }
        public bool wheel_chair_access { get; set; }
        public bool gps_equipped { get; set; }
        public bool camera_equipped { get; set; }
        public byte vehicle_make_id { get; set; }
        public byte garage_id { get; set; }
        public string VehicleTypeDesc { get; set; }
        public byte VehicleTypeActive { get; set; }
    }
}
