using Fusi.Xml.Extras.Render;
using Microsoft.AspNetCore.Mvc;
using SummerLod.Api.Models;
using SummerLod.Api.Services;
using System.Diagnostics;

namespace SummerLod.Api.Controllers;

[ApiController]
[Produces("application/json")]
public class XmlController : ControllerBase
{
    private readonly ILogger<XmlController> _logger;
    private readonly TeiEntityParserService _entityParser;

    public XmlController(ILogger<XmlController> logger,
        TeiEntityParserService entityParser)
    {
        _logger = logger;
        _entityParser = entityParser;
    }

    [HttpPost("xml/rendition")]
    public XmlRenditionModel Render([FromBody] XmlRenditionBindingModel model)
    {
        try
        {
            XsltTransformer xslt = new(model.Xslt);
            string result = xslt.Transform(model.Xml);
            return new XmlRenditionModel
            {
                Result = result
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            _logger.LogError(ex, "Error rendering XML");
            return new XmlRenditionModel
            {
                Error = ex.Message
            };
        }
    }

    [HttpPost("xml/entities")]
    public EntityListModel EntityListModel([FromBody] XmlBindingModel model)
    {
        try
        {
            return new EntityListModel()
            {
                Entities = _entityParser.Parse(model.Xml)
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            _logger.LogError(ex, "Error parsing XML for entities");
            return new EntityListModel
            {
                Error = ex.Message
            };
        }
    }
}
