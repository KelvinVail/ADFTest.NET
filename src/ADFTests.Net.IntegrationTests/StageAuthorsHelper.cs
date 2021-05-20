using System.Threading.Tasks;

namespace ADFTests.Net.IntegrationTests
{
    public class StageAuthorsHelper : DataFactoryHelper
    {
        public async Task RunPipeline()
        {
            await RunPipeline("Stage Authors");
        }
    }
}
