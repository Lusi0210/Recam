using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Remp.Common.Exceptions;
using Remp.Models.Entities;
using Remp.Models.Enums;
using Remp.Repository.Common;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;
using Remp.Service.Services;
using Remp.Service.Validators;
using Remp.Tests.TestHelpers;

namespace Remp.Tests;

public class MediaAssetServiceTests
{
    private const long OneMegabyte = 1024 * 1024;

    private readonly Mock<IListingCaseRepository> _mockListingCaseRepository;
    private readonly Mock<IMediaAssetRepository> _mockMediaAssetRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IBlobStorageService> _mockBlobStorage;
    private readonly Mock<IActivityLogService> _mockActivityLog;
    private readonly MediaAssetService _mediaAssetService;

    public MediaAssetServiceTests()
    {
        _mockListingCaseRepository = new Mock<IListingCaseRepository>();
        _mockMediaAssetRepository = new Mock<IMediaAssetRepository>();
        _mockUnitOfWork = MockHelper.CreateUnitOfWork();
        _mockBlobStorage = MockHelper.CreateBlobStorage();
        _mockActivityLog = MockHelper.CreateActivityLog();

        _mediaAssetService = new MediaAssetService(
            _mockListingCaseRepository.Object,
            _mockMediaAssetRepository.Object,
            _mockBlobStorage.Object,
            _mockUnitOfWork.Object,
            MockHelper.CreateMapper(),
            NullLogger<MediaAssetService>.Instance,
            _mockActivityLog.Object,
            new SetCoverImageDtoValidator(),
            new AgentSelectMediaDtoValidator());
    }

    // ---------- Helpers ----------

    private ListingCase SetupListingCaseForUpload()
    {
        ListingCase listingCase = TestData.CreateListingCase();
        _mockListingCaseRepository.Setup(m => m.GetListingCaseAsync(1)).ReturnsAsync(listingCase);
        return listingCase;
    }

    private void VerifyNothingSavedOrLogged()
    {
        _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Never);
        _mockActivityLog.Verify(m => m.LogListingCaseAsync(
            It.IsAny<ListingCaseActivityType>(), It.IsAny<int>(), It.IsAny<string?>(),
            It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<string?>()), Times.Never);
    }

    private void VerifyNothingUploadedSavedOrLogged()
    {
        _mockBlobStorage.Verify(m => m.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _mockMediaAssetRepository.Verify(m => m.Add(It.IsAny<MediaAsset>()), Times.Never);
        VerifyNothingSavedOrLogged();
    }

        // ---------- UploadMediaAssetsAsync ----------

    [Fact]
    public async Task UploadMediaAssetsAsync_WhenOwnerUploadsValidPhotos_UploadsToBlobSavesAndLogs()
    {
        //Arrange
        SetupListingCaseForUpload();
        List<IFormFile> files = new List<IFormFile>
        {
            TestData.CreateFormFile("front.jpg"),
            TestData.CreateFormFile("kitchen.png", contentType: "image/png"),
        };

        //Act
        List<MediaAssetDto> result = await _mediaAssetService.UploadMediaAssetsAsync(
            1, MediaType.Photos, files, TestData.OwnerId, TestData.IpAddress);

        //Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(m =>
            m.MediaType == MediaType.Photos &&
            m.MediaUrl.StartsWith("https://fake-storage/listing-cases/1/") &&
            m.MediaUrl.EndsWith("?sas=test"));

        _mockBlobStorage.Verify(m => m.UploadAsync(
            It.IsAny<Stream>(),
            It.Is<string>(name => name.StartsWith("listing-cases/1/") && name.EndsWith(".jpg")),
            "image/jpeg"), Times.Once);
        _mockBlobStorage.Verify(m => m.UploadAsync(
            It.IsAny<Stream>(),
            It.Is<string>(name => name.StartsWith("listing-cases/1/") && name.EndsWith(".png")),
            "image/png"), Times.Once);
        _mockMediaAssetRepository.Verify(m => m.Add(It.Is<MediaAsset>(a =>
            a.ListingCaseId == 1 &&
            a.UserId == TestData.OwnerId &&
            a.MediaType == MediaType.Photos)), Times.Exactly(2));
        _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
        _mockActivityLog.Verify(m => m.LogListingCaseAsync(
            ListingCaseActivityType.UploadMedia, 1, TestData.OwnerId, true, It.IsAny<string>(), TestData.IpAddress), Times.Once);
    }

    [Fact]
    public async Task UploadMediaAssetsAsync_WhenListingCaseDoesNotExist_ThrowsNotFound()
    {
        //Arrange
        _mockListingCaseRepository.Setup(m => m.GetListingCaseAsync(999)).ReturnsAsync((ListingCase?)null);
        List<IFormFile> files = new List<IFormFile> { TestData.CreateFormFile("front.jpg") };

        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _mediaAssetService.UploadMediaAssetsAsync(999, MediaType.Photos, files, TestData.OwnerId, TestData.IpAddress));

        VerifyNothingUploadedSavedOrLogged();
    }

    [Fact]
    public async Task UploadMediaAssetsAsync_WhenUserIsNotOwner_ThrowsForbidden()
    {
        //Arrange
        SetupListingCaseForUpload();
        List<IFormFile> files = new List<IFormFile> { TestData.CreateFormFile("front.jpg") };

        //Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _mediaAssetService.UploadMediaAssetsAsync(1, MediaType.Photos, files, TestData.OtherUserId, TestData.IpAddress));

        VerifyNothingUploadedSavedOrLogged();
    }

    [Theory]
    [InlineData(MediaType.Photos, "document.pdf")]
    [InlineData(MediaType.Videography, "clip.avi")]
    [InlineData(MediaType.FloorPlan, "plan.gif")]
    [InlineData(MediaType.VRTour, "tour.png")]
    public async Task UploadMediaAssetsAsync_WhenFileTypeIsNotAllowed_ThrowsBadRequest(MediaType mediaType, string fileName)
    {
        //Arrange
        SetupListingCaseForUpload();
        List<IFormFile> files = new List<IFormFile> { TestData.CreateFormFile(fileName) };

        //Act
        BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _mediaAssetService.UploadMediaAssetsAsync(1, mediaType, files, TestData.OwnerId, TestData.IpAddress));

        //Assert
        exception.Errors.Should().ContainSingle(e => e.Contains("is not allowed"));
        VerifyNothingUploadedSavedOrLogged();
    }

    [Fact]
    public async Task UploadMediaAssetsAsync_WhenPhotoIsLargerThan20MB_ThrowsBadRequest()
    {
        //Arrange
        SetupListingCaseForUpload();
        List<IFormFile> files = new List<IFormFile> { TestData.CreateFormFile("huge.jpg", length: 21 * OneMegabyte) };

        //Act
        BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _mediaAssetService.UploadMediaAssetsAsync(1, MediaType.Photos, files, TestData.OwnerId, TestData.IpAddress));

        //Assert
        exception.Errors.Should().ContainSingle(e => e.Contains("exceeds the 20MB limit"));
        VerifyNothingUploadedSavedOrLogged();
    }

    [Fact]
    public async Task UploadMediaAssetsAsync_WhenFileIsEmpty_ThrowsBadRequest()
    {
        //Arrange
        SetupListingCaseForUpload();
        List<IFormFile> files = new List<IFormFile> { TestData.CreateFormFile("empty.jpg", length: 0) };

        //Act
        BadRequestException exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _mediaAssetService.UploadMediaAssetsAsync(1, MediaType.Photos, files, TestData.OwnerId, TestData.IpAddress));

        //Assert
        exception.Errors.Should().ContainSingle(e => e.Contains("file is empty"));
        VerifyNothingUploadedSavedOrLogged();
    }

    [Fact]
    public async Task UploadMediaAssetsAsync_WhenSaveFails_DeletesUploadedBlobsAndRethrows()
    {
        //Arrange
        SetupListingCaseForUpload();
        List<string> uploadedBlobNames = new List<string>();
        _mockBlobStorage
            .Setup(m => m.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback<Stream, string, string>((stream, blobName, contentType) => uploadedBlobNames.Add(blobName))
            .Returns(Task.CompletedTask);
        _mockUnitOfWork
            .Setup(m => m.SaveChangesAsync())
            .ThrowsAsync(new DbUpdateException("Save media failed"));
        List<IFormFile> files = new List<IFormFile>
        {
            TestData.CreateFormFile("front.jpg"),
            TestData.CreateFormFile("back.jpg"),
        };

        //Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(
            () => _mediaAssetService.UploadMediaAssetsAsync(1, MediaType.Photos, files, TestData.OwnerId, TestData.IpAddress));

        uploadedBlobNames.Should().HaveCount(2);
        foreach (string blobName in uploadedBlobNames)
        {
            _mockBlobStorage.Verify(m => m.DeleteAsync(blobName), Times.Once);
        }
        _mockActivityLog.Verify(m => m.LogListingCaseAsync(
            It.IsAny<ListingCaseActivityType>(), It.IsAny<int>(), It.IsAny<string?>(),
            It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<string?>()), Times.Never);
    }

        // ---------- GetListingCaseMediaAssetsAsync ----------

    [Fact]
    public async Task GetListingCaseMediaAssetsAsync_WhenOwnerRequests_GroupsMediaByTypeWithReadUrls()
    {
        //Arrange
        ListingCase listingCase = TestData.CreateListingCase();
        TestData.AddMediaAsset(listingCase, 1, MediaType.Photos);
        TestData.AddMediaAsset(listingCase, 2, MediaType.Photos);
        TestData.AddMediaAsset(listingCase, 3, MediaType.FloorPlan);
        _mockListingCaseRepository.Setup(m => m.GetListingCaseDetailsByIdAsync(1)).ReturnsAsync(listingCase);

        //Act
        List<GetListingCaseMediaAssetsResponseDto> result =
            await _mediaAssetService.GetListingCaseMediaAssetsAsync(1, TestData.OwnerId);

        //Assert
        result.Should().HaveCount(2);

        GetListingCaseMediaAssetsResponseDto photos = result.Single(g => g.MediaType == MediaType.Photos);
        photos.MediaAssets.Should().HaveCount(2);

        GetListingCaseMediaAssetsResponseDto floorPlans = result.Single(g => g.MediaType == MediaType.FloorPlan);
        floorPlans.MediaAssets.Should().ContainSingle();
        floorPlans.MediaAssets.Single().MediaUrl.Should().Be("https://fake-storage/listing-cases/1/media-3.jpg?sas=test");
    }

    [Fact]
    public async Task GetListingCaseMediaAssetsAsync_WhenUserIsNeitherOwnerNorAgent_ThrowsForbidden()
    {
        //Arrange
        ListingCase listingCase = TestData.AssignAgent(TestData.CreateListingCase());
        TestData.AddMediaAsset(listingCase, 1);
        _mockListingCaseRepository.Setup(m => m.GetListingCaseDetailsByIdAsync(1)).ReturnsAsync(listingCase);

        //Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _mediaAssetService.GetListingCaseMediaAssetsAsync(1, TestData.OtherUserId));

        _mockBlobStorage.Verify(m => m.GetReadUrl(It.IsAny<string>()), Times.Never);
    }

    // ---------- DownloadMediaAssetAsync ----------

    [Fact]
    public async Task DownloadMediaAssetAsync_WhenAssignedAgentRequests_ReturnsFileFromBlobStorage()
    {
        //Arrange
        ListingCase listingCase = TestData.AssignAgent(TestData.CreateListingCase());
        MediaAsset mediaAsset = TestData.AddMediaAsset(listingCase, 1);
        _mockMediaAssetRepository.Setup(m => m.GetMediaAssetWithListingCaseAsync(1)).ReturnsAsync(mediaAsset);

        MemoryStream content = new MemoryStream(new byte[] { 1, 2, 3 });
        (Stream Content, string ContentType, string FileName) blobFile = (content, "image/jpeg", "media-1.jpg");
        _mockBlobStorage
            .Setup(m => m.DownloadFileAsync("listing-cases/1/media-1.jpg"))
            .ReturnsAsync(blobFile);

        //Act
        (Stream Content, string ContentType, string FileName) result =
            await _mediaAssetService.DownloadMediaAssetAsync(1, TestData.AgentId);

        //Assert
        result.Content.Should().BeSameAs(content);
        result.ContentType.Should().Be("image/jpeg");
        result.FileName.Should().Be("media-1.jpg");
    }

    [Fact]
    public async Task DownloadMediaAssetAsync_WhenUserIsNeitherOwnerNorAgent_ThrowsForbidden()
    {
        //Arrange
        ListingCase listingCase = TestData.CreateListingCase();
        MediaAsset mediaAsset = TestData.AddMediaAsset(listingCase, 1);
        _mockMediaAssetRepository.Setup(m => m.GetMediaAssetWithListingCaseAsync(1)).ReturnsAsync(mediaAsset);

        //Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _mediaAssetService.DownloadMediaAssetAsync(1, TestData.OtherUserId));

        _mockBlobStorage.Verify(m => m.DownloadFileAsync(It.IsAny<string>()), Times.Never);
    }


        // ---------- DeleteMediaAssetAsync ----------

    [Fact]
    public async Task DeleteMediaAssetAsync_WhenOwnerDeletes_SoftDeletesAndLogs()
    {
        //Arrange
        ListingCase listingCase = TestData.CreateListingCase();
        MediaAsset mediaAsset = TestData.AddMediaAsset(listingCase, 1);
        mediaAsset.IsHero = true;
        mediaAsset.IsSelected = true;
        _mockMediaAssetRepository.Setup(m => m.GetMediaAssetByIdAsync(1)).ReturnsAsync(mediaAsset);

        //Act
        await _mediaAssetService.DeleteMediaAssetAsync(1, TestData.OwnerId, TestData.IpAddress);

        //Assert
        mediaAsset.IsDeleted.Should().BeTrue();
        mediaAsset.IsHero.Should().BeFalse();
        mediaAsset.IsSelected.Should().BeFalse();
        _mockUnitOfWork.Verify(m => m.SaveChangesAsync(), Times.Once);
        _mockActivityLog.Verify(m => m.LogListingCaseAsync(
            ListingCaseActivityType.DeleteMedia, 1, TestData.OwnerId, true, It.IsAny<string>(), TestData.IpAddress), Times.Once);
        _mockBlobStorage.Verify(m => m.DeleteAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DeleteMediaAssetAsync_WhenMediaDoesNotExist_ThrowsNotFound()
    {
        //Arrange
        _mockMediaAssetRepository.Setup(m => m.GetMediaAssetByIdAsync(999)).ReturnsAsync((MediaAsset?)null);

        //Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _mediaAssetService.DeleteMediaAssetAsync(999, TestData.OwnerId, TestData.IpAddress));

        VerifyNothingSavedOrLogged();
    }

    [Fact]
    public async Task DeleteMediaAssetAsync_WhenAssignedAgentTriesToDelete_ThrowsForbidden()
    {
        //Arrange
        ListingCase listingCase = TestData.AssignAgent(TestData.CreateListingCase());
        MediaAsset mediaAsset = TestData.AddMediaAsset(listingCase, 1);
        _mockMediaAssetRepository.Setup(m => m.GetMediaAssetByIdAsync(1)).ReturnsAsync(mediaAsset);

        //Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _mediaAssetService.DeleteMediaAssetAsync(1, TestData.AgentId, TestData.IpAddress));

        mediaAsset.IsDeleted.Should().BeFalse();
        VerifyNothingSavedOrLogged();
    }
}