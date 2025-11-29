using AutoMapper;
using MonoTask.MVC.ViewModels;
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
        public VehicleModelController(IVehicleService service, IMapper mapper)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // GET: VehicleModels
        public async Task<ActionResult> Index(string sortColumn, string searchTerm, int? makeId, bool sortDescending = false, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(sortColumn)) sortColumn = "Name";
            var query = new VehicleQuery(sortColumn, sortDescending, searchTerm, makeId);
            var pagination = new PaginationRequest(page, pageSize);

            var (pagedResult, TotalItems) = await _service.GetAllVehicleModels(query, pagination);
            if (pagedResult == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound);

            SelectList makesList = new SelectList(await _service.GetAllVehicleMakes(), "Id", "Name", makeId);

            VehicleModelViewModel vm = new VehicleModelViewModel
            {
                Items = _mapper.Map<List<VehicleModelViewModel>>(pagedResult),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = TotalItems,
                CurrentSortColumn = sortColumn,
                CurrentSortDescending = sortDescending,
                CurrentSearchTerm = searchTerm,
                CurrentMakeId = makeId,
                Makes = _mapper.Map<IEnumerable<SelectListItem>>(makesList)
            }
            ;
            return View(vm);
        }

        // GET: VehicleModels/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            VehicleModel vehicleModel = await _service.GetVehicleModelById(id);
            if (vehicleModel == null)
            {
                return HttpNotFound();
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
                List<VehicleMake> makes = await _service.GetAllVehicleMakes();
                vm.Makes = _mapper.Map<IEnumerable<SelectListItem>>(makes);

                return View(vm);
            }
            VehicleModel newVehicleModel = _mapper.Map<VehicleModel>(vm);
            bool attemptCreate = await _service.CreateVehicleModel(newVehicleModel);
            if (!attemptCreate)
            {
                SetTempMessage($"Failed to create Vehicle model {newVehicleModel.Name}({newVehicleModel.Abrv}). Most likely, the specified model already exists!", "danger");
                return RedirectToAction("Index");
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
            VehicleModel vehicleModel = await _service.GetVehicleModelById(id);
            if (vehicleModel == null)
            {
                return HttpNotFound();
            }
            VehicleModelViewModel vm = _mapper.Map<VehicleModelViewModel>(vehicleModel);

            List<VehicleMake> makes = await _service.GetAllVehicleMakes();
            if (makes == null)
            {
                return HttpNotFound();
            }

            vm.Makes = _mapper.Map<IEnumerable<SelectListItem>>(makes);
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
                List<VehicleMake> makes = await _service.GetAllVehicleMakes();
                if (makes == null)
                {
                    return HttpNotFound();
                }

                vm.Makes = _mapper.Map<IEnumerable<SelectListItem>>(makes);
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
            VehicleModel vehicleModel = await _service.GetVehicleModelById(id);
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
            var model = await _service.GetVehicleModelById(id);
            if (model == null) return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Specified model doesn't exist!");

            var success = await _service.DeleteVehicleModel(id);
            if (!success) return new HttpStatusCodeResult(HttpStatusCode.NotFound, "The operation to remove the specified vehicle model failed!");

            SetTempMessage($"Vehicle model {model.Name}({model.Abrv}) was removed successfully!");
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
            if (entity == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            return null;
        }
        //Helper da se ne ponavljam previše u kodu
        private void SetTempMessage(string message, string type = "success")
        {
            TempData["Message"] = message;
            TempData["MessageType"] = type;
        }
    }
}
