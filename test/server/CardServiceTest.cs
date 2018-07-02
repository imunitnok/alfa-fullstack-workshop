using System;
using Xunit;
using Server.Services;

namespace ServerTest
{
    /// <summary>
    /// Tests for <see cref="CardService"/>>
    /// </summary>
    public class CardServiceTest
    {
        private readonly ICardService cardService = new CardService();

        /// <summary>
        /// Check if this cards numbers is valid
        /// </summary>
        [Theory]
        [InlineData("4083967629457310")]
        [InlineData("5395 0290 0902 1990")]
        [InlineData("   4978 588211036789    ")]
        public void CheckCardNumberPassed(string value)
        {
            var result = cardService.CheckCardNumber(value);

            Assert.True(result, $"{value} is not a valid card number");
        }

        /// <summary>
        /// Check if this cards numbers is not valid
        /// </summary>
        [Theory]
        [InlineData("1234 1234 1233 1234")]
        [InlineData("12341233123")]
        [InlineData("")]
        [InlineData(null)]
        public void CheckCardNumberFailed(string value)
        {
            var result = cardService.CheckCardNumber(value);

            Assert.False(result, $"{value} is a valid card number");
        }

        /// <summary>
        /// Check if this cards numbers is not emitted by AlfaBank
        /// </summary>
        [Theory]
        [InlineData("5395029009021990")]
        [InlineData("4978588211036789")]
        public void CheckCardEmittedFailed(string value)
        {
            var result = cardService.CheckCardEmmiter(value);

            Assert.False(result, $"{value} is a valid Alfabank card number");
        }

        /// <summary>
        /// Check if this cards numbers is not emitted by AlfaBank
        /// </summary>
        [Theory]
        [InlineData("4083969259636239")]
        [InlineData("5101265622568232")]
        public void CheckCardEmittedPassed(string value)
        {
            var result = cardService.CheckCardEmmiter(value);

            Assert.True(result, $"{value} is a not valid Alfabank card number");
        }

        /// <summary>
        /// Extract card type test
        /// </summary>
        [Fact]
        public void CardTypeExtractVisa()
            => Assert.Equal(2, cardService.CardTypeExtract("4083969259636239"));

        /// <summary>
        /// Extract card type test
        /// </summary>
        [Fact]
        public void CardTypeExtractMaster()
            => Assert.Equal(1, cardService.CardTypeExtract("5308276794485221"));

        /// <summary>
        /// Extract card type test
        /// </summary>
        [Fact]
        public void CardTypeExtractMaestro()
            => Assert.Equal(3, cardService.CardTypeExtract("6762302693240520"));

        /// <summary>
        /// Extract card type test
        /// </summary>
        [Fact]
        public void CardTypeExtractFail()
            => Assert.Equal(0, cardService.CardTypeExtract("6762502693240520"));
    }
}
