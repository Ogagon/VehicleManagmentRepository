using AutoMapper;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using MonoTask.Service.Context;
using MonoTask.Service.Interfaces;
using MonoTask.Service.Mappings;
using MonoTask.Service.Services;
using Ninject;
using Ninject.Web.Common;
using Ninject.Web.Common.WebHost;
using Ninject.Web.Mvc;
using System.Web.Mvc;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(MonoTask.MVC.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(MonoTask.MVC.App_Start.NinjectWebCommon), "Stop")]
namespace MonoTask.MVC.App_Start
{
    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();

            try
            {
                RegisterServices(kernel);
                DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        private static void RegisterServices(IKernel kernel)
        {
            //Bind DbContext
            kernel.Bind<VehicleContext>()
                  .ToSelf()
                  .InRequestScope();
            //Bind Services
            kernel.Bind<IVehicleService>()
                  .To<VehicleService>()
                  .InRequestScope();
            //Automapper binding
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<VehicleProfile>();
            });

            var mapper = config.CreateMapper();

            kernel.Bind<IMapper>().ToConstant(mapper);
        }
    }
}