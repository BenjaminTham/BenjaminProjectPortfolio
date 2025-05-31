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

public class GetHeaderSliderConfigurationQuery : IRequest<List<HeaderSliderDto>>
{
}
public class GetHeaderSliderConfigurationQueryHandler : IRequestHandler<GetHeaderSliderConfigurationQuery, List<HeaderSliderDto>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    public GetHeaderSliderConfigurationQueryHandler(IDataContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<HeaderSliderDto>> Handle(GetHeaderSliderConfigurationQuery request, CancellationToken cancellationToken)
    {

        var getheaderSlider = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.HeaderSlider).FirstOrDefaultAsync();
        List<HeaderSliderDto> headerSlider = new List<HeaderSliderDto>();

        if (getheaderSlider != null)
        {
            var filteredheaderSlider = JsonSerializer.Deserialize<List<HeaderSlider>>(getheaderSlider.Value);
            var getHeaderSliderImages = await _db.Galleries.Where(o => filteredheaderSlider.Select(o => o.Image).Contains(o.Id)).ToListAsync();
            headerSlider = (from chs in filteredheaderSlider
                            join i in getHeaderSliderImages on chs.Image equals i.Id into ilist
                            from i in ilist.DefaultIfEmpty()
                            select new HeaderSliderDto
                            {
                                HeaderTextLineOne = chs.HeaderTextLineOne,
                                HeaderTextLineTwo = chs.HeaderTextLineTwo,
                                SubText = chs.SubText,
                                IsActive = chs.IsActive,
                                Image = i != null ? i.Id : null,
                                ImagePreview = i != null ? i.Name : null,
                                Order = chs.Order

                            }).OrderBy(o => o.Order).ToList();

            //return headerSlider;

        }
        return headerSlider;
    }

}
