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

        private static UpdateListingCaseDto CreateValidUpdateDto()
    {
        return new UpdateListingCaseDto
        {
            Title = "Updated House",
            Description = "Updated description",
            Price = 1250000m,
            Bedrooms = 4,
            Bathrooms = 3,
            Garages = 2,
            FloorArea = 200,
            SaleCategory = SaleCategory.Auction,
        };
    }

    private void VerifyNothingSavedOrLogged()
    {
        _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Never);
        _mockActivityLog.Verify(m => m.LogListingCaseAsync(
            It.IsAny<ListingCaseActivityType>(), It.IsAny<int>(), It.IsAny<string?>(),
            It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<string?>()), Times.Never);
        _mockActivityLog.Verify(m => m.LogPhotographyCompanyAsync(
            It.IsAny<PhotographyCompanyActivityType>(), It.IsAny<string>(), It.IsAny<string?>(),
            It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string?>()), Times.Never);
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


        // ---------- UpdateListingCaseAsync ----------

    [Fact]
    public async Task UpdateListingCaseAsync_WhenOwnerSendsValidDto_UpdatesFieldsSavesAndLogs()
    {
        //Arrange
        ListingCase listingCase = TestData.CreateListingCase();
        _mockListingCaseRepository.Setup(m => m.GetListingCaseAsync(1)).ReturnsAsync(listingCase);
        UpdateListingCaseDto dto = CreateValidUpdateDto();

        //Act
        UpdateListingCaseResponseDto result = await _listingCaseService.UpdateListingCaseAsync(
            1, dto, TestData.OwnerId, TestData.IpAddress);

        //Assert
        result.Title.Should().Be("Updated House");
        result.Price.Should().Be(1250000m);
        result.SaleCategory.Should().Be(SaleCategory.Auction);

        listingCase.Title.Should().Be("Updated House");
        listingCase.Bedrooms.Should().Be(4);
        listingCase.Street.Should().Be("1 Test Street");

        _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
        _mockActivityLog.Verify(m => m.LogListingCaseAsync(
            ListingCaseActivityType.Update, 1, TestData.OwnerId, true, It.IsAny<string>(), TestData.IpAddress), Times.Once);
    }

    [Fact]
    public async Task UpdateListingCaseAsync_WhenListingCaseDoesNotExist_ThrowsNotFound()
    {
        //Arrange
        _mockListingCaseRepository.Setup(m => m.GetListingCaseAsync(999)).ReturnsAsync((ListingCase?)null);

        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _listingCaseService.UpdateListingCaseAsync(999, CreateValidUpdateDto(), TestData.OwnerId, TestData.IpAddress));

        VerifyNothingSavedOrLogged();
    }

    [Fact]
    public async Task UpdateListingCaseAsync_WhenUserIsNotOwner_ThrowsForbiddenAndKeepsData()
    {
        //Arrange
        ListingCase listingCase = TestData.CreateListingCase();
        _mockListingCaseRepository.Setup(m => m.GetListingCaseAsync(1)).ReturnsAsync(listingCase);

        //Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _listingCaseService.UpdateListingCaseAsync(1, CreateValidUpdateDto(), TestData.OtherUserId, TestData.IpAddress));

        listingCase.Title.Should().Be("Test House");
        VerifyNothingSavedOrLogged();
    }

    [Fact]
    public async Task UpdateListingCaseAsync_WhenDtoIsInvalid_ThrowsBadRequestBeforeLoadingListingCase()
    {
        //Arrange
        UpdateListingCaseDto dto = CreateValidUpdateDto();
        dto.FloorArea = 0;

        //Act
        BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _listingCaseService.UpdateListingCaseAsync(1, dto, TestData.OwnerId, TestData.IpAddress));

        //Assert
        exception.Errors.Should().Contain("FloorArea must be greater than 0");
        _mockListingCaseRepository.Verify(m => m.GetListingCaseAsync(It.IsAny<int>()), Times.Never);
        VerifyNothingSavedOrLogged();
    }

        // ---------- UpdateListingCaseStatusByIdAsync ----------

    [Theory]
    [InlineData(ListcaseStatus.Created, ListcaseStatus.Pending)]
    [InlineData(ListcaseStatus.Pending, ListcaseStatus.InReview)]
    [InlineData(ListcaseStatus.InReview, ListcaseStatus.Delivered)]
    [InlineData(ListcaseStatus.Pending, ListcaseStatus.Created)]
    [InlineData(ListcaseStatus.InReview, ListcaseStatus.Pending)]
    public async Task UpdateListingCaseStatusByIdAsync_WhenTransitionIsAllowed_UpdatesStatusAndLogs(
        ListcaseStatus from, ListcaseStatus to)
    {
        //Arrange
        ListingCase listingCase = TestData.CreateListingCase(status: from);
        _mockListingCaseRepository.Setup(m => m.GetListingCaseAsync(1)).ReturnsAsync(listingCase);
        UpdateListingCaseStatusDto dto = new UpdateListingCaseStatusDto { Status = to };

        //Act
        UpdateListingCaseStatusResponseDto result = await _listingCaseService.UpdateListingCaseStatusByIdAsync(
            dto, 1, TestData.OwnerId, TestData.IpAddress);

        //Assert
        result.Id.Should().Be(1);
        result.PreviousStatus.Should().Be(from);
        result.CurrentStatus.Should().Be(to);
        listingCase.ListcaseStatus.Should().Be(to);
        _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
        _mockActivityLog.Verify(m => m.LogListingCaseAsync(
            ListingCaseActivityType.StatusChange, 1, TestData.OwnerId, true, It.IsAny<string>(), TestData.IpAddress), Times.Once);
    }

    [Theory]
    [InlineData(ListcaseStatus.Created, ListcaseStatus.InReview)]
    [InlineData(ListcaseStatus.Created, ListcaseStatus.Delivered)]
    [InlineData(ListcaseStatus.Pending, ListcaseStatus.Delivered)]
    [InlineData(ListcaseStatus.InReview, ListcaseStatus.Created)]
    [InlineData(ListcaseStatus.Delivered, ListcaseStatus.InReview)]
    [InlineData(ListcaseStatus.Delivered, ListcaseStatus.Created)]
    [InlineData(ListcaseStatus.Created, ListcaseStatus.Created)]
    public async Task UpdateListingCaseStatusByIdAsync_WhenTransitionIsNotAllowed_ThrowsBadRequestAndKeepsStatus(
        ListcaseStatus from, ListcaseStatus to)
    {
        //Arrange
        ListingCase listingCase = TestData.CreateListingCase(status: from);
        _mockListingCaseRepository.Setup(m => m.GetListingCaseAsync(1)).ReturnsAsync(listingCase);
        UpdateListingCaseStatusDto dto = new UpdateListingCaseStatusDto { Status = to };

        //Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _listingCaseService.UpdateListingCaseStatusByIdAsync(dto, 1, TestData.OwnerId, TestData.IpAddress));

        listingCase.ListcaseStatus.Should().Be(from);
        VerifyNothingSavedOrLogged();
    }

    [Fact]
    public async Task UpdateListingCaseStatusByIdAsync_WhenStatusIsNotDefined_ThrowsBadRequest()
    {
        //Arrange
        UpdateListingCaseStatusDto dto = new UpdateListingCaseStatusDto { Status = (ListcaseStatus)99 };

        //Act
        BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _listingCaseService.UpdateListingCaseStatusByIdAsync(dto, 1, TestData.OwnerId, TestData.IpAddress));

        //Assert
        exception.Errors.Should().Contain("Invalid status");
        _mockListingCaseRepository.Verify(m => m.GetListingCaseAsync(It.IsAny<int>()), Times.Never);
        VerifyNothingSavedOrLogged();
    }

    [Fact]
    public async Task UpdateListingCaseStatusByIdAsync_WhenListingCaseDoesNotExist_ThrowsNotFound()
    {
        //Arrange
        _mockListingCaseRepository.Setup(m => m.GetListingCaseAsync(999)).ReturnsAsync((ListingCase?)null);
        UpdateListingCaseStatusDto dto = new UpdateListingCaseStatusDto { Status = ListcaseStatus.Pending };

        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _listingCaseService.UpdateListingCaseStatusByIdAsync(dto, 999, TestData.OwnerId, TestData.IpAddress));

        VerifyNothingSavedOrLogged();
    }

    [Fact]
    public async Task UpdateListingCaseStatusByIdAsync_WhenAgentTriesToChangeStatus_ThrowsForbidden()
    {
        //Arrange
        ListingCase listingCase = TestData.AssignAgent(TestData.CreateListingCase());
        _mockListingCaseRepository.Setup(m => m.GetListingCaseAsync(1)).ReturnsAsync(listingCase);
        UpdateListingCaseStatusDto dto = new UpdateListingCaseStatusDto { Status = ListcaseStatus.Pending };

        //Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _listingCaseService.UpdateListingCaseStatusByIdAsync(dto, 1, TestData.AgentId, TestData.IpAddress));

        listingCase.ListcaseStatus.Should().Be(ListcaseStatus.Created);
        VerifyNothingSavedOrLogged();
    }

        // ---------- AddAgentToListingCaseAsync ----------

    [Fact]
    public async Task AddAgentToListingCaseAsync_WhenAgentBelongsToCompany_AssignsAgentAndLogs()
    {
        //Arrange
        ListingCase listingCase = TestData.CreateListingCase();
        Agent agent = TestData.CreateAgent();
        _mockListingCaseRepository.Setup(m => m.GetListingCaseWithAgentAsync(1)).ReturnsAsync(listingCase);
        _mockAgentRepository.Setup(m => m.GetAgentByIdAsync(TestData.AgentId)).ReturnsAsync(agent);
        AddAgentToListingCaseDto dto = new AddAgentToListingCaseDto { Id = TestData.AgentId };

        //Act
        AddAgentToListingCaseResponseDto result = await _listingCaseService.AddAgentToListingCaseAsync(
            dto, TestData.OwnerId, 1, TestData.IpAddress, TestData.OwnerEmail);

        //Assert
        result.Id.Should().Be(TestData.AgentId);
        result.ListingCaseId.Should().Be(1);
        listingCase.Agents.Should().Contain(agent);
        _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
        _mockActivityLog.Verify(m => m.LogPhotographyCompanyAsync(
            PhotographyCompanyActivityType.AddAgentToListingCase, TestData.OwnerEmail, TestData.OwnerId,
            true, It.IsAny<string>(), TestData.IpAddress), Times.Once);
    }

    [Fact]
    public async Task AddAgentToListingCaseAsync_WhenAgentIdIsEmpty_ThrowsBadRequest()
    {
        //Arrange
        AddAgentToListingCaseDto dto = new AddAgentToListingCaseDto { Id = "" };

        //Act
        BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _listingCaseService.AddAgentToListingCaseAsync(dto, TestData.OwnerId, 1, TestData.IpAddress, TestData.OwnerEmail));

        //Assert
        exception.Errors.Should().Contain("Agent id can not be empty!");
        _mockListingCaseRepository.Verify(m => m.GetListingCaseWithAgentAsync(It.IsAny<int>()), Times.Never);
        VerifyNothingSavedOrLogged();
    }

    [Fact]
    public async Task AddAgentToListingCaseAsync_WhenListingCaseDoesNotExist_ThrowsNotFound()
    {
        //Arrange
        _mockListingCaseRepository.Setup(m => m.GetListingCaseWithAgentAsync(999)).ReturnsAsync((ListingCase?)null);
        AddAgentToListingCaseDto dto = new AddAgentToListingCaseDto { Id = TestData.AgentId };

        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _listingCaseService.AddAgentToListingCaseAsync(dto, TestData.OwnerId, 999, TestData.IpAddress, TestData.OwnerEmail));

        _mockAgentRepository.Verify(m => m.GetAgentByIdAsync(It.IsAny<string>()), Times.Never);
        VerifyNothingSavedOrLogged();
    }

    [Fact]
    public async Task AddAgentToListingCaseAsync_WhenUserIsNotOwner_ThrowsForbidden()
    {
        //Arrange
        ListingCase listingCase = TestData.CreateListingCase();
        _mockListingCaseRepository.Setup(m => m.GetListingCaseWithAgentAsync(1)).ReturnsAsync(listingCase);
        AddAgentToListingCaseDto dto = new AddAgentToListingCaseDto { Id = TestData.AgentId };

        //Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _listingCaseService.AddAgentToListingCaseAsync(dto, TestData.OtherUserId, 1, TestData.IpAddress, TestData.OwnerEmail));

        listingCase.Agents.Should().BeEmpty();
        VerifyNothingSavedOrLogged();
    }

    [Fact]
    public async Task AddAgentToListingCaseAsync_WhenAgentDoesNotExist_ThrowsNotFound()
    {
        //Arrange
        ListingCase listingCase = TestData.CreateListingCase();
        _mockListingCaseRepository.Setup(m => m.GetListingCaseWithAgentAsync(1)).ReturnsAsync(listingCase);
        _mockAgentRepository.Setup(m => m.GetAgentByIdAsync("missing-agent")).ReturnsAsync((Agent?)null);
        AddAgentToListingCaseDto dto = new AddAgentToListingCaseDto { Id = "missing-agent" };

        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _listingCaseService.AddAgentToListingCaseAsync(dto, TestData.OwnerId, 1, TestData.IpAddress, TestData.OwnerEmail));

        listingCase.Agents.Should().BeEmpty();
        VerifyNothingSavedOrLogged();
    }

    [Fact]
    public async Task AddAgentToListingCaseAsync_WhenAgentIsAlreadyAssigned_ThrowsBadRequest()
    {
        //Arrange
        ListingCase listingCase = TestData.AssignAgent(TestData.CreateListingCase());
        _mockListingCaseRepository.Setup(m => m.GetListingCaseWithAgentAsync(1)).ReturnsAsync(listingCase);
        _mockAgentRepository.Setup(m => m.GetAgentByIdAsync(TestData.AgentId)).ReturnsAsync(TestData.CreateAgent());
        AddAgentToListingCaseDto dto = new AddAgentToListingCaseDto { Id = TestData.AgentId };

        //Act
        BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _listingCaseService.AddAgentToListingCaseAsync(dto, TestData.OwnerId, 1, TestData.IpAddress, TestData.OwnerEmail));

        //Assert
        exception.Message.Should().Be("This agent has already been assigned to this listing case.");
        listingCase.Agents.Should().HaveCount(1);
        VerifyNothingSavedOrLogged();
    }

    [Fact]
    public async Task AddAgentToListingCaseAsync_WhenAgentIsNotInCompany_ThrowsBadRequest()
    {
        //Arrange
        ListingCase listingCase = TestData.CreateListingCase();
        Agent agentFromAnotherCompany = TestData.CreateAgent(companyId: "another-company");
        _mockListingCaseRepository.Setup(m => m.GetListingCaseWithAgentAsync(1)).ReturnsAsync(listingCase);
        _mockAgentRepository.Setup(m => m.GetAgentByIdAsync(TestData.AgentId)).ReturnsAsync(agentFromAnotherCompany);
        AddAgentToListingCaseDto dto = new AddAgentToListingCaseDto { Id = TestData.AgentId };

        //Act
        BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _listingCaseService.AddAgentToListingCaseAsync(dto, TestData.OwnerId, 1, TestData.IpAddress, TestData.OwnerEmail));

        //Assert
        exception.Message.Should().Be("This agent is not in your company.");
        listingCase.Agents.Should().BeEmpty();
        VerifyNothingSavedOrLogged();
    }
}