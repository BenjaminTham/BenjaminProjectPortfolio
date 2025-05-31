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

public class UpdateDealOfTheDayConfigCommand : IRequest<Response<string>>
{
    public DealOfTheDayDto DealOfTheDay { get; set; }
}
public class UpdateDealOfTheDayConfigCommandHandler : IRequestHandler<UpdateDealOfTheDayConfigCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheManager _cacheManager;
    public UpdateDealOfTheDayConfigCommandHandler(IDataContext db, IMapper mapper, IMemoryCacheManager cacheManager)
    {
        _db = db;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }


    public async Task<Response<string>> Handle(UpdateDealOfTheDayConfigCommand request, CancellationToken cancellationToken)
    {
        //var configKey = AppConfigurationType.DealOfTheDay;
        //var configValue = JsonSerializer.Serialize(
        //    _mapper.Map<List<HeaderSlider>>(request.DealOfTheDay));

        //var config = await _db.AppConfigurations.FindAsync(configKey, cancellationToken)
        //             ?? _db.AppConfigurations.Add(new AppConfiguration { Key = configKey }).Entity;

        //config.Value = configValue;
        //await _db.SaveChangesAsync(cancellationToken);

        //_cacheManager.AppConfigurationRestore();
        //return Response<string>.Success("Successfully updated");



        AppConfiguration appConfiguration = new AppConfiguration();
        DealOfTheDay dealOfTheDays = new DealOfTheDay();
        var getDealOfTheDayConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.DealOfTheDay).FirstOrDefaultAsync(cancellationToken);

        if (getDealOfTheDayConfiguration == null)
        {
            appConfiguration.Key = AppConfigurationType.DealOfTheDay;
            appConfiguration.Value = JsonSerializer.Serialize(dealOfTheDays);
            await _db.AppConfigurations.AddAsync(appConfiguration, cancellationToken);
        }
        else
        {
            getDealOfTheDayConfiguration.Value = JsonSerializer.Serialize(_mapper.Map<DealOfTheDay>(request.DealOfTheDay));
            _db.AppConfigurations.Update(getDealOfTheDayConfiguration);
        }

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            _cacheManager.AppConfigurationRestore();
            return Response<string>.Success("Successfully Updated");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to Update");
        }

    }
}
