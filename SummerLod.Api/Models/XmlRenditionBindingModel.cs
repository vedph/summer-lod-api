using System.ComponentModel.DataAnnotations;

namespace SummerLod.Api.Models;

public class XmlRenditionBindingModel : XmlBindingModel
{
    [Required]
    [MaxLength(50000)]
    public string Xslt { get; set; } = "";
}
