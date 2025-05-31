using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Ecommerce.Domain.Enums;
public enum BannerType
{
    [Display(Name = "Home Header With Top Category")]
    BannerOne = 1,
}