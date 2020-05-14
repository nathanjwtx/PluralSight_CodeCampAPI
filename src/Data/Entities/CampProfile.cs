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
            
            // by putting ForMember after ReverseMap that tells AutoMapper to ignore the Camp when mapping from TalkModel to Talk
            // ie mapping is included when going from Talk to TalkModel
            CreateMap<Talk, TalkModel>()
                .ReverseMap()
                .ForMember(t => t.Camp, opt => opt.Ignore())
                .ForMember(t => t.Speaker, opt => opt.Ignore());

            CreateMap<Speaker, SpeakerModel>().ReverseMap();
        }
    }
}