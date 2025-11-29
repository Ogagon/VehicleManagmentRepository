using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MonoTask.MVC.ViewModels
{
    public class VehicleMakeViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(2)]
        public string Name { get; set; }
        [Required]
        [MaxLength(20)]
        [MinLength(2)]
        [Display(Name = "Abbreviation")]
        public string Abrv { get; set; }
        public ICollection<VehicleModelViewModel> Models { get; set; }
    }
}