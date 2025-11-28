using MonoTask.Service.Models;
using System.Data.Entity;

namespace MonoTask.Service.Context
{
    public class VehicleContext : DbContext
    {
        public DbSet<VehicleMake> VehicleMakes { get; set; }
        public DbSet<VehicleModel> VehicleModels { get; set; }

        public VehicleContext() : base("name=MyDbConnection") { }

    }
}
