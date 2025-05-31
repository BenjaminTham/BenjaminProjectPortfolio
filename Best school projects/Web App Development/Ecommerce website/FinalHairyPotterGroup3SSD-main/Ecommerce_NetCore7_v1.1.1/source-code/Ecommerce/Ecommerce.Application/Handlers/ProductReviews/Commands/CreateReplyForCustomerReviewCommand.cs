using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Handlers.ProductReviews.Commands;

public class CreateReplyForCustomerReviewCommand : IRequest<Response<string>>
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Please input a reply.")]
    [RegularExpression(@"^[a-zA-Z0-9\s.,!?_'-]*$", ErrorMessage = "Only alphanumeric characters, spaces, '.', ',', '!', '?', '-', and '_' are allowed in the description.")]
    public string Reply { get; set; }
}
public class CreateReplyForCustomerReviewCommandHandler : IRequestHandler<CreateReplyForCustomerReviewCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    public CreateReplyForCustomerReviewCommandHandler(IDataContext db, IMapper mapper, ICurrentUser currentUser)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Response<string>> Handle(CreateReplyForCustomerReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var productReview = await _db.CustomerReviews.FindAsync(request.Id);
            productReview.RepliedBy = _currentUser.UserName;
            productReview.DateReplied = DateTime.UtcNow;
            _mapper.Map(request, productReview);
            var updateReview = _db.CustomerReviews.Update(productReview);
            await _db.SaveChangesAsync(cancellationToken);

            return Response<string>.Success("Successfully created");
        }
        //catch (ValidationException e)
        //{
        //    Console.WriteLine(e);
        //    return Response<string>.Fail("Failed to add item!");
        //}
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Response<string>.Fail("Failed to add item!");
        }
    }
}
