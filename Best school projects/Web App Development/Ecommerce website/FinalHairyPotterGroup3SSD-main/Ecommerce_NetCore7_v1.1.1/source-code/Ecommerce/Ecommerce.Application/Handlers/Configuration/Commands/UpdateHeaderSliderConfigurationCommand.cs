using AutoMapper;
using Ecommerce.Application.Common;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Common;
using Ecommerce.Domain.Constants;
using Ecommerce.Domain.Entities;
using Ecommerce.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Ecommerce.Application.Handlers.Configuration.Commands;

public class UpdateHeaderSliderConfigurationCommand : IRequest<Response<string>>
{
    public IList<HeaderSliderDto> HeaderSliders { get; set; }
}
public class UpdateHeaderSliderConfigurationCommandHandler : IRequestHandler<UpdateHeaderSliderConfigurationCommand, Response<string>>
{
    private readonly IDataContext _db;
    private readonly IMapper _mapper;
    private readonly IMemoryCacheManager _cacheManager;
    public UpdateHeaderSliderConfigurationCommandHandler(IDataContext db, IMapper mapper, IMemoryCacheManager cacheManager)
    {
        _db = db;
        _mapper = mapper;
        _cacheManager = cacheManager;
    }


    public async Task<Response<string>> Handle(UpdateHeaderSliderConfigurationCommand request, CancellationToken cancellationToken)
    {
        AppConfiguration appConfiguration = new AppConfiguration();
        List<HeaderSlider> headerSlider = new List<HeaderSlider>();
        var getHeaderSliderConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.HeaderSlider).FirstOrDefaultAsync();

        if (getHeaderSliderConfiguration == null)
        {
            appConfiguration.Key = AppConfigurationType.HeaderSlider;
            appConfiguration.Value = JsonSerializer.Serialize(headerSlider);

            await _db.AppConfigurations.AddAsync(appConfiguration);
        }
        else
        {

            getHeaderSliderConfiguration.Value = JsonSerializer.Serialize(_mapper.Map<List<HeaderSlider>>(request.HeaderSliders));
            _db.AppConfigurations.Update(getHeaderSliderConfiguration);
        }



        //AppConfiguration appConfiguration = new AppConfiguration();
        //var headerSliderSerialize = JsonSerializer.Serialize(request.HeaderSliders);

        //var getHeaderSliderConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.HeaderSlider).FirstOrDefaultAsync();
        //if (getHeaderSliderConfiguration == null)
        //{
        //    appConfiguration.Key = AppConfigurationType.HeaderSlider;
        //    appConfiguration.Value = headerSliderSerialize;

        //    await _db.AppConfigurations.AddAsync(appConfiguration);
        //}
        //else
        //{
        //    List<HeaderSlider> headerSlider = new List<HeaderSlider>();
        //    List<HeaderSlider> getheaderSlider = JsonSerializer.Deserialize<List<HeaderSlider>>(getHeaderSliderConfiguration.Value);
        //    var getHeaderSliderImages = await _db.Galleries.Where(o => getheaderSlider.Select(o => o.Image).Contains(o.Id)).ToListAsync();

        //    headerSlider = (from chs in getheaderSlider
        //                    join i in getHeaderSliderImages on chs.Image equals i.Id into ilist
        //                    from i in ilist.DefaultIfEmpty()
        //                    select new HeaderSlider
        //                    {
        //                        HeaderTextLineOne = chs.HeaderTextLineOne,
        //                        HeaderTextLineTwo = chs.HeaderTextLineTwo,
        //                        SubText = chs.SubText,
        //                        IsActive = chs.IsActive,
        //                        Image = i != null ? ("/" + i.Name) : null,
        //                        Order = chs.Order
        //                    }).OrderBy(o => o.Order).ToList();


        //    headerSliderSerialize = JsonSerializer.Serialize(headerSlider);
        //    getHeaderSliderConfiguration.Value = headerSliderSerialize;
        //    _db.AppConfigurations.Update(getHeaderSliderConfiguration);
        //}


        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            _cacheManager.AppConfigurationRestore();
            return Response<string>.Success("Successfully updated");
        }
        catch (Exception e)
        {
            return Response<string>.Fail("Failed to update");
        }

    }

    //public async Task<Response<string>> Handle(UpdateHeaderSliderConfigurationCommand request, CancellationToken cancellationToken)
    //{
    //    AppConfiguration appConfiguration = new AppConfiguration();
    //    var headerSliderSerialize = JsonSerializer.Serialize(request.HeaderSliders);
    //    var getHeaderSliderConfiguration = await _db.AppConfigurations.Where(o => o.Key == AppConfigurationType.HeaderSlider).FirstOrDefaultAsync();
    //    if (getHeaderSliderConfiguration == null)
    //    {
    //        appConfiguration.Key = AppConfigurationType.HeaderSlider;
    //        appConfiguration.Value = headerSliderSerialize;

    //        await _db.AppConfigurations.AddAsync(appConfiguration);
    //    }
    //    else
    //    {
    //        List<HeaderSlider> headerSlider = new List<HeaderSlider>();
    //        List<HeaderSlider> getheaderSlider = JsonSerializer.Deserialize<List<HeaderSlider>>(getHeaderSliderConfiguration.Value);
    //        var getHeaderSliderImages = await _db.Galleries.Where(o => getheaderSlider.Select(o => o.Image).Contains(o.Id)).ToListAsync();

    //        headerSlider = (from chs in getheaderSlider
    //                        join i in getHeaderSliderImages on chs.Image equals i.Id into ilist
    //                        from i in ilist.DefaultIfEmpty()
    //                        select new HeaderSlider
    //                        {
    //                            HeaderTextLineOne = chs.HeaderTextLineOne,
    //                            HeaderTextLineTwo = chs.HeaderTextLineTwo,
    //                            SubText = chs.SubText,
    //                            IsActive = chs.IsActive,
    //                            Image = i != null ? ("/" + i.Name) : null,
    //                            Order = chs.Order
    //                        }).OrderBy(o => o.Order).ToList();


    //        headerSliderSerialize = JsonSerializer.Serialize(headerSlider);
    //        getHeaderSliderConfiguration.Value = headerSliderSerialize;
    //        _db.AppConfigurations.Update(getHeaderSliderConfiguration);
    //    }


    //    try
    //    {
    //        await _db.SaveChangesAsync(cancellationToken);
    //        _cacheManager.AppConfigurationRestore();
    //        return Response<string>.Success("Successfully updated");
    //    }
    //    catch (Exception e)
    //    {
    //        return Response<string>.Fail("Failed to update");
    //    }

    //}
}
