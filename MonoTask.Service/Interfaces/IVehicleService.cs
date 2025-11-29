using MonoTask.Service.DTO;
using MonoTask.Service.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonoTask.Service.Interfaces
{
    public interface IVehicleService
    {
        // VehicleMake
        Task<List<VehicleMake>> GetAllVehicleMakes();
        Task<(List<VehicleMake> Makes, int TotalItems)> GetVehicleMakesByParameters(VehicleQuery query, int page, int pageSize);
        Task<VehicleMake> GetVehicleMakeById(int? id);
        Task<bool> CreateVehicleMake(VehicleMake make);
        Task<bool> EditVehicleMake(VehicleMake editedVehicleMake);
        Task<bool> DeleteVehicleMake(int makeToDelete);
        Task<bool> CheckVehicleMakeForDuplicates(VehicleMake vehicleMake);

        // VehicleModel
        Task<List<VehicleModel>> GetVehicleModels();
        Task<(List<VehicleModel> Models, int TotalCount)> GetAllVehicleModels(VehicleQuery query, int page, int pageSize);
        Task<VehicleModel> GetVehicleModelById(int? id);
        Task<bool> CreateVehicleModel(VehicleModel model);
        Task<bool> EditVehicleModel(VehicleModel editedVehicleModel);
        Task<bool> DeleteVehicleModel(int modelToDelete);
        Task<bool> CheckVehicleModelForDuplicates(VehicleModel vehicleModel);

        // Sorting and filtering
        IQueryable<VehicleMake> SortVehicleMakes(IQueryable<VehicleMake> listToSort, string sortColumn, bool descending);
        IQueryable<VehicleModel> SortVehicleModels(IQueryable<VehicleModel> listToSort, string sortColumn, bool descending);
        IQueryable<VehicleMake> FilterVehicleMakesBySearchTerm(IQueryable<VehicleMake> listToFilter, string searchTerm);
        IQueryable<VehicleModel> FilterVehicleModelsBySearchTerm(IQueryable<VehicleModel> listToFilter, string searchTerm);

    }
}
