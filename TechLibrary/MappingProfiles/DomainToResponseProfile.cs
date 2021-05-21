using AutoMapper;
using System.Collections.Generic;
using TechLibrary.Domain;
using TechLibrary.Models;

namespace TechLibrary.MappingProfiles
{
    public class DomainToResponseProfile : Profile
    {
        public DomainToResponseProfile()
        {
            CreateMap<Book, BookResponse>().ForMember(x => x.Descr, opt => opt.MapFrom(src => src.ShortDescr));
            CreateMap<PaginatedList<Book>, PaginatedListResponse<BookResponse>>().ForMember(x => x.Items, opt => opt.MapFrom(src => (List<Book>)src));
        }
    }
}