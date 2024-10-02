using Domain.Enums;

namespace Domain.ValueObjects
{
    public class PersonId
    {
        public int IdNumber { get; set; }
        public DocumentType DocumentType { get; set; }
    }
}
