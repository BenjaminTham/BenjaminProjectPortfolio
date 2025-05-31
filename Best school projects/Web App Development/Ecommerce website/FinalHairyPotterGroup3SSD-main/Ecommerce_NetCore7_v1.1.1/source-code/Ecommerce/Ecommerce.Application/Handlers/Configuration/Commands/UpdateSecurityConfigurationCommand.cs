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

public class UpdateSecurityConfigurationCommand : IRequest<Response<string>>
{
    public bool IsPasswordRequireDigit { get; set; }
    public bool IsPasswordRequireLowercase { get; set; }
    public bool IsPasswordRequireUppercase { get; set; }
    public bool IsPasswordRequireNonAlphanumeric { get; set; }
    public int PasswordRequiredLength { get; set; }
    public bool IsUserLockoutEnabled { get; set; }
    public int MaxFailedAccessAttempts { get; set; }
    public int UsertLockoutTime { get; set; }
}
public class UpdateSecurityConfigurationCommandHandler : IRequestHandler<UpdateSecurityConfigurationCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheManager _cacheManager;
    public UpdateSecurityConfigurationCommandHandler(IDataContext db, IMapper mapper, IMemoryCacheManager cacheManager)
    {
        _db = db;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }

    public async Task<Response<string>> Handle(UpdateSecurityConfigurationCommand request, CancellationToken cancellationToken)
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        var securityConfigurationSerialize = JsonSerializer.Serialize(request);
        var getSecurityConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.SecurityConfiguration).FirstOrDefaultAsync(cancellationToken);
        if (getSecurityConfiguration == null)
        {
            appConfiguration.Key = AppConfigurationType.SecurityConfiguration;
            appConfiguration.Value = securityConfigurationSerialize;

            await _db.AppConfigurations.AddAsync(appConfiguration, cancellationToken);
        }
        else
        {
            getSecurityConfiguration.Value = securityConfigurationSerialize;
            _db.AppConfigurations.Update(getSecurityConfiguration);
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
