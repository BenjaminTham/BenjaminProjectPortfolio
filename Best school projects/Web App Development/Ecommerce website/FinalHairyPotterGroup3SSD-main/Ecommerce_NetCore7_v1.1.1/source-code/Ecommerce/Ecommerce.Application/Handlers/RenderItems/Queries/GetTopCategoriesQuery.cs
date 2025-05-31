using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Text.Json;


namespace Ecommerce.Application.Handlers.RenderItems.Queries;

public class GetTopCategoriesQuery : IRequest<IEnumerable<TopCategoriesConfigurationDto>>
{
}
public class GetTopCategoriesQueryHandler : IRequestHandler<GetTopCategoriesQuery, IEnumerable<TopCategoriesConfigurationDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    public GetTopCategoriesQueryHandler(IDataContext db, IMapper mapper, IKeyAccessor keyAccessor)
    {
        _db = db;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
    }

    public async Task<IEnumerable<TopCategoriesConfigurationDto>> Handle(GetTopCategoriesQuery request, CancellationToken cancellationToken)
    {
        List<TopCategoriesConfigurationDto> conTopCategories = JsonSerializer.Deserialize<List<TopCategoriesConfigurationDto>>(_keyAccessor.GetSection("TopCategoriesConfiguration"))!;
        var getCatImages = await _db.Galleries.Where(o => conTopCategories.Select(o => o.Image).Contains(o.Id)).ToListAsync(cancellationToken);

        List<TopCategoriesConfigurationDto> productShocases = (from cc in conTopCategories
                                                               join c in _db.Categories on cc.CategoryId equals c.Id into clist
                                                               from c in clist.DefaultIfEmpty()
                                                               join i in getCatImages on cc.Image equals i.Id into ilist
                                                               from i in ilist.DefaultIfEmpty()
                                                               select new TopCategoriesConfigurationDto
                                                               {
                                                                   CategoryId = cc.CategoryId,
                                                                   Image = i != null ? i.Name : null,
                                                                   Slug = c.Slug,
                                                                   Title = c.Name,
                                                                   Order = cc.Order

                                                               }).OrderBy(o => o.Order).ToList();

        return productShocases;
    }

}
