using Remp.Models.Entities;
using Remp.Models.Enums;
using Microsoft.AspNetCore.Http;

namespace Remp.Tests.TestHelpers;

public static class TestData
{
    public const string OwnerId = "owner-1";
    public const string AgentId = "agent-1";
    public const string OtherUserId = "other-user";
    public const string OwnerEmail = "photo@recam.test";
    public const string IpAddress = "127.0.0.1";

    public static ListingCase CreateListingCase(
        int id = 1,
        string ownerId = OwnerId,
        ListcaseStatus status = ListcaseStatus.Created)
    {
        return new ListingCase
        {
            Id = id,
            Title = "Test House",
            Description = "A test listing case",
            Street = "1 Test Street",
            City = "Mascot",
            State = "NSW",
            Postcode = "2020",
            Longitude = 151.19m,
            Latitude = -33.92m,
            Price = 1000000m,
            Bedrooms = 3,
            Bathrooms = 2,
            Garages = 1,
            FloorArea = 150,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false,
            PropertyType = PropertyType.House,
            SaleCategory = SaleCategory.ForSale,
            ListcaseStatus = status,
            UserId = ownerId,
        };
    }

    public static Agent CreateAgent(string id = AgentId, string? companyId = OwnerId)
    {
        Agent agent = new Agent
        {
            Id = id,
            AgentFirstName = "John",
            AgentLastName = "Smith",
            CompanyName = "Test Realty",
            User = new User { Id = id, Email = $"{id}@recam.test" },
        };

        if (companyId != null)
        {
            agent.PhotographyCompanies.Add(new PhotographyCompany
            {
                Id = companyId,
                PhotographyCompanyName = "Test Photography",
            });
        }

        return agent;
    }

    public static ListingCase AssignAgent(ListingCase listingCase, string agentId = AgentId)
    {
        listingCase.Agents.Add(CreateAgent(agentId));
        return listingCase;
    }

    public static MediaAsset AddMediaAsset(
        ListingCase listingCase,
        int id,
        MediaType mediaType = MediaType.Photos)
    {
        MediaAsset mediaAsset = new MediaAsset
        {
            Id = id,
            MediaType = mediaType,
            MediaUrl = $"listing-cases/{listingCase.Id}/media-{id}.jpg",
            UploadedAt = DateTime.UtcNow,
            ListingCaseId = listingCase.Id,
            ListingCase = listingCase,
            UserId = listingCase.UserId,
            IsDeleted = false,
        };

        listingCase.MediaAssets.Add(mediaAsset);
        return mediaAsset;
    }

    public static IFormFile CreateFormFile(string fileName, long length = 1024, string contentType = "image/jpeg")
    {
        MemoryStream stream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
        return new FormFile(stream, 0, length, "files", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType,
        };
    }
}