using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Configuration.Commands;

public class UpdateFeatureProductConfigurationCommand : IRequest<Response<string>>
{
    public IList<int> ProductId { get; set; }
}
public class UpdateFeatureProductConfigurationCommandHandler : IRequestHandler<UpdateFeatureProductConfigurationCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheManager _cacheManager;
    public UpdateFeatureProductConfigurationCommandHandler(IDataContext db, IMapper mapper, IMemoryCacheManager cacheManager)
    {
        _db = db;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }

    public async Task<Response<string>> Handle(UpdateFeatureProductConfigurationCommand request, CancellationToken cancellationToken)
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        List<FeatureProductConfiguration> featureProductConfigurations = new List<FeatureProductConfiguration>();
        var getFeatureProduct = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.FeatureProductConfiguration).FirstOrDefaultAsync();

        if (getFeatureProduct == null)
        {
            appConfiguration.Key = AppConfigurationType.FeatureProductConfiguration;
            appConfiguration.Value = JsonSerializer.Serialize(featureProductConfigurations);

            await _db.AppConfigurations.AddAsync(appConfiguration);
        }
        else
        {
            var order = 0;
            foreach (var item in request.ProductId)
            {
                order++;
                featureProductConfigurations.Add(new FeatureProductConfiguration { ProductId = item, Order = order });
            }
            getFeatureProduct.Value = JsonSerializer.Serialize(featureProductConfigurations);
            _db.AppConfigurations.Update(getFeatureProduct);
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
