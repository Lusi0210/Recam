using System;
using Remp.Models.Documents;

namespace Remp.Repository.Interfaces;

public interface IUserActivityLogRepository
{
    Task AddAsync(UserActivityLog log);
}
