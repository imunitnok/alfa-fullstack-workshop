using System;
using System.IO;
using Server.Exceptions;
using Server.Infrastructure;
using Server.Models;
using Server.Services;
using Xunit;

namespace ServerTest.ServicesTest
{
    public class BusinessLogicTest
    {
        private IBusinessLogicService blservice = new BusinessLogicService();

        [Theory]
        [InlineData(Currency.RUR, Currency.USD, 1000, "15,954052329291640076579451181")]
        [InlineData(Currency.EUR, Currency.RUR, 100, "7264")]
        [InlineData(Currency.RUR, Currency.RUR, 100, "100")]
        public void GetConvertSumPassed(Currency from, Currency to, decimal valueIn, string valueOut)
        {
            var decimalValueOut = Convert.ToDecimal(valueOut);
            Assert.Equal(decimalValueOut, blservice.GetConvertSum(valueIn, from, to));
        }

        [Fact]
        public void AddBonusOnOpen_GetBalanceOfCard_Passed()
        {
            var card = new Card("4790878827491205", "4790878827491205", CardType.VISA, Currency.RUR);
            blservice.AddBonusOnOpen(card);
            Assert.Equal(10M, blservice.GetBalanceOfCard(card));
        }

        [Fact]
        public void AddBonusOnOpenException()
            => Assert.Throws<BusinessLogicException>(() => blservice.AddBonusOnOpen(null));

        [Fact]
        public void CheckCardActivityException()
            => Assert.Throws<BusinessLogicException>(() => blservice.CheckCardActivity(null));

        [Fact]
        public void CheckCardActivityPasses()
        {
            var card = new Card("4790878827491205", "4790878827491205", CardType.VISA, Currency.RUR,
                DateTime.Today.AddYears(-6));
            Assert.False(blservice.CheckCardActivity(card));
        }

        [Fact]
        public void GetBalanceOfCardException()
            => Assert.Throws<BusinessLogicException>(() => blservice.GetBalanceOfCard(null));
    }
}