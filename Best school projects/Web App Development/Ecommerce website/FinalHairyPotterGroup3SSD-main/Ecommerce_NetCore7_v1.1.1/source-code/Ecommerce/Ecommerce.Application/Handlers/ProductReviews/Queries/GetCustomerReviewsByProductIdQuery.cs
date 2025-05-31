using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.ProductReviews.Queries;

public class GetCustomerReviewsByProductIdQuery : IRequest<List<ProductReviewDto>>
{
    public long ProductId { get; set; }
}
public class GetCustomerReviewsByProductIdQueryQueryHandler : IRequestHandler<GetCustomerReviewsByProductIdQuery, List<ProductReviewDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;

    public GetCustomerReviewsByProductIdQueryQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<ProductReviewDto>> Handle(GetCustomerReviewsByProductIdQuery request, CancellationToken cancellationToken)
    {
        var customerReviews = await _db.CustomerReviews.Include(c => c.Customer).ThenInclude(c=>c.User)
            .Where(o => o.ProductId == request.ProductId && o.IsActive == true).Select(c=> new ProductReviewDto
            {
                CustomerId = c.CustomerId,
                CustomerName = c.Customer.User.FullName,
                Rating = c.Rating,
                Comment = c.Comment,
                DateCommented = c.DateCommented,
                Reply = c.Reply,
            }).ToListAsync(cancellationToken);


        var customerReviewsDto = _mapper.Map<List<ProductReviewDto>>(customerReviews);
        return customerReviewsDto;
    }
}
