using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Threading.Tasks;

namespace PC_club.Services
{
    public class SessionEstimatesStore
    {
        private readonly string _filePath = "estimated_times.json";
        private Dictionary<int, DateTime> _estimates = new();

        // Завантажуємо при старті програми
        public void LoadEstimates()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                _estimates = JsonSerializer.Deserialize<Dictionary<int, DateTime>>(json) ?? new();
            }
        }

        // Зберігаємо після кожної нової сесії
        public void SaveEstimates()
        {
            var json = JsonSerializer.Serialize(_estimates);
            File.WriteAllText(_filePath, json);
        }

        // Додати або оновити час
        public void SetEstimate(int sessionId, DateTime estimatedEnd)
        {
            _estimates[sessionId] = estimatedEnd;
            SaveEstimates(); // Одразу пишемо у файл
        }

        // Отримати час для малювання
        public DateTime? GetEstimate(int sessionId)
        {
            return _estimates.TryGetValue(sessionId, out var time) ? time : null;
        }

        // Видалити, коли сесія закрилась
        public void RemoveEstimate(int sessionId)
        {
            if (_estimates.Remove(sessionId))
            {
                SaveEstimates();
            }
        }
    }
}
