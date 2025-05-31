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

public class UpdateBannerConfigCommand : IRequest<Response<string>>
{
    public IList<BannerDto> Banners { get; set; }
}

public class UpdateBannerConfigCommandHandler : IRequestHandler<UpdateBannerConfigCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheManager _cacheManager;

    public UpdateBannerConfigCommandHandler(IDataContext db, IMapper mapper, IMemoryCacheManager cacheManager)
    {
        _db = db;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }


    public async Task<Response<string>> Handle(UpdateBannerConfigCommand request, CancellationToken cancellationToken)
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        List<Banner> banner = new List<Banner>();
        var getBannerConfiguration = await _db.AppConfigurations
            .Where(o => o.Key == AppConfigurationType.BannerConfiguration).FirstOrDefaultAsync(cancellationToken);

        if (getBannerConfiguration == null)
        {
            appConfiguration.Key = AppConfigurationType.BannerConfiguration;
            appConfiguration.Value = JsonSerializer.Serialize(banner);

            await _db.AppConfigurations.AddAsync(appConfiguration);
        }
        else
        {
            getBannerConfiguration.Value = JsonSerializer.Serialize(_mapper.Map<List<Banner>>(request.Banners));
            _db.AppConfigurations.Update(getBannerConfiguration);
        }

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            _cacheManager.AppConfigurationRestore();
            return Response<string>.Success("Successfully updated");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to update");
        }

    }
}