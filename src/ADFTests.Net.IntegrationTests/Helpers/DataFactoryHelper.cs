using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;

namespace ADFTests.Net.IntegrationTests.Helpers
{
    public class DataFactoryHelper : SettingsHelper, IDisposable
    {
        private readonly string _adfName;
        private readonly string _rgName;
        private IDataFactoryManagementClient _client;
        private bool _disposedValue;

        public DataFactoryHelper()
        {
            _adfName = GetSetting("DataFactoryName");
            _rgName = GetSetting("DataFactoryResourceGroup");
            PipelineOutcome = "Unknown";
        }

        public string PipelineOutcome { get; private set; }

        public async Task RunPipeline(string pipelineName) =>
            PipelineOutcome = await RunAndWait(pipelineName);

        public int RowCount(string tableName)
        {
            using var conn = new SqlConnection(GetSetting("DestinationConnectionString"));
            conn.Open();
            using var cmd = new SqlCommand($"SELECT COUNT(*) FROM {tableName}", conn);
            using var reader = cmd.ExecuteReader();
            reader.Read();
            return reader.GetInt32(0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing) _client.Dispose();

            _disposedValue = true;
        }

        private async Task<string> RunAndWait(string pipelineName) =>
            await Wait(await Run(pipelineName));

        private async Task<string> Run(string pipelineName)
        {
            await SetClient();

            var response = await _client.Pipelines.CreateRunWithHttpMessagesAsync(_rgName, _adfName, pipelineName);
            var runId = response.Body.RunId;
            return runId;
        }

        private async Task SetClient()
        {
            var cred = await Authenticate();

            _client = new DataFactoryManagementClient(cred) {SubscriptionId = GetSetting("AZURE_SUBSCRIPTION_ID")};
        }

        private async Task<TokenCredentials> Authenticate()
        {
            var context = new AuthenticationContext("https://login.windows.net/" + GetSetting("AZURE_TENANT_ID"));
            var cc = new ClientCredential(GetSetting("AZURE_CLIENT_ID"), GetSetting("AZURE_CLIENT_SECRET"));
            var authResult = await context.AcquireTokenAsync("https://management.azure.com/", cc);

            return new TokenCredentials(authResult.AccessToken);
        }

        private async Task<string> Wait(string runId)
        {
            var run = await _client.PipelineRuns.GetAsync(_rgName, _adfName, runId);
            while (run.Status is "Queued" or "InProgress" or "Canceling")
            {
                Thread.Sleep(2000);
                run = await _client.PipelineRuns.GetAsync(_rgName, _adfName, runId);
            }

            return run.Status;
        }
    }
}
