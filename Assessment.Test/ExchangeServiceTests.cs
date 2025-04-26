using Assessment.Contract.Interfaces.ExchangeRate;
using Assessment.Contract.Models;
using Assessment.Service.Currency;
using Moq;
using System.Collections.Concurrent;

namespace Assessment.Service.Vehicle.Tests
{
    [TestClass]
    public class ExchangeServiceTests
    {
        private Mock<IExchangeRateService> _exchangeRateServiceMock;
        private CurrencyService _currencyService;

        [TestInitialize]
        public void Setup()
        {
            _exchangeRateServiceMock = new Mock<IExchangeRateService>();
            _currencyService = new CurrencyService(_exchangeRateServiceMock.Object);

        }

        [TestMethod]
        public async Task ConvertAsync_ShouldReturnExchangeResult_WhenRatesAreAvailable()
        {
            // Arrange
            var request = new ExchangeRequest
            {
                Amount = 100,
                InputCurrency = "AUD",
                OutputCurrency = "USD"
            };

            var rates = new ConcurrentDictionary<string, decimal>([
                    new KeyValuePair<string, decimal>("USD", 0.63M)
            ]);

            _exchangeRateServiceMock
                .Setup(x => x.GetRatesAsync("AUD", It.IsAny<CancellationToken>()))
                .ReturnsAsync(rates);

            // Act
            var result = await _currencyService.ConvertAsync(request, CancellationToken.None);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(63, result.Value);
        }

        [TestMethod]
        public async Task ConvertAsync_ShouldThrowException_WhenRatesAreNull()
        {
            // Arrange
            var request = new ExchangeRequest
            {
                Amount = 100,
                InputCurrency = "USD",
                OutputCurrency = "EUR"
            };
            ConcurrentDictionary<string, decimal>? result = null;
            _exchangeRateServiceMock
                .Setup(x => x.GetRatesAsync("USD", It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<Exception>(() => _currencyService.ConvertAsync(request, CancellationToken.None));
        }

        [TestMethod]
        public async Task ConvertAsync_ShouldThrowException_WhenOutputCurrencyRateIsMissing()
        {
            // Arrange
            var request = new ExchangeRequest
            {
                Amount = 100,
                InputCurrency = "AUD",
                OutputCurrency = "USD"
            };

            var rates = new ConcurrentDictionary<string, decimal>([
                new KeyValuePair<string, decimal>("EUR", 0.63M)
                ]);

            _exchangeRateServiceMock
                .Setup(x => x.GetRatesAsync("AUD", It.IsAny<CancellationToken>()))
                .ReturnsAsync(rates);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<Exception>(() => _currencyService.ConvertAsync(request, CancellationToken.None));
            Assert.AreEqual("Can not find rate of AUD to USD.", exception.Message);
        }
    }
}
