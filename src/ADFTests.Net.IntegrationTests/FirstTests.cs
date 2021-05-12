using System.Threading.Tasks;
using Xunit;

namespace ADFTests.Net.IntegrationTests
{
    public class FirstTests
    {
        [Fact]
        public async Task PipelineShouldSucceed()
        {
            var helper = new PLStageAuthorsHelper();
            await helper.RunPipeline();

            Assert.Equal("Succeeded", helper.PipelineOutcome);
        }
    }
}
