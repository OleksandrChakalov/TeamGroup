using AutoFixture;
using NUnit.Framework;

namespace GS.Data.Test
{
    public class BaseRepositoryTest
    {
        protected readonly Fixture _fixture;

        public BaseRepositoryTest()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Test]
        public void OmitAutoProperties_SetToTrue_ShouldDisableAutoProperties()
        {
            // Arrange
            var fixture = new Fixture();

            // Act
            fixture.OmitAutoProperties = true;

            // Assert
            Assert.IsTrue(fixture.OmitAutoProperties);
        }

        [Test]
        public void Engine_ShouldNotBeNull()
        {
            // Arrange
            var fixture = new Fixture();

            // Act
            var engine = fixture.Engine;

            // Assert
            Assert.IsNotNull(engine);
        }

        [Test]
        public void OmitAutoProperties_SetToFalse_ShouldEnableAutoProperties()
        {
            // Arrange
            var fixture = new Fixture();

            // Act
            fixture.OmitAutoProperties = false;

            // Assert
            Assert.IsFalse(fixture.OmitAutoProperties);
        }

        [Test]
        public void RepeatCount_ShouldBeSetAndGetCorrectly()
        {
            // Arrange
            var fixture = new Fixture();

            // Act
            fixture.RepeatCount = 5;
            var repeatCount = fixture.RepeatCount;

            // Assert
            Assert.AreEqual(5, repeatCount);
        }
    }
}
