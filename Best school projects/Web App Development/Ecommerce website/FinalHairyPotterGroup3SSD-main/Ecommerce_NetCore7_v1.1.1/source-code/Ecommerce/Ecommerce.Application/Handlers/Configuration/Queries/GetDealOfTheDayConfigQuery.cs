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

public class GetDealOfTheDayConfigQuery : IRequest<DealOfTheDayDto>
{
}
public class GetDealOfTheDayConfigQueryHandler : IRequestHandler<GetDealOfTheDayConfigQuery, DealOfTheDayDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GetDealOfTheDayConfigQueryHandler(IDataContext db, IMapper mapper, IMediator mediator)
    {
        _db = db;
        _mapper = mapper;
        _mediator = mediator;
    }

    public async Task<DealOfTheDayDto> Handle(GetDealOfTheDayConfigQuery request, CancellationToken cancellationToken)
    {
        var config = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.DealOfTheDay).FirstOrDefaultAsync(cancellationToken);

        if (config?.Value == null) return new DealOfTheDayDto();
        var dealOfTheDay = JsonSerializer.Deserialize<DealOfTheDayDto>(config.Value);

        if (dealOfTheDay?.ProductId > 0)
        {
            var product = await _mediator.Send(new GetProductByIdQuery { Id = dealOfTheDay.ProductId }, cancellationToken);
            if (product?.Id > 0)
            {
                dealOfTheDay.ProductName = product.Name;
                dealOfTheDay.ProductSlug = product.Slug;
                dealOfTheDay.Category = product.CategoryName;
                dealOfTheDay.ProductImagePreview = product.ImagePreview;
            };

            if (dealOfTheDay?.Image != null)
            {
                dealOfTheDay.ImagePreview = await _db.Galleries
                    .Where(g => g.Id == dealOfTheDay.Image)
                    .Select(g => g.Name)
                    .FirstOrDefaultAsync(cancellationToken);
            }
        }

        return dealOfTheDay ?? new DealOfTheDayDto();
    }
}