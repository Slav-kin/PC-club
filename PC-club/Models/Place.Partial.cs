using System.Linq;

namespace PC_club.Models;

public partial class Place
{
    // Шукаємо активну сесію та повертаємо нікнейм клієнта
    public string CurrentClientNickname =>
        Sessions?.FirstOrDefault(s => s.Status == "active")?.Client?.Nickname ?? "Вільно";

    // Якщо є активна сесія - рамка червона, якщо немає - зелена
    public string StatusColor =>
        Sessions?.Any(s => s.Status == "active") == true ? "#ff5252" : "#4caf50";
}