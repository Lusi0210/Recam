using System;
using Remp.Common.Exceptions;
using Remp.Models.Entities;

namespace Remp.Service.Common;

public static class ListingCaseAccessExtensions
{
    public static bool IsOwnedBy(this ListingCase listingCase, string userId)
    {
        return listingCase.UserId == userId;
    }

    public static bool IsAssignedTo(this ListingCase listingCase, string userId)
    {
        return listingCase.Agents.Any(a => a.Id == userId);
    }

    public static void EnsureOwnerOrAssignedAgent(this ListingCase listingCase, string userId, string message)
    {
        if (!listingCase.IsOwnedBy(userId) && !listingCase.IsAssignedTo(userId))
        {
            throw new ForbiddenException(message);
        }
    }

    public static void EnsureOwner(this ListingCase listingCase, string userId, string message)
    {
        if (!listingCase.IsOwnedBy(userId))
        {
            throw new ForbiddenException(message);
        }
    }

    public static void EnsureAssignedAgent(this ListingCase listingCase, string userId, string message)
    {
        if (!listingCase.IsAssignedTo(userId))
        {
            throw new ForbiddenException(message);
        }
    }
}
