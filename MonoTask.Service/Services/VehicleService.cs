using AutoMapper;
using MonoTask.Service.Context;
using MonoTask.Service.DTO;
using MonoTask.Service.Interfaces;
using MonoTask.Service.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;

namespace MonoTask.Service.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly VehicleContext _context;
        private readonly ILogger _logger;
        public VehicleService(VehicleContext context, ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #region VehicleMake
        //================================//
        //   VehicleMake functionalities  //
        //================================//
        public async Task<List<VehicleMake>> GetAllVehicleMakes()
        {
            try
            {
                var model = await _context.VehicleMakes.Include(m => m.Models).ToListAsync();
                return model;
            }

            catch (Exception GetException)
            {
                _logger.Error("Error retrieving VehicleMakes.", GetException);
                throw;
            }
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
            try
            {
                var totalCount = await sort.CountAsync();
                var items = await sort.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();

                var pagedResult = new PagingResult<VehicleMake>(items, totalCount, pagination.Page, pagination.PageSize);
                return pagedResult;
            }
            catch (Exception GetException)
            {
                _logger.Error("Error retrieving the full list of vehicle makes.", GetException);
                throw;
            }
        }
        public async Task<VehicleMake> GetVehicleMakeById(int id)
        {
            try
            {
                var model = await _context.VehicleMakes.Include(m => m.Models).Where(m => m.Id == id).SingleOrDefaultAsync();
                return model;
            }

            catch (Exception GetException)
            {
                _logger.Error($"Error retrieving a vehicle make by id=.{id}", GetException);
                throw;
            }
        }
        public async Task<bool> EditVehicleMake(VehicleMake editedVehicleMake)
        {
            try
            {
                var existingVehicleMake = await _context.VehicleMakes.FindAsync(editedVehicleMake.Id);
                if (existingVehicleMake == null) return false;

                if (await CheckVehicleMakeForDuplicates(editedVehicleMake) == true) return false;

                existingVehicleMake.Name = editedVehicleMake.Name;
                existingVehicleMake.Abrv = editedVehicleMake.Abrv;

                // Logički nikad se ne bi trebalo dogoditi, 1 ili 0 su prihvatljivi, više od 1 fail na bazi
                if (await _context.SaveChangesAsync() > 1) return false;
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error editing VehicleMake Id={editedVehicleMake.Id}.", ex);
                throw;
            }
        }
        public async Task<bool> CreateVehicleMake(VehicleMake make)
        {

            if (await CheckVehicleMakeForDuplicates(make) == true) return false;
            try
            {
                _context.VehicleMakes.Add(make);

                // Ako nije 1, onda nismo ništa spremili u bazu (tihi fail na bazi)
                if (await _context.SaveChangesAsync() != 1) return false;
                return true;
            }
            catch (Exception CreateException)
            {
                _logger.Error($"Error creating a vehicle make of Name={make.Name}, Abrreviation= {make.Abrv} ", CreateException);
                throw;
            }
        }
        public async Task<bool> DeleteVehicleMake(int makeToDelete)
        {
            try
            {
                var doesMakeExist = await _context.VehicleMakes.FindAsync(makeToDelete);
                if (doesMakeExist == null) return false;

                _context.VehicleMakes.Remove(doesMakeExist);

                // Ako nije 1 onda smo potiho failali s brisanjem iz baze
                if (await _context.SaveChangesAsync() != 1) return false;
                return true;
            }
            catch (Exception DeleteException)
            {
                _logger.Error($"Error deleting a vehicle make of id={makeToDelete}.", DeleteException);
                throw;
            }
        }
        public async Task<bool> CheckVehicleMakeForDuplicates(VehicleMake vehicleMake)
        {
            try
            {
                var query = _context.VehicleMakes.Where(o => o.Id != vehicleMake.Id);

                var exists = await query.AnyAsync(m =>
                m.Name.ToLower() == vehicleMake.Name.ToLower() ||
                m.Abrv.ToLower() == vehicleMake.Abrv.ToLower());

                return exists;
            }
            catch (Exception ValidationException)
            {
                _logger.Warn($"VError occurred while trying to validate the make of Name = {vehicleMake.Name}, Abbreviation = {vehicleMake.Abrv} already exists.", ValidationException);
                throw;
            }
        }
        #endregion
        #region VehicleModel
        //================================//
        //   VehicleModel functionalities  //
        //================================//
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
            try
            {
                var totalCount = await sort.CountAsync();
                var items = await sort.Skip((pagination.Page - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();
                var pagedResult = new PagingResult<VehicleModel>(items, totalCount, pagination.Page, pagination.PageSize);

                return pagedResult;

            }
            catch (Exception GetException)
            {
                _logger.Error("Error retrieving the full list of vehicle models.", GetException);
                throw;
            }
        }
        public async Task<VehicleModel> GetVehicleModelById(int id)
        {
            try
            {
                var model = await _context.VehicleModels.FindAsync(id);
                return model;
            }
            catch (Exception GetException)
            {
                _logger.Error($"Error retrieving vehicle model by id= {id}.", GetException);
                throw;
            }
        }
        public async Task<bool> CreateVehicleModel(VehicleModel model)
        {
            try
            {
                if (await CheckVehicleModelForDuplicates(model) == true) return false;
                _context.VehicleModels.Add(model);
                if (await _context.SaveChangesAsync() != 1) return false;
                return true;
            }
            catch (Exception CreateException)
            {
                _logger.Error($"Error creating a vehicle model: Name= {model.Name}, Abbreviation = {model.Abrv}", CreateException);
                throw;
            }
        }
        public async Task<bool> EditVehicleModel(VehicleModel editedVehicleModel)
        {
            try
            {
                var existingVehicleModel = await _context.VehicleModels.FindAsync(editedVehicleModel.Id);
                if (existingVehicleModel == null) return false;

                if (await CheckVehicleModelForDuplicates(editedVehicleModel) == true) return false;

                existingVehicleModel.Name = editedVehicleModel.Name;
                existingVehicleModel.Abrv = editedVehicleModel.Abrv;
                existingVehicleModel.MakeId = editedVehicleModel.MakeId;

                // 0 ili 1 prihvatljivo, više od 1 ne, a logički se nikad ni ne bi trebalo dogoditi
                if (await _context.SaveChangesAsync() > 1) return false;
                return true;
            }
            catch (Exception EditException)
            {
                _logger.Error($"Error editing a vehicle model of id = {editedVehicleModel.Id}, Name = {editedVehicleModel.Name}, Abbreviation = {editedVehicleModel.Abrv}", EditException);
                throw;
            }
        }
        public async Task<bool> DeleteVehicleModel(int modelToDelete)
        {
            try
            {
                var doesModelExist = await _context.VehicleModels.FindAsync(modelToDelete);
                if (doesModelExist == null) return false;

                _context.VehicleModels.Remove(doesModelExist);

                if (await _context.SaveChangesAsync() != 1) return false;
                return true;
            }
            catch (Exception DeleteException)
            {
                _logger.Error($"Error deleting a vehicle model of id={modelToDelete}.", DeleteException);
                throw;
            }
        }
        public async Task<bool> CheckVehicleModelForDuplicates(VehicleModel vehicleModel)
        {
            try
            {

                var query = _context.VehicleModels.Where(o => o.Id != vehicleModel.Id && o.MakeId == vehicleModel.MakeId);

                var exists = await query.AnyAsync(m =>
                m.Name.ToLower() == vehicleModel.Name.ToLower() ||
                m.Abrv.ToLower() == vehicleModel.Abrv.ToLower());

                return exists;
            }
            catch (Exception ValidationException)
            {
                _logger.Warn($"Error occurred while trying to validate the model of Name = {vehicleModel.Name}, Abbreviation = {vehicleModel.Abrv} already exists.", ValidationException);
                throw;
            }
        }
        #endregion
    }
}
