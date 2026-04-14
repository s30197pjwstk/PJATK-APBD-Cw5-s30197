using Microsoft.AspNetCore.Mvc;
using TrainingCenterApi.Data;
using TrainingCenterApi.Models;

namespace TrainingCenterApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        // GET /api/reservations 
        [HttpGet]
        public ActionResult<IEnumerable<Reservation>> GetReservations(
            [FromQuery] DateOnly? date,
            [FromQuery] string? status,[FromQuery] int? roomId)
        {
            var query = InMemoryDataStore.Reservations.AsQueryable();

            if (date.HasValue)
                query = query.Where(r => r.Date == date.Value);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));

            if (roomId.HasValue)
                query = query.Where(r => r.RoomId == roomId.Value);

            return Ok(query.ToList());
        }

        // GET /api/reservations/{id}[HttpGet("{id}")]
        public ActionResult<Reservation> GetReservationById(int id)
        {
            var reservation = InMemoryDataStore.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null) return NotFound();

            return Ok(reservation);
        }

        // POST /api/reservations
        [HttpPost]
        public ActionResult<Reservation> CreateReservation([FromBody] Reservation newReservation)
        {
            var room = InMemoryDataStore.Rooms.FirstOrDefault(r => r.Id == newReservation.RoomId);
            
            if (room == null) return BadRequest("Wskazana sala nie istnieje.");
            if (!room.IsActive) return BadRequest("Wskazana sala jest nieaktywna.");

            if (HasTimeConflict(newReservation))
            {
                return Conflict("Rezerwacja koliduje czasowo z inną rezerwacją w tej sali.");
            }

            newReservation.Id = InMemoryDataStore.Reservations.Any() ? InMemoryDataStore.Reservations.Max(r => r.Id) + 1 : 1;
            InMemoryDataStore.Reservations.Add(newReservation);

            return CreatedAtAction(nameof(GetReservationById), new { id = newReservation.Id }, newReservation);
        }

        // PUT /api/reservations/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateReservation(int id, [FromBody] Reservation updatedReservation)
        {
            var existingReservation = InMemoryDataStore.Reservations.FirstOrDefault(r => r.Id == id);
            if (existingReservation == null) return NotFound();

            var room = InMemoryDataStore.Rooms.FirstOrDefault(r => r.Id == updatedReservation.RoomId);
            if (room == null) return BadRequest("Wskazana sala nie istnieje.");
            if (!room.IsActive) return BadRequest("Wskazana sala jest nieaktywna.");

            updatedReservation.Id = id;
            if (HasTimeConflict(updatedReservation))
            {
                return Conflict("Rezerwacja koliduje czasowo z inną rezerwacją w tej sali.");
            }

            existingReservation.RoomId = updatedReservation.RoomId;
            existingReservation.OrganizerName = updatedReservation.OrganizerName;
            existingReservation.Topic = updatedReservation.Topic;
            existingReservation.Date = updatedReservation.Date;
            existingReservation.StartTime = updatedReservation.StartTime;
            existingReservation.EndTime = updatedReservation.EndTime;
            existingReservation.Status = updatedReservation.Status;

            return Ok(existingReservation);
        }

        // DELETE /api/reservations/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteReservation(int id)
        {
            var reservation = InMemoryDataStore.Reservations.FirstOrDefault(r => r.Id == id);
            if (reservation == null) return NotFound();

            InMemoryDataStore.Reservations.Remove(reservation);
            return NoContent();
        }

        private bool HasTimeConflict(Reservation res)
        {
            return InMemoryDataStore.Reservations.Any(r =>
                r.RoomId == res.RoomId &&
                r.Date == res.Date &&
                r.Id != res.Id && 
                !(res.EndTime <= r.StartTime || res.StartTime >= r.EndTime) 
            );
        }
    }
}