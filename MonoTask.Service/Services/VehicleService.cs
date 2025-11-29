using AutoMapper;
using MonoTask.Service.Context;
using MonoTask.Service.DTO;
using MonoTask.Service.Interfaces;
using MonoTask.Service.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace MonoTask.Service.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly VehicleContext _context;
        private readonly IMapper _mapper;
        public VehicleService(VehicleContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(context));
        }
        #region VehicleMake
        //================================//
        //   VehicleMake functionalities  //
        //================================//
        public async Task<List<VehicleMake>> GetAllVehicleMakes()
        {
            var model = await _context.VehicleMakes.Include(m => m.Models).ToListAsync();
            return model;
        }
        public async Task<(List<VehicleMake> Makes, int TotalItems)> GetVehicleMakesByParameters(VehicleQuery query, int page, int pageSize)
        {
            var fetch = _context.VehicleMakes.Include(m => m.Models);
            if (query.CurrentMakeId.HasValue)
            {
                fetch = fetch.Where(m => m.Id == query.CurrentMakeId);
            }
            var filter = FilterVehicleMakesBySearchTerm(fetch, query.CurrentSearchTerm);
            var sort = SortVehicleMakes(filter, query.CurrentSortColumn, query.CurrentSortDescending);

            var totalCount = await sort.CountAsync();
            var items = await sort.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }
        public async Task<VehicleMake> GetVehicleMakeById(int? id)
        {
            var model = await _context.VehicleMakes.Include(m => m.Models).Where(m => m.Id == id).SingleOrDefaultAsync();
            return model;
        }
        public async Task<bool> EditVehicleMake(VehicleMake editedVehicleMake)
        {
            var existingVehicleMake = await _context.VehicleMakes.FindAsync(editedVehicleMake.Id);
            if (existingVehicleMake == null) return false;

            if (await CheckVehicleMakeForDuplicates(editedVehicleMake) == true) return false;

            existingVehicleMake.Name = editedVehicleMake.Name;
            existingVehicleMake.Abrv = editedVehicleMake.Abrv;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CreateVehicleMake(VehicleMake make)
        {
            if (await CheckVehicleMakeForDuplicates(make) == true) return false;
            _context.VehicleMakes.Add(make);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteVehicleMake(int makeToDelete)
        {
            var doesMakeExist = await _context.VehicleMakes.FindAsync(makeToDelete);
            if (doesMakeExist == null) return false;

            _context.VehicleMakes.Remove(doesMakeExist);

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> CheckVehicleMakeForDuplicates(VehicleMake vehicleMake)
        {
            return await _context.VehicleMakes.Where(o => o.Id != vehicleMake.Id).AnyAsync
                (m => m.Name.Equals(vehicleMake.Name, StringComparison.OrdinalIgnoreCase)
                || m.Abrv.Equals(vehicleMake.Abrv, StringComparison.OrdinalIgnoreCase));
        }
        #endregion
        #region VehicleModel
        //================================//
        //   VehicleModel functionalities  //
        //================================//
        public async Task<List<VehicleModel>> GetVehicleModels()
        {
            var model = await _context.VehicleModels.Include(m => m.Make).ToListAsync();
            return model;
        }
        public async Task<(List<VehicleModel> Models, int TotalCount)> GetAllVehicleModels(VehicleQuery query, int page, int pageSize)
        {
            var fetch = _context.VehicleModels.Include(m => m.Make);
            if (query.CurrentMakeId.HasValue)
            {
                fetch = fetch.Where(m => m.MakeId == query.CurrentMakeId);
            }
            var filter = FilterVehicleModelsBySearchTerm(fetch, query.CurrentSearchTerm);
            var sort = SortVehicleModels(filter, query.CurrentSortColumn, query.CurrentSortDescending);
            var totalCount = await sort.CountAsync(); // total before paging
            var items = await sort.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return (items, totalCount);
        }
        public async Task<VehicleModel> GetVehicleModelById(int? id)
        {
            var model = await _context.VehicleModels.FindAsync(id);
            return model;
        }
        public async Task<bool> CreateVehicleModel(VehicleModel model)
        {
            if (await CheckVehicleModelForDuplicates(model) == true) return false;
            _context.VehicleModels.Add(model);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> EditVehicleModel(VehicleModel editedVehicleModel)
        {
            var existingVehicleModel = await _context.VehicleModels.FindAsync(editedVehicleModel.Id);
            if (existingVehicleModel == null) return false;

            // early exit if duplicate
            if (await CheckVehicleModelForDuplicates(editedVehicleModel) == true) return false;

            existingVehicleModel.Name = editedVehicleModel.Name;
            existingVehicleModel.Abrv = editedVehicleModel.Abrv;
            existingVehicleModel.MakeId = editedVehicleModel.MakeId;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteVehicleModel(int modelToDelete)
        {
            var doesModelExist = await _context.VehicleModels.FindAsync(modelToDelete);
            if (doesModelExist == null) return false;

            _context.VehicleModels.Remove(doesModelExist);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckVehicleModelForDuplicates(VehicleModel vehicleModel)
        {
            return await _context.VehicleModels.Where(o => o.Id != vehicleModel.Id && o.MakeId == vehicleModel.MakeId).AnyAsync
                (m => m.Name.Equals(vehicleModel.Name, StringComparison.OrdinalIgnoreCase)
                || m.Abrv.Equals(vehicleModel.Abrv, StringComparison.OrdinalIgnoreCase)
                );
        }
        #endregion
        #region Sort and filter
        public IQueryable<VehicleMake> SortVehicleMakes(IQueryable<VehicleMake> listToSort, string sortColumn, bool descending)
        {
            if (string.IsNullOrEmpty(sortColumn))
            {
                sortColumn = "Name";
            }
            var orderByClause = descending ? sortColumn + " desc" : sortColumn;

            return listToSort.OrderBy(orderByClause);
        }
        public IQueryable<VehicleModel> SortVehicleModels(IQueryable<VehicleModel> listToSort, string sortColumn, bool descending)
        {
            Dictionary<string, string> allowedSortingColumns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Name", "Name" },
                { "Abrv", "Abrv" },
                { "Make", "Make.Name" }
            };

            string mappedSortColumn;
            if (!allowedSortingColumns.TryGetValue(sortColumn, out mappedSortColumn))
            {
                mappedSortColumn = "Name";
            }
            string orderByClause = descending ? mappedSortColumn + " desc" : mappedSortColumn;

            return listToSort.Include(o => o.Make).OrderBy(orderByClause);
        }
        public IQueryable<VehicleMake> FilterVehicleMakesBySearchTerm(IQueryable<VehicleMake> listToFilter, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return listToFilter;

            searchTerm = searchTerm.ToLower();

            // Get all public instance properties that are strings, except "Id"

            return listToFilter.Where(
                o => o.Name.Contains(searchTerm)
                || o.Abrv.Contains(searchTerm)
            );
        }
        public IQueryable<VehicleModel> FilterVehicleModelsBySearchTerm(IQueryable<VehicleModel> listToFilter, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return listToFilter;

            searchTerm = searchTerm.ToLower();

            // Get all public instance properties that are strings, except "Id"

            return listToFilter.Include(o => o.Make).Where(
                o => o.Name.Contains(searchTerm)
                || o.Abrv.Contains(searchTerm)
                || o.Make.Name.Contains(searchTerm)
            );
        }
        #endregion
    }
}
