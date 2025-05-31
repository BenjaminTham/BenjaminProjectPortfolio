using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;
using Ecommerce.Domain.Constants;

namespace Ecommerce.Application.Handlers.NewFolder.Queries;

public class GetCustomerReviewsByCustomerId : IRequest<List<ProductReviewDto>>
{
    public long CustomerId { get; set; }
}
public class GetCustomerReviewsByCustomerIdHandler : IRequestHandler<GetCustomerReviewsByCustomerId, List<ProductReviewDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;

    public GetCustomerReviewsByCustomerIdHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<ProductReviewDto>> Handle(GetCustomerReviewsByCustomerId request, CancellationToken cancellationToken)
    {
        var query = (
            from o in _db.Orders.Include(o => o.OrderStatus)
                .ThenInclude(o => o.OrderStatusValue)
            join od in _db.OrderDetails on o.Id equals od.OrderId into odGroup
            from od in odGroup.DefaultIfEmpty()
            join c in _db.Customers on o.CustomerId equals c.Id
            join v in _db.Variants on od.ProductVariantId equals v.Id
            join p in _db.Products on v.ProductId equals p.Id
            join pImg in _db.ProductImages on p.Id equals pImg.ProductId into pImgGroup
            from pImg in pImgGroup.DefaultIfEmpty()
            join g in _db.Galleries on pImg.ImageId equals g.Id into gGroup
            from g in gGroup.DefaultIfEmpty()
            join cr in _db.CustomerReviews on new { OrderId = o.Id, ProductId = p.Id } equals new { OrderId = cr.OrderId, ProductId = cr.ProductId } into crGroup
            from cr in crGroup.DefaultIfEmpty()
            where o.CustomerId == request.CustomerId
            select new ProductReviewDto
            {
                OrderId = o.Id,
                CustomerId = o.CustomerId,
                ProductId = p.Id,
                InvoiceNo = o.InvoiceNo,
                ProductName = p.Name,
                ProductSlug = p.Slug,
                ProductImage = g.Name,
                Rating = cr.Rating,
                Comment = cr.Comment,
                Reply = cr.Reply,
                RepliedBy = cr.RepliedBy,
                DateCommented = cr.DateCommented,
                IsActive = cr.IsActive,
                PaymentStatus = o.PaymentStatus,
                OrderStatusValue = _db.OrderStatus
                    .OrderByDescending(c=>c.Id).Where(os => os.OrderId == o.Id)
                    .Select(os => os.OrderStatusValue.StatusValue)
                    .FirstOrDefault()
            }).Distinct();



        var result = await query.Distinct().Where(c=>c.OrderStatusValue == DefaultOrderStatusValue.Delivered().StatusValue).ToListAsync(cancellationToken);
        return result;
    }
}