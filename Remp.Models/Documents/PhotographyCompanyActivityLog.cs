using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Remp.Models.Enums;

namespace Remp.Models.Documents;

public class PhotographyCompanyActivityLog
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string? UserId { get; set; }

    public string Email { get; set; } = null!;

    [BsonRepresentation(BsonType.String)]
    public PhotographyCompanyActivityType Action { get; set; }

    public bool Success { get; set; }

    public string? Message { get; set; }

    public string? IpAddress { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
