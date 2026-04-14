using System.ComponentModel.DataAnnotations;

namespace TrainingCenterApi.Models
{
    public class Reservation : IValidatableObject
    {
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }[Required(ErrorMessage = "Imię i nazwisko organizatora jest wymagane.")]
        public string OrganizerName { get; set; }[Required(ErrorMessage = "Temat jest wymagany.")]
        public string Topic { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public string Status { get; set; } = "planned"; // planned, confirmed, cancelled

        // Walidacja biznesowa daty i czasu
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult(
                    "Czas zakończenia musi być późniejszy niż czas rozpoczęcia.",
                    new[] { nameof(EndTime) }
                );
            }
        }
    }
}