using AutoMapper;
using Ecommerce.Application.Handlers.Categories.Queries;
using Ecommerce.Web.Mvc.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Views.Shared.Components.NavigationCategory;

public class NavigationCategoryViewComponent : ViewComponent
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    public NavigationCategoryViewComponent(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var category = await _mediator.Send(new GetCategoriesQuery());

        List<NavCategoryVM> listMenu = category
                          .Select(m => new NavCategoryVM()
                          {
                              Id = m.Id,
                              Name = m.Name,
                              Slug = m.Slug,
                              ParentCategoryId = m.ParentCategoryId,
                          }).Distinct().ToList();

        var d = listMenu;

        List<NavCategoryVM> menuTree = GetMenuTree(listMenu, null);
        ViewBag.NavCategory = menuTree.OrderByDescending(o => o.Children.Count);
        return View();
    }

    public List<NavCategoryVM> GetMenuTree(List<NavCategoryVM> list, int? parent)
    {
        var d = list.Where(x => x.ParentCategoryId == parent).Select(x => new NavCategoryVM
        {
            Id = x.Id,
            Name = x.Name,
            Slug = x.Slug,
            ParentCategoryId = x.ParentCategoryId,
            Children = GetMenuTree(list, x.Id)
        }).ToList();

        return d;
    }
}
