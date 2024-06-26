﻿using SummerLod.Api.Models;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SummerLod.Api.Services;

public class TeiEntityParserService
{
    public static readonly XNamespace TEI = "http://www.tei-c.org/ns/1.0";

    private static string CollectSpacedConcatenatedValue(XElement element)
    {
        if (element.HasElements)
        {
            return Regex.Replace(
                string.Join(" ", element.Descendants().Select(n => n.Value)),
                @"\s+", " ").Trim();
        }
        return element.Value;
    }

    private static Entity ParsePerson(XElement person, bool org = false)
    {
        return new()
        {
            Type = org? "organization" : "person",
            Names = person.Elements(TEI + (org? "orgName" : "persName"))
                .Select(CollectSpacedConcatenatedValue).ToList(),
            Ids = person.Elements(TEI + "idno").Select(n => n.Value).ToList(),
            Links = person.Elements(TEI + "link")
                .Where(n => n.Attribute("target") != null)
                .Select(n => n.Attribute("target")!.Value)
                .ToList(),
            Description = person.Elements(TEI + "note")
                .Select(n => n.Value).FirstOrDefault()
        };
    }

    private static Entity ParsePlace(XElement place)
    {
        return new()
        {
            Type = "place",
            Names = place.Elements(TEI + "placeName")
                .Select(CollectSpacedConcatenatedValue).ToList(),
            Ids = place.Elements(TEI + "idno").Select(n => n.Value).ToList(),
            Links = place.Elements(TEI + "link")
                .Where(n => n.Attribute("target") != null)
                .Select(n => n.Attribute("target")!.Value)
                .ToList(),
            Description = place.Elements(TEI + "note")
                .Select(n => n.Value).FirstOrDefault()
        };
    }

    public IList<Entity> Parse(string tei)
    {
        ArgumentNullException.ThrowIfNull(tei);

        XDocument doc = XDocument.Parse(tei);
        List<Entity> entities = [];

        // persons
        foreach (XElement p in doc.Descendants(TEI + "person"))
        {
            Entity entity = ParsePerson(p);
            if (entity.Ids.Count > 0) entities.Add(entity);
        }

        // organizations
        foreach (XElement o in doc.Descendants(TEI + "org"))
        {
            Entity entity = ParsePerson(o, true);
            if (entity.Ids.Count > 0) entities.Add(entity);
        }

        // places
        foreach (XElement p in doc.Descendants(TEI + "place"))
        {
            Entity entity = ParsePlace(p);
            if (entity.Ids.Count > 0) entities.Add(entity);
        }

        return [.. entities.OrderBy(e => e.Type)
            .ThenBy(e => e.Names?.FirstOrDefault())
            .ThenBy(e => e.Ids[0])];
    }
}
