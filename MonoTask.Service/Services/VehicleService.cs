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
        public async Task<PagingResult<VehicleMake>> GetVehicleMakesByParameters(VehicleQuery query, PaginationRequest pagination)
        {
            var fetch = _context.VehicleMakes.Include(m => m.Models);
            if (query.CurrentMakeId.HasValue)
            {
                fetch = fetch.Where(m => m.Id == query.CurrentMakeId);
            }
            if (!string.IsNullOrWhiteSpace(query.CurrentSearchTerm)) fetch = fetch.Where(
                o => o.Name.Contains(query.CurrentSearchTerm)
                || o.Abrv.Contains(query.CurrentSearchTerm)
                );
            
            var orderByClause = query.CurrentSortDescending ? query.CurrentSortColumn + " desc" : query.CurrentSortColumn;
            var sort = fetch.OrderBy(orderByClause);

            var totalCount = await sort.CountAsync();
            var items = await sort.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();

            var pagedResult = new PagingResult<VehicleMake>(items, totalCount, pagination.Page, pagination.PageSize);
            return pagedResult;
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
            var query = _context.VehicleMakes.Where(o => o.Id != vehicleMake.Id);
            
            var exists = await query.AnyAsync(m => 
            m.Name.ToLower() == vehicleMake.Name.ToLower() ||
            m.Abrv.ToLower() == vehicleMake.Abrv.ToLower());

            return exists;

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
        public async Task<PagingResult<VehicleModel>> GetAllVehicleModels(VehicleQuery query, PaginationRequest pagination)
        {
            var fetch = _context.VehicleModels.Include(m => m.Make);
            if (query.CurrentMakeId.HasValue)
            {
                fetch = fetch.Where(m => m.MakeId == query.CurrentMakeId);
            }
            if (!string.IsNullOrWhiteSpace(query.CurrentSearchTerm)) fetch = fetch.Where(
                o => o.Name.Contains(query.CurrentSearchTerm)
                || o.Abrv.Contains(query.CurrentSearchTerm)
                || o.Make.Name.Contains(query.CurrentSearchTerm)
            ); ;
            string orderByClause = query.CurrentSortDescending ? query.CurrentSortColumn + " desc" : query.CurrentSortColumn;

            var sort = fetch.OrderBy(orderByClause);

            var totalCount = await sort.CountAsync(); // total before paging
            var items = await sort.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();
            var pagedResult = new PagingResult<VehicleModel>(items, totalCount, pagination.Page, pagination.PageSize);
            
            return pagedResult;
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
            var query = _context.VehicleModels.Where(o => o.Id != vehicleModel.Id && o.MakeId == vehicleModel.MakeId);

            var exists = await query.AnyAsync(m => 
            m.Name.ToLower() == vehicleModel.Name.ToLower() ||
            m.Abrv.ToLower() == vehicleModel.Abrv.ToLower());

            return exists;
        }
        #endregion
    }
}
