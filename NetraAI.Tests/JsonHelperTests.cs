using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using NetraAI.Desktop.Utils;

namespace NetraAI.Tests
{
    public class JsonHelperTests
    {
        private class TestClass
        {
            public string Name { get; set; } = string.Empty;
            public int Value { get; set; }
        }

        [Fact]
        public void Serialize_ValidObject_ReturnsJsonString()
        {
            var obj = new TestClass { Name = "Test", Value = 123 };
            var json = JsonHelper.Serialize(obj);
            Assert.Contains("\"Name\": \"Test\"", json);
            Assert.Contains("\"Value\": 123", json);
        }

        [Fact]
        public void Serialize_NullObject_ReturnsEmptyString()
        {
            TestClass? obj = null;
            var json = JsonHelper.Serialize(obj!);
            Assert.Equal(string.Empty, json);
        }

        [Fact]
        public void Deserialize_ValidJson_ReturnsObject()
        {
            var json = "{\"Name\":\"Test\",\"Value\":123}";
            var obj = JsonHelper.Deserialize<TestClass>(json);
            Assert.NotNull(obj);
            Assert.Equal("Test", obj.Name);
            Assert.Equal(123, obj.Value);
        }

        [Fact]
        public void Deserialize_NullOrWhitespace_ReturnsNull()
        {
            Assert.Null(JsonHelper.Deserialize<TestClass>(null!));
            Assert.Null(JsonHelper.Deserialize<TestClass>(string.Empty));
            Assert.Null(JsonHelper.Deserialize<TestClass>("   "));
        }

        [Fact]
        public void FileOperations_SaveAndLoad_Succeeds()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"jsonhelper_test_{Guid.NewGuid()}.json");
            try
            {
                var obj = new TestClass { Name = "FileTest", Value = 456 };
                var saveResult = JsonHelper.SerializeToFile(obj, tempFile);
                Assert.True(saveResult);
                Assert.True(File.Exists(tempFile));

                var loadedObj = JsonHelper.DeserializeFromFile<TestClass>(tempFile);
                Assert.NotNull(loadedObj);
                Assert.Equal("FileTest", loadedObj.Name);
                Assert.Equal(456, loadedObj.Value);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }

        [Fact]
        public async Task FileOperationsAsync_SaveAndLoad_Succeeds()
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"jsonhelper_async_test_{Guid.NewGuid()}.json");
            try
            {
                var obj = new TestClass { Name = "AsyncFileTest", Value = 789 };
                var saveResult = await JsonHelper.SerializeToFileAsync(obj, tempFile);
                Assert.True(saveResult);
                Assert.True(File.Exists(tempFile));

                var loadedObj = await JsonHelper.DeserializeFromFileAsync<TestClass>(tempFile);
                Assert.NotNull(loadedObj);
                Assert.Equal("AsyncFileTest", loadedObj.Name);
                Assert.Equal(789, loadedObj.Value);
            }
            finally
            {
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
        }
    }
}
