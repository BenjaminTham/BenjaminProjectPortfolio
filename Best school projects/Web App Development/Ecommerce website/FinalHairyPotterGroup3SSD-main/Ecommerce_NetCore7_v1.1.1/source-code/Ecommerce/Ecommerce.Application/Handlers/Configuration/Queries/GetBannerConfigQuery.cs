using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.RenderItems.Queries;
using Ecommerce.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using Ecommerce.Application.Handlers.Products.Queries;

namespace Ecommerce.Application.Handlers.Configuration.Queries;

public class GetBannerConfigQuery : IRequest<List<BannerDto>>
{
}
public class GetBannerConfigQueryHandler : IRequestHandler<GetBannerConfigQuery, List<BannerDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetBannerConfigQueryHandler(IDataContext db, IMapper mapper, IMediator mediator)
    {
        _db = db;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<List<BannerDto>> Handle(GetBannerConfigQuery request, CancellationToken cancellationToken)
    {
        var bannerConfig = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.BannerConfiguration).FirstOrDefaultAsync(cancellationToken);
        List<BannerDto> bannerList = new List<BannerDto>();

        if (bannerConfig != null)
        {
            bannerList = JsonSerializer.Deserialize<List<BannerDto>>(bannerConfig.Value)!;
        }
        return bannerList;
    }
}