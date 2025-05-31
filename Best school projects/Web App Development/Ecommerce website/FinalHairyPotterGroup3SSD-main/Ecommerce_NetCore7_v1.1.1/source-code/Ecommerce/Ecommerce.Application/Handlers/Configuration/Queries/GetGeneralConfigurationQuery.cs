using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Configuration.Queries;

public class GetGeneralConfigurationQuery : IRequest<GeneralConfigurationDto>
{
}
public class GetGeneralConfigurationQueryHandler : IRequestHandler<GetGeneralConfigurationQuery, GeneralConfigurationDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetGeneralConfigurationQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<GeneralConfigurationDto> Handle(GetGeneralConfigurationQuery request, CancellationToken cancellationToken)
    {
        var getGeneralConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.GeneralConfiguration).FirstOrDefaultAsync(cancellationToken);
        GeneralConfigurationDto generalConfiguration = new GeneralConfigurationDto();
        if (getGeneralConfiguration != null)
        {
            generalConfiguration = JsonSerializer.Deserialize<GeneralConfigurationDto>(getGeneralConfiguration.Value);
            generalConfiguration.CompanyLogoPreview = generalConfiguration.CompanyLogo;
            generalConfiguration.CompanyFaviconPreview = generalConfiguration.CompanyFavicon;
        }
        return generalConfiguration;
    }
}
