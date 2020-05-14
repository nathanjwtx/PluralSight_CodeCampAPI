using AutoMapper;
using CoreCodeCamp.Models;

namespace CoreCodeCamp.Data.Entities
{
    public class CampProfile : Profile
    {
        public CampProfile()
        {
            CreateMap<Camp, CampModel>()
                .ForMember(c => c.Venue,
                    o => o.MapFrom(m => m.Location.VenueName))
                .ReverseMap();
            CreateMap<Talk, TalkModel>()
                .ReverseMap();
            CreateMap<Speaker, SpeakerModel>().ReverseMap();
        }
    }
}