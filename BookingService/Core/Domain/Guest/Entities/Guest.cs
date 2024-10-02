using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Guest
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Surname {get; set; }
        public string Email { get; set; }
        public PersonId DocumentId { get; set; }

    }
}
