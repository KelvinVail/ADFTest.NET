using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace ADFTests.Net.IntegrationTests
{
    public class PLStageAuthorsHelper
{
    public string PipelineOutcome { get; private set; }
 
    public async Task RunPipeline()
    {
        PipelineOutcome = "Unknown";
 
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Development.json", false)
            .Build();

        // authenticate against Azure
        var context = new AuthenticationContext("https://login.windows.net/" + configuration.GetSection("AZURE_TENANT_ID").Value);
        var cc = new ClientCredential(configuration.GetSection("AZURE_CLIENT_ID").Value, configuration.GetSection("AZURE_CLIENT_SECRET").Value);
        var authResult = await context.AcquireTokenAsync("https://management.azure.com/", cc);
 
        // prepare ADF client
        var cred = new TokenCredentials(authResult.AccessToken);
        using (var adfClient = new DataFactoryManagementClient(cred) { SubscriptionId = configuration.GetSection("AZURE_SUBSCRIPTION_ID").Value })
        {
            var adfName = "adf-testing2-adf";  // name of data factory
            var rgName = "AzureDataFactoryTesting";    // name of resource group that contains the data factory
 
            // run pipeline
            var response = await adfClient.Pipelines.CreateRunWithHttpMessagesAsync(rgName, adfName, "Stage Authors");
            string runId = response.Body.RunId;
 
            // wait for pipeline to finish
            var run = await adfClient.PipelineRuns.GetAsync(rgName, adfName, runId);
            while (run.Status == "Queued" || run.Status == "InProgress" || run.Status == "Canceling")
            {
                Thread.Sleep(2000);
                run = await adfClient.PipelineRuns.GetAsync(rgName, adfName, runId);
            }
            PipelineOutcome = run.Status;
        }
    }
 
    public PLStageAuthorsHelper()
    {
        PipelineOutcome = "Unknown";
    }
}
}
