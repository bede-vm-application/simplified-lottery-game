namespace Bede.Lottery.Services
{
    using System.Security.Cryptography;

    internal sealed class RandomNumberService : IRandomNumberService
    {
        public int Next(int minValue, int maxValue)
        {
            return RandomNumberGenerator.GetInt32(maxValue - minValue + 1) + minValue;
        }
    }
}
