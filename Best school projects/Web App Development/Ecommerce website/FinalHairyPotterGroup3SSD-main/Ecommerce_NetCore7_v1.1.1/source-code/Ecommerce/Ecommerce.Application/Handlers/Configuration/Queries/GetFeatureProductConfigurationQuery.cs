using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Constants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Configuration.Queries;

public class GetFeatureProductConfigurationQuery : IRequest<List<FeatureProductConfigurationDto>>
{
}
public class GetFeatureProductConfigurationQueryHandler : IRequestHandler<GetFeatureProductConfigurationQuery, List<FeatureProductConfigurationDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetFeatureProductConfigurationQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<FeatureProductConfigurationDto>> Handle(GetFeatureProductConfigurationQuery request, CancellationToken cancellationToken)
    {
        var getFeatureProduct = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.FeatureProductConfiguration).FirstOrDefaultAsync();
        List<FeatureProductConfigurationDto> featureProductDto = new List<FeatureProductConfigurationDto>();

        if (getFeatureProduct != null)
        {
            var featureProduct = JsonSerializer.Deserialize<List<FeatureProductConfigurationDto>>(getFeatureProduct.Value);

            var product = (from p in _db.Products.Include(o => o.Category)
                  .Where(o => featureProduct.Select(o => o.ProductId).Contains(o.Id))
                           join pi in _db.ProductImages on p.Id equals pi.ProductId into plist
                           from pi in plist.DefaultIfEmpty()

                           join i in _db.Galleries on pi.ImageId equals i.Id into ilist
                           from i in ilist.DefaultIfEmpty()
                           select new FeatureProductConfigurationDto
                           {
                               ProductId = p.Id,
                               ProductName = p.Name,
                               ProductCategory = p.Category.Name,
                               ImagePreview = i.Name
                           }).ToList();

            featureProductDto = (from p in product
                                 join fp in featureProduct on p.ProductId equals fp.ProductId into fplist
                                 from fp in fplist.DefaultIfEmpty()
                                 select new FeatureProductConfigurationDto
                                 {
                                     ProductId = p.ProductId,
                                     ProductName = p.ProductName,
                                     ProductCategory = p.ProductCategory,
                                     ImagePreview = p.ImagePreview,
                                     Order = fp.Order
                                 }).OrderBy(o => o.Order).ToList();

            return featureProductDto;
        }


        return featureProductDto;
    }

}
