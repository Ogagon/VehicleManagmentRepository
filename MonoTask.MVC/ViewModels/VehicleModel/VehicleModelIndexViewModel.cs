using MonoTask.Service.DTO;
using System.Web.Mvc;

namespace MonoTask.MVC.ViewModels.VehicleModel
{
    public class VehicleModelIndexViewModel
    {
        public VehicleModelViewModel DataModel { get; set; }
        public PagingResult<VehicleModelViewModel> PagingResult { get; set; }
        public VehicleQuery Query { get; set; }
        public SelectList SelectMakes { get; set; }
    }
}