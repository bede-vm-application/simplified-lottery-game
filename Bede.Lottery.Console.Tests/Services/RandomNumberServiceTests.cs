namespace Bede.Lottery.Services
{
    [TestClass]
    public sealed class RandomNumberServiceTests
    {
        [TestMethod]
        public void NextTest()
        {
            const int minValue = 1;
            const int maxValue = 100;
            var service = new RandomNumberService();

            var randomValues = Enumerable.Range(1, 1000).Select(_ => service.Next(minValue, maxValue));

            randomValues.Should()
                .AllSatisfy(value => value.Should()
                    .BeGreaterThanOrEqualTo(minValue)
                    .And
                    .BeLessThanOrEqualTo(maxValue));
        }
    }
}
