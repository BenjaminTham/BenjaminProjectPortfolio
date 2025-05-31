using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Configuration.Queries;

public class GetAdvancedConfigurationQuery : IRequest<AdvancedConfigurationDto>
{
}
public class GetAdvancedConfigurationQueryHandler : IRequestHandler<GetAdvancedConfigurationQuery, AdvancedConfigurationDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetAdvancedConfigurationQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<AdvancedConfigurationDto> Handle(GetAdvancedConfigurationQuery request, CancellationToken cancellationToken)
    {
        var getAdvancedConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.AdvancedConfiguration).FirstOrDefaultAsync();
        AdvancedConfigurationDto advancedConfigurationDto = new AdvancedConfigurationDto();
        if (getAdvancedConfiguration != null)
        {
            advancedConfigurationDto = JsonSerializer.Deserialize<AdvancedConfigurationDto>(getAdvancedConfiguration.Value);
        }
        return advancedConfigurationDto;
    }

}
