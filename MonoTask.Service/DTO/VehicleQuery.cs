using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTask.Service.DTO
{
    public class VehicleQuery
    {
        public string CurrentSortColumn { get; set; } = "Name";
        public bool CurrentSortDescending { get; set; } = false;
        public string CurrentSearchTerm { get; set; }
        public int? CurrentMakeId { get; set; }
        public static readonly Dictionary<string, string> allowedSortingColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", "Name" },
                { "Abrv", "Abrv" },
                { "Make", "Make.Name" }
            };
        public VehicleQuery(string currentSortColumn, bool currentSortDescending, string currentSearchTerm, int? currentMakeId)
        {
            if (string.IsNullOrEmpty(currentSortColumn) || !allowedSortingColumns.TryGetValue(currentSortColumn, out var sortColumn))
            {
                sortColumn = "Name";
            }
            CurrentSortColumn = sortColumn;
            CurrentSortDescending = currentSortDescending;
            CurrentSearchTerm = currentSearchTerm;
            CurrentMakeId = currentMakeId;
        }
    }
}
