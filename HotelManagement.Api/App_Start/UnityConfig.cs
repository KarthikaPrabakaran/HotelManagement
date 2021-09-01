using HotelManagement.Business;
using HotelManagement.IRepository;
using HotelManagement.IService;
using HotelManagement.Repository;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace HotelManagement.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<HotelInfoIRepository,HotelInfoRepository>();
            container.RegisterType<HotelInfoIService, HotelInfoService>();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}