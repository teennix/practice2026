using Xunit;
using Moq;
using task04;

namespace task04tests
{
    public class SpaceshipTests
    {
        [Fact]
        public void Cruiser_ShouldHaveCorrectStats()
        {
            ISpaceship cruiser = new Cruiser();
            Assert.Equal(50, cruiser.Speed);
            Assert.Equal(100, cruiser.FirePower);
        }

        [Fact]
        public void Fighter_ShouldBeFasterThanCruiser()
        {
            var fighter = new Fighter();
            var cruiser = new Cruiser();
            Assert.True(fighter.Speed > cruiser.Speed);
        }

        [Fact]
        public void Fighter_ShouldHaveCorrectStats()
        {
            ISpaceship fighter = new Fighter();
            Assert.Equal(100, fighter.Speed);
            Assert.True(fighter.FirePower < 50);
        }

        [Fact]
        public void MockSpaceship_CanVerifyMethodInvocations()
        {
            var mockSpaceship = new Mock<ISpaceship>();

            mockSpaceship.Object.MoveForward();
            mockSpaceship.Object.Rotate(90);
            mockSpaceship.Object.Fire();

            mockSpaceship.Verify(s => s.MoveForward(), Times.Once);
            mockSpaceship.Verify(s => s.Rotate(90), Times.Once);
            mockSpaceship.Verify(s => s.Fire(), Times.Once);
        }
    }
}