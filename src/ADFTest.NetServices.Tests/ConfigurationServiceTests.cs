using System;
using System.Collections.Generic;
using ADFTest.Net.Services.Tests.TestDoubles;
using Xunit;

namespace ADFTest.Net.Services.Tests
{
    public class ConfigurationServiceTests
    {
        private readonly ConfigurationRootStub _config = new ();
        private readonly SecretClientStub _keyVault = new ();
        private readonly ConfigurationService _service;

        public ConfigurationServiceTests()
        {
            _service = new ConfigurationService(_config, _keyVault);
        }

        [Fact]
        public void ReturnNullIfVariableNameIsNull()
        {
            var value = _service.GetConfiguration(null);

            Assert.Null(value);
        }

        [Theory]
        [InlineData("VariableName")]
        [InlineData("DoesNotExist")]
        [InlineData("ConnectionString")]
        public void ThrowIfVariableNameDoesNotExist(string name)
        {
            var ex = Assert.Throws<KeyNotFoundException>(() => _service.GetConfiguration(name));
            Assert.Equal($"The configuration '{name}' was not found.", ex.Message);
        }

        [Theory]
        [InlineData("TestVariable", "TestValue")]
        [InlineData("ConnectionString", "MyConnectionString")]
        [InlineData("Url", "http://www.test.com/")]
        public void ReturnVariableValueIfItIsAnEnvironmentVariable(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value);

            var actual = _service.GetConfiguration(name);

            Assert.Equal(value, actual);
            Environment.SetEnvironmentVariable(name, null);
        }

        [Theory]
        [InlineData("TestVariable", "TestValue")]
        [InlineData("ConnectionString", "MyConnectionString")]
        [InlineData("Url", "http://www.test.com/")]
        public void ReturnVariableValueIfItIsInTheConfigurationRoot(string name, string value)
        {
            _config.Add(name, value);

            var actual = _service.GetConfiguration(name);

            Assert.Equal(value, actual);
        }

        [Theory]
        [InlineData("TestVariable", "TestValue")]
        [InlineData("ConnectionString", "MyConnectionString")]
        [InlineData("Url", "http://www.test.com/")]
        public void ReturnVariableValueIfItIsInKeyVault(string name, string value)
        {
            _keyVault.SetSecret(name, value);

            var actual = _service.GetConfiguration(name);

            Assert.Equal(value, actual);
        }

        [Fact]
        public void DoNotCheckConfigurationOrKeyVaultIfVariableIsAnEnvironmentVariable()
        {
            Environment.SetEnvironmentVariable("name", "value");

            _service.GetConfiguration("name");

            Assert.False(_config.Called);
            Assert.False(_keyVault.Called);
        }

        [Fact]
        public void DoNotCheckKeyVaultIfVariableIsInConfiguration()
        {
            _config.Add("name", "value");

            _service.GetConfiguration("name");

            Assert.False(_keyVault.Called);
        }
    }
}
