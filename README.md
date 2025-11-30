using AutoMapper;
using Azure;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRoomService _roomService;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(IRoomService roomService, IMapper mapper, ILogger<RoomsController> logger)
        {
            _roomService = roomService;
            _mapper = mapper;
            _logger = logger;
        }
        
        [HttpGet]
        public async Task<ActionResult<PaginatedResponseDTO<RoomDTO>>> GetRooms([FromQuery] RoomFilterDTO filter)
        {

            //var rooms = await _roomService.GetAllRoomsAsync();
            //return Ok(rooms);
            try
            {
                var pagedRooms = await _roomService.GetFilteredRoomsAsync(filter);
                var roomDtos = _mapper.Map<List<RoomDTO>>(pagedRooms.Data);

                var pagedList = new PagedList<RoomDTO>(
                    roomDtos,
                    pagedRooms.TotalCount,
                    pagedRooms.PageNumber,
                    pagedRooms.PageSize
                    );

                return Ok(PaginatedResponseDTO<RoomDTO>.SuccessResult(pagedList.Data, pagedList.PageNumber, pagedList.PageSize, pagedList.TotalCount ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, PaginatedResponseDTO<List<RoomDTO>>.FailureResult(
                    "An error occurred while retrieving rooms",
                    new List<string> { ex.Message }
                    ));
            }
        }
        [HttpPost]
        public async Task<ActionResult<NonPaginatedResponseDTO<RoomDTO>>> CreateRoom([FromBody] CreateRoomDTO createRoomDTO)
        {
            // _logger.LogInformation("CreateRoom createRoomDTO: {@Response}", createRoomDTO);
            try
            {

            var response = await _roomService.CreateRoomAsync(createRoomDTO);

            // _logger.LogInformation("CreateRoom response: {@Response}", response);

            return response;
            }
            catch (Exception ex)
            {
                return StatusCode(500, NonPaginatedResponseDTO<List<RoomDTO>>.FailureResult(
                    "An error occurred while creating room",
                    new List<string> { ex.Message }
                    ));
            }

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<NonPaginatedResponseDTO<RoomDTO>>> GetRoom(int id)
        {
            try
            {

            var response = await _roomService.GetRoomByIdAsync(id);
         
            return response;

            }
             catch (Exception ex)
            {
                return StatusCode(500, NonPaginatedResponseDTO<List<RoomDTO>>.FailureResult(
                    "An error occurred while creating room",
                    new List<string> { ex.Message }
                    ));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<NonPaginatedResponseDTO<RoomDTO>>> UpdateRoom(int id, [FromBody] CreateRoomDTO updateRoomDTO)
        {
            try
            {

            var response = await _roomService.UpdateRoomAsync(id, updateRoomDTO);
                //if (room == null) return NotFound();
                ////return Ok(room);
                //return Ok(room);
                return response;


            }
            catch (Exception ex)
            {
                return StatusCode(500, NonPaginatedResponseDTO<List<RoomDTO>>.FailureResult(
                    "An error occurred while updating room",
                    new List<string> { ex.Message }
                    ));
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<NonPaginatedResponseDTO<RoomDTO>>> DeleteRoom(int id)
        {
            try
            {

            var response = await _roomService.DeleteRoomAsync(id);
                //return NoContent();
                return response;


            }
            catch (Exception ex)
            {
                return StatusCode(500, NonPaginatedResponseDTO<List<RoomDTO>>.FailureResult(
                    "An error occurred while deleting room",
                    new List<string> { ex.Message }
                    ));
            }
        }
        [HttpGet("available")]
        public async Task<IActionResult> GetAvailableRooms([FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut)
        {
            var rooms = await _roomService.GetAvailableRoomsAsync(checkIn, checkOut);
            return Ok(rooms);
        }
    }
}






using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Models;
using Microsoft.AspNetCore.Authorization;

namespace HotelManagement.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository _roomRepository;
        private readonly ILogger<RoomService> _logger;

        public RoomService(IRoomRepository roomRepository, ILogger<RoomService> logger)
        {
            _roomRepository = roomRepository;
            _logger = logger;
        }
        public async Task<IEnumerable<RoomDTO>> GetAllRoomsAsync()
        {
            var rooms = await _roomRepository.GetAllRoomsAsync();
            return rooms.Select(r => MapToDTO(r));
        }

        public async Task<PagedList<RoomDTO>> GetFilteredRoomsAsync(RoomFilterDTO filter)
        {
            var pagedRooms = await _roomRepository.GetFilteredRoomsAsync(filter);

            var dtoData = pagedRooms.Data.Select(r => MapToDTO(r)).ToList();

            // Return a new PagedList<RoomDTO>
            return new PagedList<RoomDTO>(
                dtoData,
                pagedRooms.TotalCount,
                pagedRooms.PageNumber,
                pagedRooms.PageSize
            );
        }
        //Task<IEnumerable<RoomDTO>> GetFilteredRoomsAsync();

        public async Task<NonPaginatedResponseDTO<RoomDTO>> GetRoomByIdAsync(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);

            return room == null
                ? NonPaginatedResponseDTO<RoomDTO>.FailureResult(
                    "Room Not Found",
                    new List<string>() // empty error list
                  )
                : NonPaginatedResponseDTO<RoomDTO>.SuccessResult(
                    MapToDTO(room),
                    "Room Fetched Successfully"
                  );
        }

        public async Task<NonPaginatedResponseDTO<RoomDTO>> CreateRoomAsync(CreateRoomDTO createRoomDTO)
        {
            var room = new Room
            {
                RoomNumber = createRoomDTO.RoomNumber,
                //Type = createRoomDTO.Type,
                RoomType = Enum.Parse<RoomType>(createRoomDTO.RoomType),
                PricePerNight = createRoomDTO.PricePerNight,
                Capacity = createRoomDTO.Capacity,
                IsAvailable = createRoomDTO.IsAvailable,
                Description = createRoomDTO.Description
            };
           var createdRoom =  await _roomRepository.CreateRoomAsync(room);

            // _logger.LogInformation("CreateRoomService createdRoom: {@Response}", createdRoom);

            //return MapToDTO(createdRoom);
            return NonPaginatedResponseDTO<RoomDTO>.SuccessResult(MapToDTO(createdRoom), "Room Created Successfully");
        }
        public async Task<NonPaginatedResponseDTO<RoomDTO>> UpdateRoomAsync(int id, CreateRoomDTO updateRoomDTO)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            //if (room == null) return null;
            if (room == null)
            {

                return NonPaginatedResponseDTO<RoomDTO>.FailureResult(
                        "Room Not Found",
                        new List<string>() // empty error list
                      );
            }
            room.RoomNumber = updateRoomDTO.RoomNumber;
            //room.Type = createRoomDTO.Type,
            room.RoomType = Enum.Parse<RoomType>(updateRoomDTO.RoomType);
            room.PricePerNight = updateRoomDTO.PricePerNight;
            room.Capacity = updateRoomDTO.Capacity;
            room.IsAvailable = updateRoomDTO.IsAvailable;
            room.Description = updateRoomDTO.Description;
            var updatedRoom = await _roomRepository.UpdateRoomAsync(room);
            //return MapToDTO(updatedRoom);

            return NonPaginatedResponseDTO<RoomDTO>.SuccessResult(
                    MapToDTO(updatedRoom),
                    "Room Updated Successfully"
                  );
        }
        public async Task<NonPaginatedResponseDTO<RoomDTO>> DeleteRoomAsync(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            //if (room == null) return null;
            if (room == null)
            {

                return NonPaginatedResponseDTO<RoomDTO>.FailureResult(
                        "Room Not Found",
                        new List<string>() // empty error list
                      );
            }
           var deletedRoom = await _roomRepository.DeleteRoomAsync(room);

            return NonPaginatedResponseDTO<RoomDTO>.SuccessResult(
                MapToDTO(deletedRoom),
                "Room Deleted Successfully"
              );
        }
        public async Task<IEnumerable<RoomDTO>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            var rooms = await _roomRepository.GetAvailableRoomsAsync(checkIn, checkOut);
            return rooms.Select(r => MapToDTO(r));
        }

        private RoomDTO MapToDTO(Room room)
        {
            return new RoomDTO
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType.ToString(),
                PricePerNight = room.PricePerNight,
                Capacity = room.Capacity,
                IsAvailable = room.IsAvailable,
                Description = room.Description
            };
        }
    }
}







using Microsoft.EntityFrameworkCore;
using HotelManagement.Data;
using HotelManagement.DTOs;
using HotelManagement.Interfaces;
using HotelManagement.Models;

namespace HotelManagement.Repositories
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;
        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms.ToListAsync();
        }
        public async Task<PagedList<Room>> GetFilteredRoomsAsync(RoomFilterDTO filter)
        {
            var query = _context.Rooms.AsQueryable();

            query = ApplyFilters(query, filter);

            query = ApplySorting(query, filter.SortBy, filter.SortOrder);

            return await PagedList<Room>.CreateAsync(query, filter.PageNumber, filter.PageSize);
        }
        public async Task<Room> GetRoomByIdAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }
        public async Task<Room> CreateRoomAsync(Room room)
        {
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }
        public async Task<Room> UpdateRoomAsync(Room room)
        {
            //_context.Rooms.Update(room);
            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return room;
        }
        public async Task<Room> DeleteRoomAsync(Room room)
        {
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return room;
        }
        public async Task<IEnumerable<Room>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
        {
            var bookedRoomIds = await _context.Bookings
                .Where(b => b.CheckInDate < checkOut && b.CheckOutDate > checkIn)
                .Select(b => b.RoomId)
                .ToListAsync();
            return await _context.Rooms
                .Where(r => !bookedRoomIds.Contains(r.Id) && r.IsAvailable)
                .ToListAsync();
        }
        private IQueryable<Room> ApplyFilters(IQueryable<Room> query, RoomFilterDTO filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(r => r.RoomNumber.ToLower().Contains(searchTerm) ||
                r.RoomType.ToString().ToLower().Contains(searchTerm) ||
                r.Description.ToLower().Contains(searchTerm)
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.RoomType) &&
                Enum.TryParse<RoomType>(filter.RoomType, true, out var parsedType))
            {
                query = query.Where(r => r.RoomType == parsedType);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(r => r.PricePerNight >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(r => r.PricePerNight >= filter.MaxPrice.Value);
            }

            if (filter.IsAvailable.HasValue)
            {
                query = query.Where(r => r.IsAvailable == filter.IsAvailable.Value);
            }

            //if (filter.Floor.HasValue)
            //{
            //    some logic for room floor filtering
            //    query = query.Where(r => r.Floor >= filter.Floor.Value);
            //}

            //if (filter.AvailableFrom.HasValue)
            //{
            // some logic from AvailableFrom
            //    query = query.Where(r => r.PricePerNight >= filter.AvailableFrom.Value);
            //}

            //if (filter.AvailableTo.HasValue)
            //{
            // some logic from AvailableTo
            //    query = query.Where(r => r.PricePerNight >= filter.AvailableTo.Value);
            //}

            return query;
        }

        private IQueryable<Room> ApplySorting(IQueryable<Room> query, string sortBy, string sortOrder)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderBy(r => r.RoomNumber);
            }

            var isDescending = sortOrder?.ToLower() == "desc";

            query = sortBy.ToLower() switch
            {
                "price" => isDescending
                    ? query.OrderByDescending(r => r.PricePerNight)
                    : query.OrderBy(r => r.PricePerNight)
            };

            return query;
        }
    }
}





