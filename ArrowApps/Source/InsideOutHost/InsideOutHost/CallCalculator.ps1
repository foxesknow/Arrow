param
(
    [Parameter(Mandatory = $true)]
    [ValidateSet("Add", "Subtract", "Divide", "Multiply")]
    [string] $operation,

    [Parameter(Mandatory = $true)]
    [int] $x,
    
    [Parameter(Mandatory = $true)]
    [int] $y
)

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
        "CommandPath" : "Calculator/$operation",
        "Arguments" : [
            {
                "`$type" : "Decimal",
                "Value" : $x,
                "Name": "lhs"
            },
            {
                "`$type" : "Decimal",
                "Value" : $y,
                "Name": "rhs"
            }
        ]
    }
}
"@

Invoke-WebRequest http://localhost:8080/InsideOut/Process -ContentType application/json -Method POST -Body $json