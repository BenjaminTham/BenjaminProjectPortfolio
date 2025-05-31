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

public class UpdateSocialConfigurationCommand : IRequest<Response<string>>
{
    public string Facebook { get; set; }
    public string Youtube { get; set; }
    public string Twitter { get; set; }
    public string Instagram { get; set; }
    public string Pinterest { get; set; }
    public string Linkedin { get; set; }
}
public class UpdateSocialConfigurationCommandHandler : IRequestHandler<UpdateSocialConfigurationCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheManager _cacheManager;
    public UpdateSocialConfigurationCommandHandler(IDataContext db, IMapper mapper, IMemoryCacheManager cacheManager)
    {
        _db = db;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }

    public async Task<Response<string>> Handle(UpdateSocialConfigurationCommand request, CancellationToken cancellationToken)
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        var socialConfigurationSerialize = JsonSerializer.Serialize(request);
        var getSocialConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.SocialProfile).FirstOrDefaultAsync(cancellationToken);
        if (getSocialConfiguration == null)
        {
            appConfiguration.Key = AppConfigurationType.SocialProfile;
            appConfiguration.Value = socialConfigurationSerialize;

            await _db.AppConfigurations.AddAsync(appConfiguration, cancellationToken);
        }
        else
        {
            getSocialConfiguration.Value = socialConfigurationSerialize;
            _db.AppConfigurations.Update(getSocialConfiguration);
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
