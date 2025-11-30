using HotelManagement.DTOs;
using HotelManagement.Models;

namespace HotelManagement.Interfaces
{
    public interface IRoomService
    {
        Task<PaginatedResponseDTO<RoomDTO>> GetFilteredRoomsAsync(RoomFilterDTO filter);
        Task<NonPaginatedResponseDTO<RoomDTO>> GetRoomByIdAsync(int id);
        Task<NonPaginatedResponseDTO<RoomDTO>> CreateRoomAsync(CreateRoomDTO createRoomDTO);
        Task<NonPaginatedResponseDTO<RoomDTO>> UpdateRoomAsync(int id, CreateRoomDTO updateRoomDTO);
        Task<NonPaginatedResponseDTO<RoomDTO>> DeleteRoomAsync(int id);
        Task<NonPaginatedResponseDTO<IEnumerable<RoomDTO>>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
    }

    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<PagedList<Room>> GetFilteredRoomsAsync(RoomFilterDTO filter);
        Task<Room?> GetRoomByIdAsync(int id);
        Task<Room?> GetRoomByRoomNumberAsync(string roomNumber);
        Task<Room> CreateRoomAsync(Room room);
        Task<Room> UpdateRoomAsync(Room room);
        Task<Room> DeleteRoomAsync(Room room);
        Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut);
        Task<bool> HasActiveBookingsAsync(int roomId);
    }
}



// Program.cs - Dependency Injection Registration

using HotelManagement.Data;
using HotelManagement.Interfaces;
using HotelManagement.Infrastructure;
using HotelManagement.Repositories;
using HotelManagement.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<IRoomRepository, RoomRepository>();

// Services
builder.Services.AddScoped<IRoomService, RoomService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

// Logging
builder.Services.AddLogging();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
