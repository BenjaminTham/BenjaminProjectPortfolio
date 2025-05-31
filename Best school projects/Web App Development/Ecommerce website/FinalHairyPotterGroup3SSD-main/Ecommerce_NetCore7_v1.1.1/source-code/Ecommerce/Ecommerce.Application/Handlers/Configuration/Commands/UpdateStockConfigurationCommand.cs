using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Configuration.Commands;

public class UpdateStockConfigurationCommand : IRequest<Response<string>>
{
    public bool IsLowStockNotificationEnabled { get; set; }
    public bool IsOutOfStockNotificationEnabled { get; set; }
    public int? LowStockThreshold { get; set; }
    public int? OutOfStockThreshold { get; set; }
    public bool IsOutOfStockItemHidden { get; set; }
}
public class UpdateStockConfigurationCommandHandler : IRequestHandler<UpdateStockConfigurationCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheManager _cacheManager;
    public UpdateStockConfigurationCommandHandler(IDataContext db, IMapper mapper, IMemoryCacheManager cacheManager)
    {
        _db = db;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }

    public async Task<Response<string>> Handle(UpdateStockConfigurationCommand request, CancellationToken cancellationToken)
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        var stockConfigurationSerialize = JsonSerializer.Serialize(request);
        var getStockConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.StockConfiguration).FirstOrDefaultAsync();
        if (getStockConfiguration == null)
        {
            appConfiguration.Key = AppConfigurationType.StockConfiguration;
            appConfiguration.Value = stockConfigurationSerialize;

            await _db.AppConfigurations.AddAsync(appConfiguration);
        }
        else
        {
            getStockConfiguration.Value = stockConfigurationSerialize;
            _db.AppConfigurations.Update(getStockConfiguration);
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
