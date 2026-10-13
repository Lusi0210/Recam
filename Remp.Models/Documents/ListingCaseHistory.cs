using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Remp.Models.Enums;

namespace Remp.Models.Documents;

public class ListingCaseHistory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public int ListingCaseId{get;set;}
    public string? UserId { get; set; }

    [BsonRepresentation(BsonType.String)]
    public ListingCaseActivityType Action { get; set; }

    public bool Success { get; set; }

    public string? Message { get; set; }

    public string? IpAddress { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
