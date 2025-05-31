using AspNetCore.Reporting;
using AutoMapper;
using Ecommerce.Application.Dto;
using Ecommerce.Application.Handlers.Orders.Queries;
using Ecommerce.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace Ecommerce.Web.Mvc.Controllers;

public class ReportController : Controller
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IMapper _mapper;
    private readonly IKeyAccessor _keyAccessor;
    private readonly ILogger<ReportController> _logger;

    public ReportController(IMediator mediator, IMapper mapper, IWebHostEnvironment webHostEnvironment, IKeyAccessor keyAccessor, ILogger<ReportController> logger)
    {
        _mediator = mediator;
        _mapper = mapper;
        _webHostEnvironment = webHostEnvironment;
        _keyAccessor = keyAccessor;
        _logger = logger;
    }


    [HttpGet]
    [Route("/order-completed/{invoiceNo}")]
    public async Task<IActionResult> OrderInvoice(string invoiceNo)
    {
        _logger.LogInformation("Received request for OrderInvoice with InvoiceNo: {InvoiceNo}", invoiceNo);

        if (string.IsNullOrEmpty(invoiceNo))
        {
            _logger.LogWarning("Invoice number is null or empty.");
            return NotFound("Invoice number is required.");
        }

        var order = await _mediator.Send(new GetRptOrderInvoiceByOrderIdQuery { InvoiceNo = invoiceNo });
        if (order == null)
        {
            _logger.LogWarning("No order found with invoice number: {InvoiceNo}", invoiceNo);
            return NotFound($"No order found for invoice number: {invoiceNo}.");
        }

        var genConfig = JsonSerializer.Deserialize<GeneralConfigurationDto>(_keyAccessor.GetSection("GeneralConfiguration"));


        try
        {
            //var currentProject = Assembly.GetCallingAssembly().GetName().Name;
            //string fileDirPath = Assembly.GetExecutingAssembly().Location.Replace($"{currentProject}.dll", string.Empty);
            //string rdlcFilePath = string.Format("{0}report\\rdlc\\{1}.rdlc", fileDirPath, "Invoice");

            var path = $"{_webHostEnvironment.WebRootPath}\\report\\rdlc\\Invoice.rdlc";

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("utf-8");
            LocalReport report = new LocalReport(path);


            var logo = genConfig.CompanyLogo != null
                ? Convert.ToBase64String(System.IO.File.ReadAllBytes($"{_webHostEnvironment.WebRootPath}\\{genConfig.CompanyLogo}"))
                : ImageToBase64(TextToImage(genConfig?.CompanyName));

            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("logo", logo);

            report.AddDataSource("dsReport", order.OrderItems);
            report.AddDataSource("dsReportCustomerInfo", order.CustomerInfo);
            report.AddDataSource("dsReportOrderInfo", order.OrderSummary);
            report.AddDataSource("dsReportOrderAmount", order.OrderAmount);
            report.AddDataSource("dsReportPageInfos", order.ReportPageInfos);
            var result = report.Execute(RenderType.Pdf, 1, param);
            return File(result.MainStream, System.Net.Mime.MediaTypeNames.Application.Octet, $"{order.InvoiceNo}.pdf");
        }
        catch (Exception ex)
        {
            return NotFound();
        }


    }

    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

    static Bitmap TextToImage(string text)
    {
        // Create a new Bitmap object
        Bitmap bitmap = new Bitmap(2, 1);

        // Create a new Graphics object from the Bitmap
        Graphics graphics = Graphics.FromImage(bitmap);

        // Measure the size of the text using the Graphics object
        SizeF size = graphics.MeasureString(text, new Font("Rubik", 100));

        // Resize the Bitmap object to fit the text
        bitmap = new Bitmap(bitmap, (int)size.Width, (int)size.Height);

        // Create a new Graphics object from the resized Bitmap
        graphics = Graphics.FromImage(bitmap);

        System.Drawing.Color color = ColorTranslator.FromHtml("#7a7a7a");
        Brush brush = new SolidBrush(color);

        // Draw the text onto the Graphics object
        graphics.DrawString(text, new Font("Rubik", 100, FontStyle.Bold), brush, new PointF(0, 0));

        return bitmap;
    }

    static string ImageToBase64(Image image)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            // Convert the Image to a byte array
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            byte[] imageBytes = ms.ToArray();

            // Convert the byte array to a base64 string
            string base64String = Convert.ToBase64String(imageBytes);

            return base64String;
        }
    }
}

