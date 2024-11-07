using Domain.Entities;
using Domain.Room.ValueObjects;
using Domain.Room.Enums;

namespace Application.Dtos
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public bool InMaintenance { get; set; }
        public decimal PriceValue { get; set; }
        public int CurrencyCode { get; set; }

        public static Domain.Entities.Room MapToEntity(RoomDto roomDto)
        {
            return new Domain.Entities.Room
            {
                Id = roomDto.Id,
                Name = roomDto.Name,
                Level = roomDto.Level,
                InMaintenance = roomDto.InMaintenance,
                Price = new Price
                {
                    Value = roomDto.PriceValue,
                    Currency = (AcceptedCurrencies)roomDto.CurrencyCode
                }
            };
        }

        public static RoomDto MapToDto(Domain.Entities.Room room)
        {
            return new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Level = room.Level,
                InMaintenance = room.InMaintenance,
                PriceValue = room.Price.Value,
                CurrencyCode = (int)room.Price.Currency
            };
        }
    }
}
