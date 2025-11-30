using HotelManagement.Booking.DTOs;
using HotelManagement.Booking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Booking.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GuestsController : ControllerBase
    {
        private readonly IGuestService _guestService;
        private readonly ILogger<GuestsController> _logger;

        public GuestsController(IGuestService guestService, ILogger<GuestsController> logger)
        {
            _guestService = guestService;
            _logger = logger;
        }

        /// <summary>
        /// Get all guests
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GuestDTO>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<GuestDTO>>>> GetGuests()
        {
            var response = await _guestService.GetAllGuestsAsync();
            return Ok(response);
        }

        /// <summary>
        /// Get a specific guest by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GuestDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<GuestDTO>>> GetGuest(int id)
        {
            var response = await _guestService.GetGuestByIdAsync(id);
            return response.Success ? Ok(response) : NotFound(response);
        }

        /// <summary>
        /// Get a guest by email address
        /// </summary>
        [HttpGet("by-email/{email}")]
        [ProducesResponseType(typeof(ApiResponse<GuestDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<GuestDTO>>> GetGuestByEmail(string email)
        {
            var response = await _guestService.GetGuestByEmailAsync(email);
            return response.Success ? Ok(response) : NotFound(response);
        }

        /// <summary>
        /// Create a new guest
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<GuestDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<GuestDTO>>> CreateGuest([FromBody] CreateGuestDTO createGuestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _guestService.CreateGuestAsync(createGuestDTO);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return CreatedAtAction(nameof(GetGuest), new { id = response.Data!.Id }, response);
        }

        /// <summary>
        /// Update an existing guest
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GuestDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<GuestDTO>>> UpdateGuest(int id, [FromBody] UpdateGuestDTO updateGuestDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _guestService.UpdateGuestAsync(id, updateGuestDTO);
            
            if (!response.Success)
            {
                return response.Message.Contains("not found") ? NotFound(response) : BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Delete a guest (only if they have no bookings)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteGuest(int id)
        {
            var response = await _guestService.DeleteGuestAsync(id);
            
            if (!response.Success)
            {
                return response.Message.Contains("not found") ? NotFound(response) : BadRequest(response);
            }

            return Ok(response);
        }
    }
}
