using AutoMapper;
using htdc_api.Models;
using htdc_api.Models.Payloads;
using htdc_api.Models.ViewModel;

namespace htdc_api.Mapper;

public class MappingProfile: Profile
{
    public MappingProfile()
    {
        CreateMap<CreateProductsViewModel, Products>()
            .ReverseMap();

        CreateMap<UserProfile, PatientInformation>()
            .ForMember(d => d.UserId, 
                opt => opt.MapFrom(s => s.AspNetUserId));

        CreateMap<AppointmentInformation, AppointmentTable>();

        CreateMap<InquiryPayload, Inquiry>()
            .ForMember(d => d.Phone, opt => opt.MapFrom(s => s.MobileNumber));
    }
}