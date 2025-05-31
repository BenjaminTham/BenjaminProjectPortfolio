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

public class UpdateAdvancedConfigurationCommand : IRequest<Response<string>>
{
    public bool EnableTwoFactorAuthentication { get; set; }
    public bool ActiveResetPassword { get; set; }
    public bool EnableEmailConfirmation { get; set; }
    public bool IsComingSoonEnabled { get; set; }
    public string RoleName { get; set; }
}
public class UpdateAdvancedConfigurationCommandHandler : IRequestHandler<UpdateAdvancedConfigurationCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheManager _cacheManager;
    public UpdateAdvancedConfigurationCommandHandler(IDataContext db, IMapper mapper, IMemoryCacheManager cacheManager)
    {
        _db = db;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }

    public async Task<Response<string>> Handle(UpdateAdvancedConfigurationCommand request, CancellationToken cancellationToken)
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        var advancedConfigurationSerialize = JsonSerializer.Serialize(request);
        var getAdvancedConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.AdvancedConfiguration).FirstOrDefaultAsync();
        if (getAdvancedConfiguration == null)
        {
            appConfiguration.Key = AppConfigurationType.AdvancedConfiguration;
            appConfiguration.Value = advancedConfigurationSerialize;

            await _db.AppConfigurations.AddAsync(appConfiguration, cancellationToken);
        }
        else
        {
            getAdvancedConfiguration.Value = advancedConfigurationSerialize;
            _db.AppConfigurations.Update(getAdvancedConfiguration);
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
