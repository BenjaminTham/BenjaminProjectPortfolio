using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Configuration.Queries;

public class GetSocialConfigurationQuery : IRequest<SocialProfileDto>
{
}
public class GetSocialConfigurationQueryHandler : IRequestHandler<GetSocialConfigurationQuery, SocialProfileDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetSocialConfigurationQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<SocialProfileDto> Handle(GetSocialConfigurationQuery request, CancellationToken cancellationToken)
    {
        var getSocialProfile = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.SocialProfile).FirstOrDefaultAsync();
        SocialProfileDto socialProfile = new SocialProfileDto();
        if (getSocialProfile != null)
        {
            socialProfile = JsonSerializer.Deserialize<SocialProfileDto>(getSocialProfile.Value);
        }
        return socialProfile;
    }

}
