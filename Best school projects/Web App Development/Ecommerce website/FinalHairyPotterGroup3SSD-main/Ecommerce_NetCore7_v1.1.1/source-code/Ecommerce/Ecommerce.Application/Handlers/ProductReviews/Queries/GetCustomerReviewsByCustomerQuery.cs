using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.ProductReviews.Queries;

public class GetCustomerReviewsByCustomerQuery : IRequest<List<ProductReviewDto>>
{
    public long CustomerId { get; set; }
}
public class GetCustomerReviewsByCustomerQueryQueryHandler : IRequestHandler<GetCustomerReviewsByCustomerQuery, List<ProductReviewDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;

    public GetCustomerReviewsByCustomerQueryQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<ProductReviewDto>> Handle(GetCustomerReviewsByCustomerQuery request, CancellationToken cancellationToken)
    {
        var customerReviews = await _db.CustomerReviews.Include(c => c.Product).Include(c => c.Customer)
            .Where(o => o.CustomerId == request.CustomerId).ToListAsync(cancellationToken);

        var customerReviewsDto = _mapper.Map<List<ProductReviewDto>>(customerReviews);
        return customerReviewsDto;
    }
}
