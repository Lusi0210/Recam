using System;
using Remp.Models.Entities;

namespace Remp.Repository.Interfaces;

public interface IMediaAssetRepository
{
    void Add(MediaAsset mediaAsset);
}
