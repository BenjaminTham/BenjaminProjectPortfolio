using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Configuration.Queries;

public class GetBasicSeoConfigurationQuery : IRequest<BasicSeoConfigurationDto>
{
}
public class GetBasicSeoConfigurationQueryHandler : IRequestHandler<GetBasicSeoConfigurationQuery, BasicSeoConfigurationDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetBasicSeoConfigurationQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<BasicSeoConfigurationDto> Handle(GetBasicSeoConfigurationQuery request, CancellationToken cancellationToken)
    {
        var getBasicSeoConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.BasicSeoConfiguration).FirstOrDefaultAsync();
        BasicSeoConfigurationDto basicSeoConfiguration = new BasicSeoConfigurationDto();
        if (getBasicSeoConfiguration != null)
        {
            basicSeoConfiguration = JsonSerializer.Deserialize<BasicSeoConfigurationDto>(getBasicSeoConfiguration.Value);
        }
        return basicSeoConfiguration;
    }
}
