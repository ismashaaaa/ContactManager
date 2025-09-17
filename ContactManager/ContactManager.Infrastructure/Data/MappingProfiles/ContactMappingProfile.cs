using AutoMapper;
using ContactManager.Application.DTOs.Contacts;
using ContactManager.Domain.Entities;
using ContactManager.Infrastructure.Data.Entities;

namespace ContactManager.Infrastructure.Data.MappingProfiles;

public class ContactMappingProfile : Profile
{
    public ContactMappingProfile()
    {
        CreateMap<Contact, ContactEntity>().ReverseMap();

        CreateMap<Contact, ContactDto>().ReverseMap();
        CreateMap<Contact, CreateContactDto>().ReverseMap();
        CreateMap<Contact, UpdateContactDto>().ReverseMap();
    }
}