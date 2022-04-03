using AutoMapper;
using CampCode.Data.Entities;
using CampCode.Models;
using System;
using System.Collections.Generic;
using System.Text;


namespace Data
{
    class CampProfile : Profile
    {
        public CampProfile() {

            this.CreateMap<Camp, CampModel>()
                .ForMember(a => a.Venue, b => b.MapFrom(c => c.Location.VenueName))
                .ReverseMap();


            this.CreateMap<Talk, TalkModel>()
                 .ReverseMap()
                 .ForMember(t =>t.Camp, opt=>opt.Ignore())
                 .ForMember(t => t.Speaker, opt => opt.Ignore());


            this.CreateMap<Speaker, SpeakerModel>()
                .ReverseMap();
        }
        
            
    }
}
