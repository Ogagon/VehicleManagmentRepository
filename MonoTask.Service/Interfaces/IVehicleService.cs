using MonoTask.Service.DTO;
using MonoTask.Service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonoTask.Service.Interfaces
{
    public interface IVehicleService
    {
        // VehicleMake
        Task<List<VehicleMake>> GetAllVehicleMakes();
        Task<PagingResult<VehicleMake>> GetVehicleMakesByParameters(VehicleQuery query, PaginationRequest pagination);
        Task<VehicleMake> GetVehicleMakeById(int id);
        Task<bool> CreateVehicleMake(VehicleMake make);
        Task<bool> EditVehicleMake(VehicleMake editedVehicleMake);
        Task<bool> DeleteVehicleMake(int makeToDelete);
        Task<bool> CheckVehicleMakeForDuplicates(VehicleMake vehicleMake);

        // VehicleModel
        Task<PagingResult<VehicleModel>> GetAllVehicleModels(VehicleQuery query, PaginationRequest pagination);
        Task<VehicleModel> GetVehicleModelById(int id);
        Task<bool> CreateVehicleModel(VehicleModel model);
        Task<bool> EditVehicleModel(VehicleModel editedVehicleModel);
        Task<bool> DeleteVehicleModel(int modelToDelete);
        Task<bool> CheckVehicleModelForDuplicates(VehicleModel vehicleModel);

    }
}
