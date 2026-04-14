using Microsoft.AspNetCore.Mvc;
using TrainingCenterApi.Data;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        // GET /api/rooms oraz /api/rooms?minCapacity=20&hasProjector=true&activeOnly=true
        [HttpGet]
        public ActionResult<IEnumerable<Room>> GetRooms(
            [FromQuery] int? minCapacity, 
            [FromQuery] bool? hasProjector, 
            [FromQuery] bool? activeOnly)
        {
            var query = InMemoryDataStore.Rooms.AsQueryable();

            if (minCapacity.HasValue)
                query = query.Where(r => r.Capacity >= minCapacity.Value);

            if (hasProjector.HasValue)
                query = query.Where(r => r.HasProjector == hasProjector.Value);

            if (activeOnly.HasValue && activeOnly.Value)
                query = query.Where(r => r.IsActive);

            return Ok(query.ToList());
        }

        // GET /api/rooms/{id}[HttpGet("{id}")]
        public ActionResult<Room> GetRoomById(int id)
        {
            var room = InMemoryDataStore.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null) return NotFound($"Sala o ID {id} nie została znaleziona.");
            
            return Ok(room);
        }

        // GET /api/rooms/building/{buildingCode}
        [HttpGet("building/{buildingCode}")]
        public ActionResult<IEnumerable<Room>> GetRoomsByBuilding(string buildingCode)
        {
            var rooms = InMemoryDataStore.Rooms
                .Where(r => r.BuildingCode.Equals(buildingCode, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(rooms);
        }

        // POST /api/rooms
        [HttpPost]
        public ActionResult<Room> CreateRoom([FromBody] Room newRoom)
        {
            newRoom.Id = InMemoryDataStore.Rooms.Any() ? InMemoryDataStore.Rooms.Max(r => r.Id) + 1 : 1;
            InMemoryDataStore.Rooms.Add(newRoom);

            return CreatedAtAction(nameof(GetRoomById), new { id = newRoom.Id }, newRoom);
        }

        // PUT /api/rooms/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateRoom(int id, [FromBody] Room updatedRoom)
        {
            var existingRoom = InMemoryDataStore.Rooms.FirstOrDefault(r => r.Id == id);
            if (existingRoom == null) return NotFound();

            existingRoom.Name = updatedRoom.Name;
            existingRoom.BuildingCode = updatedRoom.BuildingCode;
            existingRoom.Floor = updatedRoom.Floor;
            existingRoom.Capacity = updatedRoom.Capacity;
            existingRoom.HasProjector = updatedRoom.HasProjector;
            existingRoom.IsActive = updatedRoom.IsActive;

            return Ok(existingRoom);
        }

        // DELETE /api/rooms/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteRoom(int id)
        {
            var room = InMemoryDataStore.Rooms.FirstOrDefault(r => r.Id == id);
            if (room == null) return NotFound();

            // Reguła biznesowa: nie można usunąć sali, jeśli ma przypisane rezerwacje
            var hasReservations = InMemoryDataStore.Reservations.Any(res => res.RoomId == id);
            if (hasReservations)
            {
                return Conflict("Nie można usunąć sali, ponieważ istnieją do niej przypisane rezerwacje.");
            }

            InMemoryDataStore.Rooms.Remove(room);
            return NoContent();
        }
    }
}