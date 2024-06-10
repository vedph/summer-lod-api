using Fusi.Xml.Extras.Render;
using Microsoft.AspNetCore.Mvc;
using SummerLod.Api.Models;
using SummerLod.Api.Services;
using System.Diagnostics;
using System.Xml.Linq;

namespace SummerLod.Api.Controllers;

[ApiController]
[Route("xml")]
[Produces("application/json")]
public sealed class XmlController(ILogger<XmlController> logger,
    TeiEntityParserService entityParser) : ControllerBase
{
    private readonly ILogger<XmlController> _logger = logger;
    private readonly TeiEntityParserService _entityParser = entityParser;

    /// <summary>
    /// Renders the specified XML using the received XSLT code.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>Rendition output, with result or error.</returns>
    [HttpPost("rendition")]
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

    /// <summary>
    /// Parses entities from the received XML.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>Entities with result or error.</returns>
    [HttpPost("entities")]
    public EntityListModel ParseEntities([FromBody] XmlBindingModel model)
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

    /// <summary>
    /// Prettifies the received XML.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>Prettified XML or error.</returns>
    [HttpPost("prettify")]
    public XmlModel PrettifyXml([FromBody] XmlBindingModel model)
    {
        try
        {
            XDocument doc = XDocument.Parse(model.Xml);
            return new XmlModel
            {
                Xml = doc.ToString(SaveOptions.OmitDuplicateNamespaces)
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            _logger.LogError(ex, "Error prettifying XML");
            return new XmlModel
            {
                Error = ex.Message
            };
        }
    }

    /// <summary>
    /// Uglifies the received XML.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>Uglified XML or error.</returns>
    [HttpPost("uglify")]
    public XmlModel UglifyXml([FromBody] XmlBindingModel model)
    {
        try
        {
            XDocument doc = XDocument.Parse(model.Xml);
            return new XmlModel
            {
                Xml = doc.ToString(SaveOptions.OmitDuplicateNamespaces |
                    SaveOptions.DisableFormatting)
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.ToString());
            _logger.LogError(ex, "Error prettifying XML");
            return new XmlModel
            {
                Error = ex.Message
            };
        }
    }
}
