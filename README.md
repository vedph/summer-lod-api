# Summer LOD API

This API provides backend support for the [Summer LOD app](https://github.com/vedph/summer-lod-app).

## Docker

🐋 Quick Docker image build:

```ps1
docker build . -t vedph2020/summer-lod-api:0.0.4 -t vedph2020/summer-lod-api:latest
```

Replace with the current version.

Docker run (or use the [compose script](./docker-compose.yml)):

```ps1
docker run -d --name summer-lod-api -p 5275:8080 vedph2020/summer-lod-api:0.0.4
```

## API

The API is publicly accessible endpoints under CORS and uses JSON for both input and output.

### XML Rendition

🎯 `POST xml/rendition`: transform the received XML using the received XSLT.

🔼 Input:

```json
{
    "xml": "the XML code",
    "xslt": "the XSLT 1.0 code"
}
```

🔽 Output:

```json
{
    "result": "HTML code",
    "error": "error message"
}
```

When an error occurs, `result` is undefined and `error` has a value. Otherwise, `result` has a value and `error` is undefined.

### Entities Parsing

🎯 `POST xml/entities`: parse the received XML TEI extracting entities from it. To make things quicker, this implies some assumptions about the TEI encoding of entities.

🔼 Input:

```json
{
    "xml": "the XML code",
}
```

🔽 Output:

```json
{
    "entities": [
        {
            "ids": [],
            "type": "type: person organization place",
            "names": [],
            "links": [],
            "description": "..."
        }
    ],
    "error": "error message"
}
```

On return, `error` is set instead of entities in case of errors. Entities have at least 1 ID, 1 name and a type. Other data are optional.

### Prettify XML

🎯 `POST xml/prettify`: prettify the received XML.

🔼 Input:

```json
{
    "xml": "the XML code",
}
```

🔽 Output:

```json
{
    "xml": "prettified XML",
    "error": "error message"
}
```

### Uglify XML

🎯 `POST xml/uglify`: uglify the received XML.

🔼 Input:

```json
{
    "xml": "the XML code",
}
```

🔽 Output:

```json
{
    "xml": "uglified XML",
    "error": "error message"
}
```

## History

- 2024-09-14: updated packages.
- 2024-06-25: updated packages.

### 0.0.3

- 2024-06-18: fixed typo in entities parser.

### 0.0.2
 
- 2024-06-10: minor changes.
- 2024-05-24: updated packages.
