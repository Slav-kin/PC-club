using System.Linq;

namespace PC_club.Models;

public partial class Place
{
    // Шукаємо активну сесію та повертаємо нікнейм клієнта
    public string CurrentClientNickname =>
        Sessions?.FirstOrDefault(s => s.Status == "active")?.Client?.Nickname ?? "Вільно";

    public string StatusColor =>
     Sessions?.Any(s => s.Status == "active") == true ? "#ff5252" :
     Bookings.Any(b => b.Status == "active") == true ? "#f0b100" : "#4caf50";

    public string PcType => PlaceNumber switch
    {
        <= 5 => "basic",
        <= 10 => "consol",
        <= 15 => "pro",
        _ => "vip"
    };
    // Витягуємо назву тарифу з активної сесії
    public string CurrentTariffName => Sessions?.FirstOrDefault(s => s.Status == "active")?.Tariff?.TariffName
                                       ?? PcType; // Виведе тип ПК замість слова "Вільно"

}