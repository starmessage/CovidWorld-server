﻿namespace Sygic.Corona.Contracts.Requests
{
    public class CreateProfileRequest
    {
        public string DeviceId { get; set; }
        public string PhoneNumber { get; set; }
        public string PushToken { get; set; }
        public string Locale { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Accuracy { get; set; }
    }
}
