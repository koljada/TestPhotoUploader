using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace FunctionDemoApp
{
    public static class MyFirstAzureFunction
    {
        [FunctionName("MyFirstAzureFunction")]
        public static void Run([QueueTrigger("testqueueu", Connection = "AzureWebJobsStorage")]string myQueueItem, TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");
            log.Warning("Just Testing!");
        }
    }
}
