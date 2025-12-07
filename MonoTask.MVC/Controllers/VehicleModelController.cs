using AutoMapper;
using MonoTask.MVC.ViewModels;
using MonoTask.MVC.ViewModels.VehicleModel;
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
    public class VehicleModelController : Controller
    {
        private IVehicleService _service;
        private IMapper _mapper;
        private ILogger _logger;
        public VehicleModelController(IVehicleService service, IMapper mapper, ILogger logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: VehicleModels
        public async Task<ActionResult> Index(string sortColumn, string searchTerm, int? makeId, bool sortDescending = false, int page = 1, int pageSize = 10)
        {
            var query = new VehicleQuery(sortColumn, sortDescending, searchTerm, makeId);
            var pagination = new PaginationRequest(page, pageSize);

            var pagedResult = await _service.GetAllVehicleModels(query, pagination);
            List<VehicleMake> allMakes = await _service.GetAllVehicleMakes();

            SelectList makesList = new SelectList(allMakes, "Id", "Name", makeId);

            var indexVM = new VehicleModelIndexViewModel
            {
                PagingResult = _mapper.Map<PagingResult<VehicleModelViewModel>>(pagedResult),
                Query = query,
                SelectMakes = makesList
            };
            return View(indexVM);
        }

        // GET: VehicleModels/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleModel vehicleModel = await _service.GetVehicleModelById(id.Value);
            if (vehicleModel == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            VehicleModelViewModel vm = _mapper.Map<VehicleModelViewModel>(vehicleModel);
            return View(vm);
        }

        // GET: VehicleModels/Create
        public async Task<ActionResult> Create(int? makeId)
        {
            List<VehicleMake> makes = await _service.GetAllVehicleMakes();

            var vm = new VehicleModelViewModel
            {
                MakeId = makeId ?? 0,
                Makes = _mapper.Map<IEnumerable<SelectListItem>>(makes)
            };

            return View(vm);
        }

        // POST: VehicleModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(VehicleModelViewModel vm, string submitButton)
        {
            if (!ModelState.IsValid)
            {
                await PopulateMakesAsync(vm);
                return View(vm);
            }
            VehicleModel newVehicleModel = _mapper.Map<VehicleModel>(vm);
            bool attemptCreate = await _service.CreateVehicleModel(newVehicleModel);
            if (!attemptCreate)
            {
                await PopulateMakesAsync(vm);
                SetTempMessage($"Failed to create Vehicle model {newVehicleModel.Name}({newVehicleModel.Abrv}). Most likely, the specified model already exists!", "danger");
                return View(vm);
            }
            SetTempMessage($"Vehicle model {newVehicleModel.Name}({newVehicleModel.Abrv}) created successfully!");
            if (submitButton == "Save and Add another")
            {
                return RedirectToAction("Create");
            }
            return RedirectToAction("Index");
        }

        // GET: VehicleModels/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleModel vehicleModel = await _service.GetVehicleModelById(id.Value);
            if (vehicleModel == null)
            {
                return HttpNotFound();
            }
            VehicleModelViewModel vm = _mapper.Map<VehicleModelViewModel>(vehicleModel);

            await PopulateMakesAsync(vm);
            return View(vm);
        }

        // POST: VehicleModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(VehicleModelViewModel vm)
        {
            ActionResult result = ValidateAndMap<VehicleModelViewModel, VehicleModel>(vm, out var model);
            if (result != null) return result;
            bool status = await _service.EditVehicleModel(model);

            if (status)
            {
                SetTempMessage("Vehicle make update success!");
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "The input vehicle make already exists!");
                await PopulateMakesAsync(vm);
                return View(vm);
            }
        }

        // GET: VehicleModels/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleModel vehicleModel = await _service.GetVehicleModelById(id.Value);
            if (vehicleModel == null)
            {
                return HttpNotFound();
            }
            VehicleModelViewModel vehicleModelViewModel = _mapper.Map<VehicleModelViewModel>(vehicleModel);
            return View(vehicleModelViewModel);
        }

        // POST: VehicleModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var success = await _service.DeleteVehicleModel(id);
            if (!success) return new HttpStatusCodeResult(HttpStatusCode.NotFound, "The operation to remove the specified vehicle model failed!");

            SetTempMessage("Vehicle model was removed successfully!");
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
        //Helper da se ne ponavljam previše u kodu
        private void SetTempMessage(string message, string type = "success")
        {
            TempData["Message"] = message;
            TempData["MessageType"] = type;
        }
        private async Task<VehicleModelViewModel> PopulateMakesAsync(VehicleModelViewModel vm)
        {
            var makes = await _service.GetAllVehicleMakes();
            vm.Makes = _mapper.Map<IEnumerable<SelectListItem>>(makes);
            return vm;
        }
    }
}
