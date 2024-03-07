using System.ComponentModel.DataAnnotations;

namespace SummerLod.Api.Models;

public class XmlBindingModel
{
    [Required]
    [MaxLength(50000)]
    public string Xml { get; set; } = "";
}
