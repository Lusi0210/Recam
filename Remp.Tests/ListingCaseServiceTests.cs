using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Remp.Common.Settings;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Common;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using Remp.Service.Services;
using Remp.Tests.TestHelpers;
using Remp.Service.Validators;
using Remp.Common.Exceptions;

namespace Remp.Tests;

public class ListingCaseServiceTests
{
    private readonly Mock<IListingCaseRepository> _mockListingCaseRepository;
    private readonly Mock<IAgentRepository> _mockAgentRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IBlobStorageService> _mockBlobStorage;
    private readonly Mock<IActivityLogService> _mockActivityLog;
    private readonly ListingCaseService _listingCaseService;

    public ListingCaseServiceTests()
    {
        _mockListingCaseRepository = new Mock<IListingCaseRepository>();
        _mockAgentRepository = new Mock<IAgentRepository>();
        _mockUnitOfWork = MockHelper.CreateUnitOfWork();
        _mockBlobStorage = MockHelper.CreateBlobStorage();
        _mockActivityLog = MockHelper.CreateActivityLog();

        _listingCaseService = new ListingCaseService(
            _mockListingCaseRepository.Object,
            new CreateListingCaseValidator(),
            _mockUnitOfWork.Object,
            new ListingCaseQueryDtoValidator(),
            new UpdateListingCaseValidator(),
            new UpdateListingCaseStatusDtoValidator(),
            MockHelper.CreateMapper(),
            _mockAgentRepository.Object,
            new AddAgentToListingCaseDtoValidator(),
            _mockBlobStorage.Object,
            Options.Create(new FrontendSettings { BaseUrl = "http://localhost:5173" }),
            _mockActivityLog.Object);
    }

    private static CreateListingCaseDto CreateValidDto()
    {
        return new CreateListingCaseDto
        {
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
            PropertyType = PropertyType.House,
            SaleCategory = SaleCategory.ForSale,
        };
    }

    //Naming pattern for test method
    //What_When_Should
    [Fact]
    public async Task CreateListingCaseAsync_WhenDtoIsValid_ReturnsNewListingCaseId()
    {
        //Arrange
        CreateListingCaseDto dto = CreateValidDto();
        _mockListingCaseRepository
            .Setup(m => m.Add(It.IsAny<ListingCase>()))
            .Callback<ListingCase>(listingCase => listingCase.Id = 1);

        //Act
        int result = await _listingCaseService.CreateListingCaseAsync(dto, "user-1", "127.0.0.1");

        //Assert
        result.Should().Be(1);
        _mockListingCaseRepository.Verify(m => m.Add(It.Is<ListingCase>(l =>
            l.Title == "Test House" &&
            l.UserId == "user-1" &&
            l.ListcaseStatus == ListcaseStatus.Created)), Times.Once);
        _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
        _mockActivityLog.Verify(m => m.LogListingCaseAsync(
            ListingCaseActivityType.Create, 1, "user-1", true, It.IsAny<string>(), "127.0.0.1"), Times.Once);
    }

    [Fact]
    public async Task CreateListingCaseAsync_WhenSaveFails_ThrowsDbUpdateException()
    {
        //Arrange
        CreateListingCaseDto dto = CreateValidDto();
        _mockUnitOfWork
            .Setup(m => m.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException("Add listing case failed"));

        //Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(
            () => _listingCaseService.CreateListingCaseAsync(dto, "user-1", "127.0.0.1"));

        _mockActivityLog.Verify(m => m.LogListingCaseAsync(
            It.IsAny<ListingCaseActivityType>(), It.IsAny<int>(), It.IsAny<string?>(),
            It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<string?>()), Times.Never);
    }

        // ---------- External storage mock (IBlobStorageService) ----------

    [Fact]
    public async Task GetListingCaseDetailsByIdAsync_WhenOwnerRequests_ReturnsReadUrlsFromBlobStorage()
    {
        //Arrange
        ListingCase listingCase = TestData.CreateListingCase();
        TestData.AddMediaAsset(listingCase, 1);
        TestData.AddMediaAsset(listingCase, 2);
        _mockListingCaseRepository
            .Setup(m => m.GetListingCaseDetailsByIdAsync(1))
            .ReturnsAsync(listingCase);

        //Act
        GetListingCaseDetailsResponseDto result =
            await _listingCaseService.GetListingCaseDetailsByIdAsync(1, TestData.OwnerId);

        //Assert
        result.MediaAssets.Should().HaveCount(2);
        result.MediaAssets.Select(m => m.MediaUrl).Should().BeEquivalentTo(
            "https://fake-storage/listing-cases/1/media-1.jpg?sas=test",
            "https://fake-storage/listing-cases/1/media-2.jpg?sas=test");
        _mockBlobStorage.Verify(m => m.GetReadUrl(It.IsAny<string>()), Times.Exactly(2));
    }

    [Fact]
    public async Task GetListingCaseDetailsByIdAsync_WhenUserIsNeitherOwnerNorAgent_ThrowsForbidden()
    {
        //Arrange
        ListingCase listingCase = TestData.AssignAgent(TestData.CreateListingCase());
        TestData.AddMediaAsset(listingCase, 1);
        _mockListingCaseRepository
            .Setup(m => m.GetListingCaseDetailsByIdAsync(1))
            .ReturnsAsync(listingCase);

        //Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _listingCaseService.GetListingCaseDetailsByIdAsync(1, TestData.OtherUserId));

        _mockBlobStorage.Verify(m => m.GetReadUrl(It.IsAny<string>()), Times.Never);
    }
}