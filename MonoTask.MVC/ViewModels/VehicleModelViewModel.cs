using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace MonoTask.MVC.ViewModels
{
    public class VehicleModelViewModel : PagedResult<VehicleModelViewModel>
    {
        public int Id { get; set; }
        public int MakeId { get; set; }
        [Required]
        [MaxLength(100)]
        [DisplayName("Model Name")]
        public string Name { get; set; }
        [Required]
        [MaxLength(20)]
        [DisplayName("Abbreviation")]
        public string Abrv { get; set; }
        [ForeignKey("MakeId")]
        public VehicleMakeViewModel Make { get; set; }
        public IEnumerable<SelectListItem> Makes { get; set; }
        public string Display => $"{Name}({Abrv})";
    }
}