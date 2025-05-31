using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using MediatR;

namespace Ecommerce.Application.Handlers.ProductReviews.Commands;

public class UpdateProductReviewVisibilityCommand : IRequest<Response<string>>
{
    public long ReviewId { get; set; }
    public bool Checked { get; set; }
}
public class UpdateProductReviewVisibilityCommandHandler : IRequestHandler<UpdateProductReviewVisibilityCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    public UpdateProductReviewVisibilityCommandHandler(IDataContext db, IMapper mapper, ICurrentUser currentUser)
    {
        _db = db;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<Response<string>> Handle(UpdateProductReviewVisibilityCommand request, CancellationToken cancellationToken)
    {
        var review = await _db.CustomerReviews.FindAsync(request.ReviewId);
        review.IsActive = request.Checked;
        try
        {
            _db.CustomerReviews.Update(review);
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
