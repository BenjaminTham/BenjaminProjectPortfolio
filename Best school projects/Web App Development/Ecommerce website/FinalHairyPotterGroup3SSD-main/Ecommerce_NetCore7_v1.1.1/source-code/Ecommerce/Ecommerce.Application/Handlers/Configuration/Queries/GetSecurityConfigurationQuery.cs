using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Configuration.Queries;

public class GetSecurityConfigurationQuery : IRequest<SecurityConfigurationDto>
{
}
public class GetSecurityConfigurationQueryHandler : IRequestHandler<GetSecurityConfigurationQuery, SecurityConfigurationDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetSecurityConfigurationQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<SecurityConfigurationDto> Handle(GetSecurityConfigurationQuery request, CancellationToken cancellationToken)
    {
        var getSecurityConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.SecurityConfiguration).FirstOrDefaultAsync();
        SecurityConfigurationDto securityConfigurationDto = new SecurityConfigurationDto();
        if (getSecurityConfiguration != null)
        {
            securityConfigurationDto = JsonSerializer.Deserialize<SecurityConfigurationDto>(getSecurityConfiguration.Value);
        }
        return securityConfigurationDto;
    }

}
