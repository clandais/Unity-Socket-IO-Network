# Simple Chat Sample

Dependencies:

https://openupm.com/packages/com.itisnajim.socketiounity/

Install via manifest.json :

``` json
{
    "scopedRegistries": [
        {
            "name": "package.openupm.com",
            "url": "https://package.openupm.com",
            "scopes": [
                "com.itisnajim.socketiounity"
            ]
        }
    ],
    "dependencies": {
        "com.itisnajim.socketiounity": "1.1.4"
    }
}
```

Parallel Sync for testing:

```json
{
    "dependencies": {
        "com.veriorpies.parrelsync": "https://github.com/VeriorPies/ParrelSync.git?path=/ParrelSync"
    }
}
```

## How to use

1. Run the server
2. Create a new `ChatSettings` asset in a `Resources`folder
3. Set the server url, port and token in the `ChatSettings` asset