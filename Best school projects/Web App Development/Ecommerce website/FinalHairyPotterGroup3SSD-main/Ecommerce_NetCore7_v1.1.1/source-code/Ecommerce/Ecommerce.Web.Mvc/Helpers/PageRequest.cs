namespace Ecommerce.Web.Mvc.Helpers;

public class PageRequest
{
    public PageEntity GetPageResponse(HttpRequest queryCollection)
    {
        int draw = Convert.ToInt32(queryCollection.Query["draw"]);
        int start = Convert.ToInt32(queryCollection.Query["start"]); // Data to be skipped, if 0 first "length" records will be fetched, if 1 second "length" of records will be fethced ...
        int length = Convert.ToInt32(queryCollection.Query["length"]); // Records count to be fetched after skip
        int pageIndex = (start + length) / length;

        // Getting Sort Column Name
        int sortColumnIdx = Convert.ToInt32(queryCollection.Query["order[0][column]"]);
        string sortColumnName = queryCollection.Query["columns[" + sortColumnIdx + "][name]"];

        string sortOrder = queryCollection.Query["order[0][dir]"]; // Sort Column Direction  Asc/Desc
        string searchValue = queryCollection.Query["search[value]"].FirstOrDefault()?.Trim().ToLower(); // Search Value

        PageEntity pageEntity = new PageEntity
        {
            Draw = draw,
            PageIndex = pageIndex,
            Start = start,
            Length = length,
            SortColumnName = sortColumnName,
            SortOrder = sortOrder,
            SearchValue = searchValue
        };

        return pageEntity;
    }

    public PageEntity PostPageResponse(HttpRequest queryCollection)
    {
        int draw = Convert.ToInt32(queryCollection.Form["draw"]);
        int start = Convert.ToInt32(queryCollection.Form["start"]); // Data to be skipped, if 0 first "length" records will be fetched, if 1 second "length" of records will be fethced ...
        int length = Convert.ToInt32(queryCollection.Form["length"]); // Records count to be fetched after skip
        int pageIndex = (start + length) / length;

        // Getting Sort Column Name
        int sortColumnIdx = Convert.ToInt32(queryCollection.Form["order[0][column]"]);
        string sortColumnName = queryCollection.Form["columns[" + sortColumnIdx + "][name]"];

        string sortOrder = queryCollection.Form["order[0][dir]"]; // Sort Column Direction  Asc/Desc
        string searchValue = queryCollection.Form["search[value]"].FirstOrDefault()?.Trim().ToLower(); // Search Value

        PageEntity pageEntity = new PageEntity
        {
            Draw = draw,
            PageIndex = pageIndex,
            Start = start,
            Length = length,
            SortColumnName = sortColumnName,
            SortOrder = sortOrder,
            SearchValue = searchValue
        };

        return pageEntity;
    }
}

public class PageEntity
{
    public int Draw { get; set; }
    public int PageIndex { get; set; }
    public int Start { get; set; }
    public int Length { get; set; }
    public string? SortOrder { get; set; }
    public string? SortColumnName { get; set; }
    public string? SearchValue { get; set; }
}
