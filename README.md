using HotelManagement.Auth.DTOs;
using HotelManagement.Auth.Interfaces;
using HotelManagement.Auth.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotelManagement.Auth.Services
{
    // Add this method to your existing AuthService class

    public async Task<PaginatedResponse<UserDTO>> GetFilteredUsersAsync(UserFilterDTO filter)
    {
        try
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.Email.ToLower().Contains(searchTerm) ||
                    u.FirstName.ToLower().Contains(searchTerm) ||
                    u.LastName.ToLower().Contains(searchTerm));
            }

            // Apply role filter
            if (!string.IsNullOrWhiteSpace(filter.Role))
            {
                query = query.Where(u => u.Role.ToLower() == filter.Role.ToLower());
            }

            // Apply active status filter
            if (filter.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == filter.IsActive.Value);
            }

            // Apply created date filters
            if (filter.CreatedAfter.HasValue)
            {
                query = query.Where(u => u.CreatedAt >= filter.CreatedAfter.Value);
            }

            if (filter.CreatedBefore.HasValue)
            {
                query = query.Where(u => u.CreatedAt <= filter.CreatedBefore.Value);
            }

            // Apply last login filters
            if (filter.LastLoginAfter.HasValue)
            {
                query = query.Where(u => u.LastLoginAt.HasValue && u.LastLoginAt >= filter.LastLoginAfter.Value);
            }

            if (filter.LastLoginBefore.HasValue)
            {
                query = query.Where(u => u.LastLoginAt.HasValue && u.LastLoginAt <= filter.LastLoginBefore.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, filter.SortBy, filter.SortOrder);

            // Apply pagination
            query = query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);

            // Execute query and map to DTOs
            var users = await query
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt,
                    LastLoginAt = u.LastLoginAt
                })
                .ToListAsync();

            return PaginatedResponse<UserDTO>.SuccessResponse(
                users,
                totalCount,
                filter.PageNumber,
                filter.PageSize,
                $"Retrieved {users.Count} of {totalCount} users"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving filtered users");
            return PaginatedResponse<UserDTO>.FailureResponse(
                "An error occurred while retrieving users",
                new List<string> { ex.Message }
            );
        }
    }

    private IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, string sortOrder)
    {
        var isDescending = sortOrder.ToLower() == "desc";

        return sortBy?.ToLower() switch
        {
            "email" => isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "firstname" => isDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
            "lastname" => isDescending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
            "role" => isDescending ? query.OrderByDescending(u => u.Role) : query.OrderBy(u => u.Role),
            "createdat" => isDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
            "lastloginat" => isDescending ? query.OrderByDescending(u => u.LastLoginAt) : query.OrderBy(u => u.LastLoginAt),
            _ => query.OrderByDescending(u => u.CreatedAt) // Default sort by CreatedAt descending
        };
    }
}
