using Xunit;

namespace ADFTests.Net.IntegrationTests.Pipelines.StageAuthors
{
    public class FirstTests : IClassFixture<StageAuthorsFixture>
    {
        private readonly StageAuthorsFixture _fixture;

        public FirstTests(StageAuthorsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void PipelineShouldSucceed()
        {
            Assert.Equal("Succeeded", _fixture.PipelineOutcome);
        }

        [Fact]
        public void AllRowsShouldBeCopied()
        {
            Assert.Equal(32, _fixture.StagedRowCount);
        }
    }
}
