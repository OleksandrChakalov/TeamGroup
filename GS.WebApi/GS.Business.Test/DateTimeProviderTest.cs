using GS.Business;
using NUnit.Framework;
using System;

namespace GS.Business.Tests
{
    public class DateTimeProviderTests
    {
        private DateTimeProvider _dateTimeProvider;

        [SetUp]
        public void Setup()
        {
            _dateTimeProvider = new DateTimeProvider();
        }

        [Test]
        public void GetUtcNow_ReturnsCurrentUtcTime()
        {
            // Arrange
            var expected = DateTime.UtcNow;

            // Act
            var actual = _dateTimeProvider.GetUtcNow();

            // Assert
            Assert.That(actual, Is.EqualTo(expected).Within(1).Seconds);
        }
    }
}
