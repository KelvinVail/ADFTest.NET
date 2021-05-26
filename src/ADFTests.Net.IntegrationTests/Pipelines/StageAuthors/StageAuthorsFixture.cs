using System.Threading.Tasks;
using ADFTests.Net.IntegrationTests.Helpers;

namespace ADFTests.Net.IntegrationTests.Pipelines.StageAuthors
{
    public class StageAuthorsFixture : DataFactoryHelper
    {
        public StageAuthorsFixture()
        {
            RunPipeline("Stage Authors").Wait();
        }

        public async Task RunPipeline()
        {
            await RunPipeline("Stage Authors");
        }

        public int StagedRowCount => RowCount("dbo.Authors");
    }
}
