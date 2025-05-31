using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Handlers.ProductReviews.Queries;

public class GetCustomerReviewDetailsByIdQuery : IRequest<ProductReviewDetailsDto>
{
    public int ReviewId { get; set; }
}
public class GGetCustomerReviewDetailsByIdQueryQueryHandler : IRequestHandler<GetCustomerReviewDetailsByIdQuery, ProductReviewDetailsDto>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    //private readonly UserManager<User> _userManager;
    public GGetCustomerReviewDetailsByIdQueryQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ProductReviewDetailsDto> Handle(GetCustomerReviewDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        var reviewDetails = await (from pr in _db.CustomerReviews.Include(c => c.Customer)
                                   where pr.Id == request.ReviewId
                                   join p in _db.Products on pr.ProductId equals p.Id into plist
                                   from p in plist.DefaultIfEmpty()
                                   join pi in _db.ProductImages on p.Id equals pi.ProductId into pilist
                                   from pi in pilist.DefaultIfEmpty()
                                   join i in _db.Galleries on pi.ImageId equals i.Id into ilist
                                   from i in ilist.DefaultIfEmpty()
                                   select new ProductReviewDetailsDto
                                   {
                                       Id = pr.Id,
                                       Comment = pr.Comment,
                                       Rating = pr.Rating,
                                       DateCommented = pr.DateCommented,
                                       CustomerId = pr.CustomerId,
                                       //CustomerName = pr.Customer.FullName,
                                       Reply = pr.Reply,
                                       DateReplied = pr.DateReplied,
                                       RepliedBy = pr.RepliedBy,
                                       IsActive = pr.IsActive,
                                       ProductId = p.Id,
                                       ProductName = p.Name,
                                       ProductPreview = i.Name
                                   }).OrderByDescending(o => o.DateCommented).FirstOrDefaultAsync();

        return reviewDetails;
    }
}
