using System;
using System.Collections.Generic;
using CoreCodeCamp.Data;

namespace CoreCodeCamp.Models
{
    public class CampModel
    {
        public string Name { get; set; }
        public string Moniker { get; set; }
        public DateTime EventDate { get; set; } = DateTime.MinValue;
        public int Length { get; set; } = 1;
        
        // AutoMapper automatically maps properties if prefaced with the entity name
        // see Reverse Mapping on documentation page
        // if these automatic names aren't sufficient they can be customised in the profile
        public string Venue { get; set; }
        public string LocationAddress1 { get; set; }
        public string LocationAddress2 { get; set; }
        public string LocationAddress3 { get; set; }
        public string LocationCityTown { get; set; }
        public string LocationStateProvince { get; set; }
        public string LocationPostalCode { get; set; }
        public string LocationCountry { get; set; }
        
        public ICollection<TalkModel> Talks { get; set; }
    }
}