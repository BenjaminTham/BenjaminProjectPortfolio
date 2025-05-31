using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Controllers;

[AllowAnonymous]
public class ErrorController : Controller
{
    [Route("Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        var msg = "";
        switch (statusCode)
        {
            case 404:
                msg = "<script>swal(`" + "Not Found!" + "`, `" + "404 - Page Not Found" + "`,`" + "warning" + "`)" + "</script>";
                ViewBag.ErrorMessageTitle = "Page Not Found";
                ViewBag.ErrorMessage = "Sorry, the resource you requested could not be found";
                ViewBag.ErrorCode = "404";
                break;
            case 500:
                msg = "<script>swal(`" + "Not Found!" + "`, `" + "500 - Internal Server Error" + "`,`" + "warning" + "`)" + "</script>";
                ViewBag.ErrorMessageTitle = "Internal Server Error";
                ViewBag.ErrorMessage = "Something went wrong! The server encountered an internal error or misconfiguration and unable to complete your request.";
                ViewBag.ErrorCode = "500";
                break;
            default:
                msg = "<script>swal(`" + "Not Found!" + "`, `" + "Sorry, resource not found" + "`,`" + "warning" + "`)" + "</script>";
                ViewBag.ErrorMessageTitle = "Error";
                ViewBag.ErrorMessage = "Encountered an error, please try again.";
                ViewBag.ErrorCode = statusCode.ToString();
                break;

        }

        var returnurl = Request.Headers["Referer"].ToString();
        if (returnurl != "")
        {
            TempData["notification"] = msg;
            return Redirect(returnurl);

        }
        return View("NotFound");
    }
}
