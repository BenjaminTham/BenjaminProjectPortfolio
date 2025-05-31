using AspNetCore.Reporting;

namespace Ecommerce.Web.Mvc.Helpers;

public class ReportHelpers
{
    public RenderType GetRenderType(string reportType)
    {
        var renderType = RenderType.Pdf;
        switch (reportType.ToLower())
        {
            default:
            case "pdf":
                renderType = RenderType.Pdf;
                break;
            case "word":
                renderType = RenderType.Word;
                break;
            case "excel":
                renderType = RenderType.Excel;
                break;
        }

        return renderType;
    }
}
