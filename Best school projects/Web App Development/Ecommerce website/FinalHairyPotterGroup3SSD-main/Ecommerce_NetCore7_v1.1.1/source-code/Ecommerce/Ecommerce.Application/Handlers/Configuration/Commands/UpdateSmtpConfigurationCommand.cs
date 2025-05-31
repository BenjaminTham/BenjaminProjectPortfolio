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

public class UpdateSmtpConfigurationCommand : IRequest<Response<string>>
{
    public string EmailFromName { get; set; }
    public string EmailFromEmail { get; set; }
    public string EmailUserName { get; set; }
    public string EmailPassword { get; set; }
    public string EmailHost { get; set; }
    public string EmailPort { get; set; }
}
public class UpdateSmtpConfigurationCommandHandler : IRequestHandler<UpdateSmtpConfigurationCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheManager _cacheManager;
    public UpdateSmtpConfigurationCommandHandler(IDataContext db, IMapper mapper, IMemoryCacheManager cacheManager)
    {
        _db = db;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }

    public async Task<Response<string>> Handle(UpdateSmtpConfigurationCommand request, CancellationToken cancellationToken)
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        var smtpConfigurationSerialize = JsonSerializer.Serialize(request);
        var getSmtpConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.SmtpConfiguration).FirstOrDefaultAsync();
        if (getSmtpConfiguration == null)
        {
            appConfiguration.Key = AppConfigurationType.SmtpConfiguration;
            appConfiguration.Value = smtpConfigurationSerialize;

            await _db.AppConfigurations.AddAsync(appConfiguration);
        }
        else
        {
            getSmtpConfiguration.Value = smtpConfigurationSerialize;
            _db.AppConfigurations.Update(getSmtpConfiguration);
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
