using AutoMapper;
using MonoTask.MVC.ViewModels;
using MonoTask.MVC.ViewModels.VehicleMake;
using MonoTask.Service.DTO;
using MonoTask.Service.Interfaces;
using MonoTask.Service.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MonoTask.MVC.Controllers
{
    public class VehicleMakeController : Controller
    {
        private readonly IVehicleService _service;
        private readonly IMapper _mapper;
        public VehicleMakeController(IVehicleService service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        // GET: VehicleModel
        public async Task<ActionResult> Index(string sortColumn, string searchTerm, int? makeId, bool sortDescending = false, int page = 1, int pageSize = 10)
        {
            var query = new VehicleQuery(sortColumn, sortDescending, searchTerm, makeId);
            var pagination = new PaginationRequest(page, pageSize);

            var PagedMakes = await _service.GetVehicleMakesByParameters(query, pagination);
            List<VehicleMake> allMakes = await _service.GetAllVehicleMakes();

            var selectMakes = new SelectList(allMakes, "Id", "Name", makeId);
            VehicleMakeIndexViewModel indexVM= new VehicleMakeIndexViewModel
            {
                PagingResult = _mapper.Map<PagingResult<VehicleMakeViewModel>>(PagedMakes),
                SelectMakes = selectMakes,
                Query = query
            };
            return View(indexVM);
        }
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(VehicleMakeViewModel vm, string submitButton)
        {
            ActionResult result = ValidateAndMap<VehicleMakeViewModel, VehicleMake>(vm, out VehicleMake model);
            if (result != null) return result;

            bool status = await _service.CreateVehicleMake(model);
            if (!status)
            {
                ModelState.AddModelError("", "The input vehicle make already exists!");
                return View(vm);
            }
            SetTempMessage($"Vehicle make {model.Name} ({model.Abrv}) created successfully!");
            if (submitButton == "Save and Add another")
            {
                return RedirectToAction("Create");
            }
            return RedirectToAction("Index");

        }
        [HttpGet]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleMake model = await _service.GetVehicleMakeById(id.Value);
            if (model == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            VehicleMakeViewModel vm = _mapper.Map<VehicleMakeViewModel>(model);
            if (vm == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(vm);
        }
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleMake vehicleMake = await _service.GetVehicleMakeById(id.Value);
            if (vehicleMake == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            VehicleMakeViewModel vm = _mapper.Map<VehicleMakeViewModel>(vehicleMake);
            if (vm == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(VehicleMakeViewModel vm)
        {
            ActionResult result = ValidateAndMap<VehicleMakeViewModel, VehicleMake>(vm, out VehicleMake model);
            if (result != null) return result;

            bool status = await _service.EditVehicleMake(model);

            if (status)
            {
                SetTempMessage("Vehicle make update success!");
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "The input vehicle make already exists!");
                return View(vm);
            }
        }
        [HttpGet]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "You need to specify an id for the item you want to delete.");

            }
            VehicleMake model = await _service.GetVehicleMakeById(id.Value);
            if (model == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound, $"Item with Id = {id} does not exist!");

            VehicleMakeViewModel vm = _mapper.Map<VehicleMakeViewModel>(model);

            return View(vm);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            bool success = await _service.DeleteVehicleMake(id);
            if (!success) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            SetTempMessage($"Vehicle make was removed successfully!)");
            return RedirectToAction("Index");
        }

        // Nešto što se ponavljalo često u kodu, pa sam našao način da s genericima to skratim
        protected ActionResult ValidateAndMap<TViewModel, TEntity>(TViewModel vm, out TEntity entity)
        {
            entity = default;
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            entity = _mapper.Map<TEntity>(vm);
            return null;
        }
        //Helper for status messages
        private void SetTempMessage(string message, string type = "success")
        {
            TempData["Message"] = message;
            TempData["MessageType"] = type;
        }
    }
}