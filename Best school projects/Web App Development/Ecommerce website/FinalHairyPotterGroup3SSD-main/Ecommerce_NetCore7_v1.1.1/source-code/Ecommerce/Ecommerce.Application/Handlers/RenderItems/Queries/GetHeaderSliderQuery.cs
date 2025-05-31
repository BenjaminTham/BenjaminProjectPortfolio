using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.RenderItems.Queries;

public class GetHeaderSliderQuery : IRequest<List<HeaderSliderDto>>
{
}
public class GetHeaderSliderQueryHandler : IRequestHandler<GetHeaderSliderQuery, List<HeaderSliderDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    public GetHeaderSliderQueryHandler(IDataContext db, IMapper mapper, IKeyAccessor keyAccessor)
    {
        _db = db;
        _mapper = mapper;
        _keyAccessor = keyAccessor;
    }

    public async Task<List<HeaderSliderDto>> Handle(GetHeaderSliderQuery request, CancellationToken cancellationToken)
    {
        List<HeaderSliderDto> getheaderSlider = JsonSerializer.Deserialize<List<HeaderSliderDto>>(_keyAccessor.GetSection("HeaderSlider"))!;
        var getHeaderSliderImages = await _db.Galleries.Where(o => getheaderSlider.Select(o => o.Image).Contains(o.Id)).ToListAsync(cancellationToken);

        //List<HeaderSliderDto> headerSlider = (from chs in getheaderSlider
        //                                      where chs.IsActive != false
        //                                      join i in getHeaderSliderImages on chs.Image equals i.Id into ilist
        //                                      from i in ilist.DefaultIfEmpty()
        //                                      select new HeaderSliderDto
        //                                      {
        //                                          HeaderTextLineOne = chs.HeaderTextLineOne,
        //                                          HeaderTextLineTwo = chs.HeaderTextLineTwo,
        //                                          SubText = chs.SubText,
        //                                          IsActive = chs.IsActive,
        //                                          Image = i != null ? i.Id : null,
        //                                          ImagePreview = i != null ? i.Name : null,
        //                                          Order = chs.Order

        //                                      }).OrderBy(o => o.Order).ToList();

        var headerSlider = (
            from chs in getheaderSlider
            where chs.IsActive
            let i = getHeaderSliderImages.FirstOrDefault(img => img.Id == chs.Image)
            select new HeaderSliderDto
            {
                HeaderTextLineOne = chs.HeaderTextLineOne,
                HeaderTextLineTwo = chs.HeaderTextLineTwo,
                SubText = chs.SubText,
                IsActive = chs.IsActive,
                Image = i?.Id,
                ImagePreview = i?.Name,
                Order = chs.Order
            }
        ).OrderBy(o => o.Order).ToList();

        return headerSlider;
    }

}