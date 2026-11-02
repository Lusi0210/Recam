using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Remp.Repository.Common;
using Remp.Service.Interfaces;
using Remp.Service.Mappers;

namespace Remp.Tests.TestHelpers;

public static class MockHelper
{
    public static IMapper CreateMapper()
    {
        MapperConfiguration config = new MapperConfiguration(
            cfg => cfg.AddProfile<MappingProfile>(),
            NullLoggerFactory.Instance);
        return config.CreateMapper();
    }

    public static Mock<IUnitOfWork> CreateUnitOfWork()
    {
        Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
        mock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        return mock;
    }

    public static Mock<IBlobStorageService> CreateBlobStorage()
    {
        Mock<IBlobStorageService> mock = new Mock<IBlobStorageService>();
        mock.Setup(b => b.GetReadUrl(It.IsAny<string>()))
            .Returns((string blobName) => $"https://fake-storage/{blobName}?sas=test");
        return mock;
    }

    public static Mock<IActivityLogService> CreateActivityLog()
    {
        return new Mock<IActivityLogService>();
    }
}