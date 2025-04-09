using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        AddGlobalIgnore("Events");
    }
}
