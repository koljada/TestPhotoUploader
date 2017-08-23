using Microsoft.WindowsAzure.Storage;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using TestPhotoUploader.Services.Implemntations;
using TestPhotoUploader.Services.Interfaces;

namespace TestPhotoUploader.App_Start
{
    public class SimpleInjectorInitializer
    {
        private static Container _container = null;

        public static void Initialize()
        {
            _container = new Container();

            RegisterDependencies();

            // This is an extension method from the integration package.
            _container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            _container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(_container));
        }

        private static void RegisterDependencies()
        {
            string storageConnectionString = ConfigurationManager.AppSettings["StorageConnectionString"];

            _container.Register<CloudStorageAccount>(() => CloudStorageAccount.Parse(storageConnectionString));

            _container.Register<IBlobService, BlobService>();
            _container.Register<ICognitiveService, CognitiveService>();
            _container.Register(typeof(ITableService<>), typeof(TableService<>));
            _container.Register(typeof(IMessageService<>), typeof(ServiceBusMessageService<>));
            //_container.Register(typeof(IMessageService<>), typeof(QueueMessageService<>));

            _container.Register(typeof(IAzureService<>), typeof(AzureService<>));
        }
    }
}