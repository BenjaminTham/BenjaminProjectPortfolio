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

public class UpdateGeneralConfigurationCommand : IRequest<Response<string>>
{
    public GeneralConfigurationDto generalConfiguration { get; set; }
}
public class UpdateGeneralConfigurationCommandHandler : IRequestHandler<UpdateGeneralConfigurationCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheManager _cacheManager;
    public UpdateGeneralConfigurationCommandHandler(IDataContext db, IMapper mapper, IMemoryCacheManager cacheManager)
    {
        _db = db;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }

    public async Task<Response<string>> Handle(UpdateGeneralConfigurationCommand request, CancellationToken cancellationToken)
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        GeneralConfiguration generalConfiguration = new GeneralConfiguration();
        var getGeneralConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.GeneralConfiguration).FirstOrDefaultAsync();
        if (getGeneralConfiguration == null)
        {
            appConfiguration.Key = AppConfigurationType.GeneralConfiguration;
            appConfiguration.Value = JsonSerializer.Serialize(generalConfiguration);

            await _db.AppConfigurations.AddAsync(appConfiguration);
        }
        else
        {
            var logoPreview = await _db.Galleries.Where(o => o.Id == request.generalConfiguration.CompanyLogo).Select(o => o.Name).FirstOrDefaultAsync();
            var faviconPreview = await _db.Galleries.Where(o => o.Id == request.generalConfiguration.CompanyFavicon).Select(o => o.Name).FirstOrDefaultAsync();
            if (logoPreview != null)
            {
                request.generalConfiguration.CompanyLogo = logoPreview;
            }
            if (faviconPreview != null)
            {
                request.generalConfiguration.CompanyFavicon = faviconPreview;
            }

            getGeneralConfiguration.Value = JsonSerializer.Serialize(_mapper.Map<GeneralConfiguration>(request.generalConfiguration));
            _db.AppConfigurations.Update(getGeneralConfiguration);
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
