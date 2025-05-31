using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Configuration.Commands;

public class UpdateBasicSeoConfigurationCommand : IRequest<Response<string>>
{
    public BasicSeoConfigurationDto basicSeoConfiguration { get; set; }
}
public class UpdateBasicSeoConfigurationCommandHandler : IRequestHandler<UpdateBasicSeoConfigurationCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheManager _cacheManager;
    public UpdateBasicSeoConfigurationCommandHandler(IDataContext db, IMapper mapper, IMemoryCacheManager cacheManager)
    {
        _db = db;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }

    public async Task<Response<string>> Handle(UpdateBasicSeoConfigurationCommand request, CancellationToken cancellationToken)
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        BasicSeoConfiguration basicSeoConfiguration = new BasicSeoConfiguration();
        var getBasicSeoConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.BasicSeoConfiguration).FirstOrDefaultAsync();
        if (getBasicSeoConfiguration == null)
        {
            appConfiguration.Key = AppConfigurationType.BasicSeoConfiguration;
            appConfiguration.Value = JsonSerializer.Serialize(basicSeoConfiguration);

            await _db.AppConfigurations.AddAsync(appConfiguration);
        }
        else
        {
            getBasicSeoConfiguration.Value = JsonSerializer.Serialize(_mapper.Map<BasicSeoConfiguration>(request.basicSeoConfiguration));
            _db.AppConfigurations.Update(getBasicSeoConfiguration);
        }


        try
        {
            await _db.SaveChangesAsync();
            _cacheManager.AppConfigurationRestore();
            return Response<string>.Success("Successfully updated");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to update");
        }

    }
}
