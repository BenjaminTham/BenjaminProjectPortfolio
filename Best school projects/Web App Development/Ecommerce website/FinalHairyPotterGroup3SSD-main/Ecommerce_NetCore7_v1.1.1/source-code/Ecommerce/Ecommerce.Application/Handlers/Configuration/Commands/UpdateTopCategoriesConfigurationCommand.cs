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

public class UpdateTopCategoriesConfigurationCommand : IRequest<Response<string>>
{
    public List<TopCategoriesConfigurationDto> TopCategories { get; set; }
}
public class UpdateTopCategoriesConfigurationCommandHandler : IRequestHandler<UpdateTopCategoriesConfigurationCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheManager _cacheManager;
    public UpdateTopCategoriesConfigurationCommandHandler(IDataContext db, IMapper mapper, IMemoryCacheManager cacheManager)
    {
        _db = db;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }

    public async Task<Response<string>> Handle(UpdateTopCategoriesConfigurationCommand request, CancellationToken cancellationToken)
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        List<TopCategoriesConfiguration> topCategoriesConfigurations = new List<TopCategoriesConfiguration>();
        var getTopCategories = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.TopCategoriesConfiguration).FirstOrDefaultAsync();

        if (getTopCategories == null)
        {
            appConfiguration.Key = AppConfigurationType.TopCategoriesConfiguration;
            appConfiguration.Value = JsonSerializer.Serialize(topCategoriesConfigurations);

            await _db.AppConfigurations.AddAsync(appConfiguration);
        }
        else
        {
            getTopCategories.Value = JsonSerializer.Serialize(_mapper.Map<List<TopCategoriesConfiguration>>(request.TopCategories));
            _db.AppConfigurations.Update(getTopCategories);
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
