using System;
using Moq;
using Xunit;
using NetraAI.Desktop.Services;
using NetraAI.Desktop.Utils;

namespace NetraAI.Tests
{
    public class NavigationServiceTests
    {
        [Fact]
        public void NavigateTo_NullOrEmptyName_LogsWarning()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var service = new NavigationService(mockLogger.Object);

            // Act
            service.NavigateTo(null!);
            service.NavigateTo("");
            service.NavigateTo("   ");

            // Assert
            mockLogger.Verify(l => l.Warning(It.Is<string>(s => s.Contains("null or empty"))), Times.Exactly(3));
        }

        [Fact]
        public void NavigateTo_UnregisteredView_LogsWarning()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var service = new NavigationService(mockLogger.Object);

            // Act
            service.NavigateTo("SomeRandomView");
            service.NavigateTo("  AnotherInvalidView  ");

            // Assert
            mockLogger.Verify(l => l.Warning(It.Is<string>(s => s.Contains("not registered") && s.Contains("SomeRandomView"))), Times.Once);
            mockLogger.Verify(l => l.Warning(It.Is<string>(s => s.Contains("not registered") && s.Contains("AnotherInvalidView"))), Times.Once);
        }

        [Fact]
        public void InitialState_UninitializedAndNullCurrentView()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var service = new NavigationService(mockLogger.Object);

            // Assert
            Assert.False(service.IsInitialized);
            Assert.Null(service.CurrentViewName);
            Assert.Null(service.GetCurrentWindow());
        }
    }
}
