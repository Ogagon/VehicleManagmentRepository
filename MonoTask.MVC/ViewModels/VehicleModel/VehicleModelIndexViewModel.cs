using MonoTask.Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonoTask.MVC.ViewModels.VehicleModel
{
    public class VehicleModelIndexViewModel
    {
        public PagingResult<VehicleMakeViewModel> PagingResult { get; set; }
        public VehicleQuery Query { get; set; }
        public SelectList SelectMakes { get; set; }
    }
}