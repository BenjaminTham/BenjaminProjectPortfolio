using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Handlers.Customers.Queries;
using Ecommerce.Application.Handlers.ProductReviews.Queries;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Entities;
using MediatR;

namespace Ecommerce.Application.Handlers.ProductReviews.Commands;

public class CreateProductReviewByCustomerCommand : IRequest<Response<string>>
{
    public long ProductId { get; set; }
    public long OrderId { get; set; }
    public long CustomerId { get; set; }
    public int Rating { get; set; }
    [StringLength(500, ErrorMessage = "The comment must be no longer than 500 characters.")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?-]*$", ErrorMessage = "Invalid comment format. Only alphabets, numbers, spaces, and common punctuation are allowed.")]
    public string Comment { get; set; }

}
public class CreateProductReviewByCustomerCommandHandler : IRequestHandler<CreateProductReviewByCustomerCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly IMediator _mediator;
    public CreateProductReviewByCustomerCommandHandler(IDataContext db, IMapper mapper, ICurrentUser currentUser, IMediator mediator)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
        _mediator = mediator;
    }
    public async Task<Response<string>> Handle(CreateProductReviewByCustomerCommand request, CancellationToken cancellationToken)
    {
        var customerReview = _mapper.Map<CustomerReview>(request);
        customerReview.DateCommented = DateTime.UtcNow;
        customerReview.IsActive = true;

        try
        {
            var addReview = await _db.CustomerReviews.AddAsync(customerReview, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            return Response<string>.Success("Successfully created");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Response<string>.Fail("Failed to add item!");
        }
    }
}
