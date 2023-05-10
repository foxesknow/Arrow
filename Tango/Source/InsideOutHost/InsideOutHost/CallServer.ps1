$endpoint = "http://localhost:8080/InsideOut/Process"

$json = @"
{
    "PublisherID": {
        "ServerName" : "fuji",
        "InstanceName" : "InsideOutHost"
    },
    "RequestID" : {
        "ApplicationID": "52c9ccd8-ea78-4b75-bdf6-080dbe29c811",
        "CorrelationID": "b81bc193-2fe1-46e9-90fc-4082275c2372"
    },
    "NodeFunction" : "Execute",
    "Request" : {
        "`$type" : "Execute",
        "CommandPath" : "Echo/Echo",
        "Arguments" : [
            {
                "`$type" : "String",
                "Value" : "Hello world",
                "Name": "message"
            }
        ]
    }
}
"@

Invoke-WebRequest http://localhost:8080/InsideOut/Process -ContentType application/json -Method POST -Body $json