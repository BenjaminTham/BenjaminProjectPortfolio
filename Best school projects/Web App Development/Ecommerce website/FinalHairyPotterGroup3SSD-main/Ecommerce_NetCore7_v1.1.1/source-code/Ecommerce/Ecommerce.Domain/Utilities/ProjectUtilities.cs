using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Domain.Utilities;
public static class ProjectUtilities
{
    public static int TryParseNullableInt(int? val)
    {
        //int value;
        //return int.TryParse(val, out value);
        return Convert.ToInt32(val);
    }
}
