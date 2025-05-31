using AutoMapper;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.Categories.Queries;
using Ecommerce.Application.Handlers.Configuration.Commands;
using Ecommerce.Application.Handlers.Configuration.Queries;
using Ecommerce.Domain.Identity.Permissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class ConfigurationController : Controller
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public ConfigurationController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    #region General Configuration
    [Authorize(Permissions.Permissions_Configuration_General)]
    public async Task<IActionResult> Index()
    {
        var result = await _mediator.Send(new GetGeneralConfigurationQuery());
        return View(result);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_General)]
    public async Task<IActionResult> Index(GeneralConfigurationDto generalConfiguration)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(new UpdateGeneralConfigurationCommand { generalConfiguration = generalConfiguration });
            if (response.Succeeded)
            {
                generalConfiguration.CompanyFaviconPreview = generalConfiguration.CompanyFavicon;
                generalConfiguration.CompanyLogoPreview = generalConfiguration.CompanyLogo;
                return View(generalConfiguration);
            }
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(generalConfiguration);
    }
    #endregion

    #region Solial
    [Authorize(Permissions.Permissions_Configuration_Social)]
    public async Task<IActionResult> Social()
    {
        var result = await _mediator.Send(new GetSocialConfigurationQuery());
        return View(result);
    }
    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_Social)]
    public async Task<IActionResult> Social(SocialProfileDto socialProfile)
    {
        var updateSocialProfile = _mapper.Map<UpdateSocialConfigurationCommand>(socialProfile);
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(updateSocialProfile);
            if (response.Succeeded) return View(socialProfile);
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(socialProfile);
    }
    #endregion

    [Authorize(Permissions.Permissions_Configuration_Shop)]
    public IActionResult Shop()
    {
        return View();
    }

    #region Header Slider
    [Authorize(Permissions.Permissions_Configuration_Shop)]
    public async Task<IActionResult> HeaderSlider()
    {
        var result = await _mediator.Send(new GetHeaderSliderConfigurationQuery());
        return View(result);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_Shop)]
    public async Task<IActionResult> UpdateHeaderSlider(IList<HeaderSliderDto> headerSliders)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(new UpdateHeaderSliderConfigurationCommand { HeaderSliders = headerSliders });
            if (response.Succeeded) return Json(headerSliders);
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return Json(headerSliders);
    }
    #endregion

    #region Banner
    [Authorize(Permissions.Permissions_Configuration_Shop)]
    public async Task<IActionResult> Banner()
    {
        var result = await _mediator.Send(new GetBannerConfigQuery());
        return View(result);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_Shop)]
    public async Task<IActionResult> UpdateBanner(IList<BannerDto> banners)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(new UpdateBannerConfigCommand() { Banners = banners });
            if (response.Succeeded) return Json(banners);
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return Json(banners);
    }
    #endregion

    #region Deal Of The day
    [Authorize(Permissions.Permissions_Configuration_Shop)]
    public async Task<IActionResult> DealOfTheDay()
    {
        var result = await _mediator.Send(new GetDealOfTheDayConfigQuery());
        return View(result);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_Shop)]
    public async Task<IActionResult> DealOfTheDay(DealOfTheDayDto dealOfTheDay)
    {
        //var updateDealOfTheDay = await _mapper.Map<UpdateDealOfTheDayConfigCommand>(dealOfTheDay);
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(new UpdateDealOfTheDayConfigCommand { DealOfTheDay = dealOfTheDay });
            if (response.Succeeded) return RedirectToAction("DealOfTheDay");
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(dealOfTheDay);
    }
    #endregion

    #region Feature Items
    [Authorize(Permissions.Permissions_Configuration_Shop)]
    public async Task<IActionResult> FeatureItems()
    {
        var result = await _mediator.Send(new GetFeatureProductConfigurationQuery());
        return View(result);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_Shop)]
    public async Task<IActionResult> FeatureItems(List<int> ProductId)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(new UpdateFeatureProductConfigurationCommand { ProductId = ProductId });
            if (response.Succeeded) return RedirectToAction("FeatureItems"); ;
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return RedirectToAction("FeatureItems");
    }
    #endregion

    #region Top Categories
    [Authorize(Permissions.Permissions_Configuration_Shop)]
    public async Task<IActionResult> TopCategories()
    {
        ViewData["CategoryId"] = new SelectList(await _mediator.Send(new GetCategoriesQuery()), "Id", "Name");

        var result = await _mediator.Send(new GetTopCategoriesConfigurationQuery());
        return View(result);
    }

    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_Shop)]
    public async Task<IActionResult> TopCategories(List<TopCategoriesConfigurationDto> data)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(new UpdateTopCategoriesConfigurationCommand { TopCategories = data });
            if (response.Succeeded) return Json(data);
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return Json(data);
    }
    #endregion

    #region Basic Seo
    [Authorize(Permissions.Permissions_Configuration_BasicSeo)]
    public async Task<IActionResult> BasicSeo()
    {
        var result = await _mediator.Send(new GetBasicSeoConfigurationQuery());
        return View(result);
    }
    [HttpPost]
    [Authorize(Permissions.Permissions_Configuration_BasicSeo)]
    public async Task<IActionResult> BasicSeo(BasicSeoConfigurationDto basicSeoConfiguration)
    {
        if (ModelState.IsValid)
        {
            var response = await _mediator.Send(new UpdateBasicSeoConfigurationCommand { basicSeoConfiguration = basicSeoConfiguration });
            if (response.Succeeded) return View(basicSeoConfiguration);
            ModelState.AddModelError(string.Empty, response.Message);
        }

        return View(basicSeoConfiguration);
    }
    #endregion

}
