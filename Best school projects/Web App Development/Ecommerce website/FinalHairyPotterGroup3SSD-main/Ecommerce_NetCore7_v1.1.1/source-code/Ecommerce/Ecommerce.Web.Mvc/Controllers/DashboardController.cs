using AutoMapper;
using Ecommerce.Application.Handlers.Dashboard.Queries;
using Ecommerce.Domain.Identity.Permissions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IMapper _mapper;
    public DashboardController(IMediator mediator, IMapper mapper, IWebHostEnvironment webHostEnvironment)
    {
        _mediator = mediator;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
    }


    [Authorize(Permissions.Permissions_Size_View)]
    public async Task<IActionResult> Index()
    {
        var result = await _mediator.Send(new GetDashboardSummaryQuery());

        ViewBag.DashboardItems = result;
        return View();
    }

    public async Task<IActionResult> WeeklySalesCountChart()
    {
        var result = await _mediator.Send(new GetWeeklyItemSalesCountQuery());
        return Json(result);
    }

    public async Task<IActionResult> GetWeeklyItemSalesAmount()
    {
        var result = await _mediator.Send(new GetWeeklyItemSalesAmountQuery());
        return Json(result);
    }

    [Authorize(Permissions.Permissions_Size_View)]
    public IActionResult Invoice()
    {
        return View();
    }

    //public async Task<IActionResult> RenderInvoice(string invoiceNo)
    //{
    //    if (id == null || id == 0) return null;
    //    try
    //    {
    //        var currentProject = Assembly.GetCallingAssembly().GetName().Name;
    //        string fileDirPath = Assembly.GetExecutingAssembly().Location.Replace($"{currentProject}.dll", string.Empty);
    //        string rdlcFilePath = string.Format("{0}report\\rdlc\\{1}.rdlc", fileDirPath, "Invoice");
    //        var path = $"{_webHostEnvironment.WebRootPath}\\report\\rdlc\\Invoice.rdlc";

    //        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    //        Encoding.GetEncoding("utf-8");
    //        LocalReport report = new LocalReport(path);

    //        var order = await _mediator.Send(new GetRptOrderInvoiceByOrderIdQuery { InvoiceNo = invoiceNo });


    //        report.AddDataSource("dsReport", order.OrderItems);
    //        report.AddDataSource("dsReportCustomerInfo", order.CustomerInfo);
    //        report.AddDataSource("dsReportOrderInfo", order.OrderSummary);
    //        report.AddDataSource("dsReportOrderAmount", order.OrderAmount);
    //        report.AddDataSource("dsReportPageInfos", order.ReportPageInfos);
    //        var result = report.Execute(RenderType.Pdf, 1);
    //        return File(result.MainStream, System.Net.Mime.MediaTypeNames.Application.Octet, order.InvoiceNo + ".pdf");
    //    }
    //    catch (Exception ex)
    //    {
    //        return null;
    //    }
    //}

}
