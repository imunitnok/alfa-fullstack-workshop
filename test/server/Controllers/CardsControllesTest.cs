using Server.Controllers;
using Server.Data;
using Xunit;
using Moq;
using System.Linq;
using System.Collections.Generic;
using Server.Models;
using Server.Services;
using Server.Infrastructure;
using Server.Exceptions;

namespace ServerTest.ControllersTest
{
    public class CardsControllerTest
    {
        private readonly ICardService _cardService = new CardService();

        public CardsController controller;
        public Mock<IBankRepository> mock;

        //Generate arrange in constructor
        public CardsControllerTest() {
            mock = new Mock<IBankRepository>();
            var mockUser = FakeDataGenerator.GenerateFakeUser();
            var mockCards = FakeDataGenerator.GenerateFakeCardsToUser(mockUser);

            mock.Setup(r => r.GetCurrentUser()).Returns(mockUser);
            mock.Setup(r => r.GetCards()).Returns(mockUser.Cards);
            mock.Setup(r => r.GetCard(It.IsAny<string>()))
                .Returns((string s) => mockUser.Cards.FirstOrDefault(x => x.CardNumber == s));

            controller = new CardsController(mock.Object, _cardService);
        }

        [Fact]
        public void GetCardsPassed()
        {
            // Test
            var cards = controller.Get();

            // Assert
            mock.Verify(r => r.GetCards(), Times.AtMostOnce());
            Assert.Equal(3, cards.Count());
        }

        [Theory]
        [InlineData("myNewCard", CardType.VISA, Currency.RUR)]
        [InlineData("otherName", CardType.MIR, Currency.USD)]
        [InlineData("megaCard", CardType.MASTERCARD, Currency.EUR)]
        public void NewCardPassed(string name, CardType type, Currency currency)
        {
            // Test
            var card = controller.Post(name, type.ToString(), currency.ToString());

            // Assert
            mock.Verify(r => r.GetCurrentUser(), Times.AtMostOnce());
            Assert.Equal(name, card.CardName);
            Assert.Equal(type, card.CardType);
            Assert.Equal(currency, card.Currency);

            var cards = controller.Get();

            // Assert
            mock.Verify(r => r.GetCards(), Times.AtMostOnce());
            Assert.Equal(4, cards.Count());
        }

        [Theory]
        [InlineData("my salary", "VISA", "RUR")]
        [InlineData("otherName", "MIRR", "USD")]
        [InlineData("megaCard", "MAESTRO", "RUS")]
        public void NewCardException(string name, string type, string currency)
        {
            //Assert & Test
            Assert.Throws<UserDataException>(() => controller.Post(name, type, currency));

            var cards = controller.Get();
            // Assert
            mock.Verify(r => r.GetCards(), Times.AtMostOnce());
            Assert.Equal(3, cards.Count());
        }

        [Fact]
        public void DubleCardException() {
            //Assert & Test
            controller.Post("MyNewCard","MAESTRO","USD");
            Assert.Throws<UserDataException>(() => controller.Post("MyNewCard","MAESTRO","USD"));
        }

        [Theory]
        [InlineData("myNewCard", CardType.VISA, Currency.RUR)]
        [InlineData("otherName", CardType.MIR, Currency.USD)]
        [InlineData("megaCard", CardType.MASTERCARD, Currency.EUR)]
        public void GetNewCardPassed(string name, CardType type, Currency currency)
        {
            // Test
            var card = controller.Post(name, type.ToString(), currency.ToString());
            var cardNumber = card.CardNumber;
            // Assert
            mock.Verify(r => r.GetCurrentUser(), Times.AtMostOnce());
            
            card = controller.Get(cardNumber);

            Assert.Equal(name, card.CardName);
            Assert.Equal(type, card.CardType);
            Assert.Equal(currency, card.Currency);

            var cards = controller.Get();

            // Assert
            mock.Verify(r => r.GetCards(), Times.AtMostOnce());
            Assert.Equal(4, cards.Count());
        }

        [Fact]
        public void DeleteCardException() {
            Assert.Equal(controller.StatusCode(405), controller.Delete("Any string"));
        }
    }
}