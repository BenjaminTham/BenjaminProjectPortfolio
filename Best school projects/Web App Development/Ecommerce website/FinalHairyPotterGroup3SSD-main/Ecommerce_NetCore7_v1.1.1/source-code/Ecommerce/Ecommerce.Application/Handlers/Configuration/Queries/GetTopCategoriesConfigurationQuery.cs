using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Configuration.Queries;

public class GetTopCategoriesConfigurationQuery : IRequest<List<TopCategoriesConfigurationDto>>
{
}
public class GetTopCategoriesConfigurationQueryHandler : IRequestHandler<GetTopCategoriesConfigurationQuery, List<TopCategoriesConfigurationDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetTopCategoriesConfigurationQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<TopCategoriesConfigurationDto>> Handle(GetTopCategoriesConfigurationQuery request, CancellationToken cancellationToken)
    {
        var getCategoriesValue = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.TopCategoriesConfiguration).FirstOrDefaultAsync();
        List<TopCategoriesConfigurationDto> topCategories = new List<TopCategoriesConfigurationDto>();
        if (getCategoriesValue != null)
        {
            var filteredTopCategories = JsonSerializer.Deserialize<List<TopCategoriesConfiguration>>(getCategoriesValue.Value);
            var categories = _db.Categories.Where(o => filteredTopCategories.Select(o => o.CategoryId).Contains(o.Id)).ToList();
            var getCatImages = await _db.Galleries.Where(o => filteredTopCategories.Select(o => o.Image).Contains(o.Id)).ToListAsync();


            topCategories = (from cc in filteredTopCategories
                             join c in _db.Categories on cc.CategoryId equals c.Id into clist
                             from c in clist.DefaultIfEmpty()
                             join i in getCatImages on cc.Image equals i.Id into ilist
                             from i in ilist.DefaultIfEmpty()
                             select new TopCategoriesConfigurationDto
                             {
                                 CategoryId = cc.CategoryId,
                                 Image = i != null ? i.Id : null,
                                 ImagePreview = i != null ? i.Name : null,
                                 //Slug = c.Slug,
                                 Title = c.Name,
                                 Order = cc.Order

                             }).OrderBy(o => o.Order).ToList();

            return topCategories;
        }
        return topCategories;
    }

}
