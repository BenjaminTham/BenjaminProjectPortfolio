using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Ecommerce.Application.Handlers.ProductReviews.Queries;

public class GetCustomerReviewsWithPagingQuery : IRequest<PaginatedList<ProductReviewDto>>
{
    public int? page { get; set; }
    public int length { get; set; }
    public string searchValue { get; set; } = "";
    public string sortColumn { get; set; } = "Id";
    public string sortOrder { get; set; } = "Desc";
}
public class GetCustomerReviewsWithPagingQueryHandler : IRequestHandler<GetCustomerReviewsWithPagingQuery, PaginatedList<ProductReviewDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetCustomerReviewsWithPagingQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<PaginatedList<ProductReviewDto>> Handle(GetCustomerReviewsWithPagingQuery request, CancellationToken cancellationToken)
    {
        var productReviews = _db.CustomerReviews.Include(c => c.Customer).ThenInclude(c=>c.User).Select(o => new ProductReviewDto
        {
            Id = o.Id,
            Comment = o.Comment,
            Rating = o.Rating,
            DateCommented = o.DateCommented,
            CustomerName = o.Customer.User.FullName,
            ProductName = o.Product.Name,
            Reply = o.Reply,
            IsActive = o.IsActive,
        }).OrderByDescending(o => o.DateCommented).AsQueryable();

        var getProductReviews =
                productReviews
                .Where(a => a.ProductName.ToLower().Contains(request.searchValue))
                .OrderBy($"{request.sortColumn} {request.sortOrder}");

        var data = await PaginatedList<ProductReviewDto>.CreateAsync(getProductReviews, request.page ?? 1, request.length);
        return data;
    }
}
