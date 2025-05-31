using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Configuration.Queries;

public class GetSmtpConfigurationQuery : IRequest<SmtpConfigurationDto>
{
}
public class GetSmtpConfigurationQueryHandler : IRequestHandler<GetSmtpConfigurationQuery, SmtpConfigurationDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetSmtpConfigurationQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<SmtpConfigurationDto> Handle(GetSmtpConfigurationQuery request, CancellationToken cancellationToken)
    {
        var getSmtpConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.SmtpConfiguration).FirstOrDefaultAsync();
        SmtpConfigurationDto smtpConfigurationDto = new SmtpConfigurationDto();
        if (getSmtpConfiguration != null)
        {
            smtpConfigurationDto = JsonSerializer.Deserialize<SmtpConfigurationDto>(getSmtpConfiguration.Value);
        }
        return smtpConfigurationDto;
    }

}
