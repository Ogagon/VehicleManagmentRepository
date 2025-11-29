using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MonoTask.Service.Models
{
    public class VehicleMake
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(2)]
        public string Name { get; set; }
        [Required]
        [MaxLength(20)]
        [MinLength(2)]
        public string Abrv { get; set; }
        virtual public ICollection<VehicleModel> Models { get; set; }
    }
}
