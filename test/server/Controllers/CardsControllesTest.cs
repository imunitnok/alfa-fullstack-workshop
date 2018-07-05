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

        /// <summary>
        /// Simple test without cards adding
        /// </summary>
        [Fact]
        public void GetCardsPassed()
        {
            // Test
            var cards = controller.Get();

            // Assert
            mock.Verify(r => r.GetCards(), Times.AtMostOnce());
            Assert.Equal(3, cards.Count());
        }

        /// <summary>
        /// Check controller behavior if all got data is valid
        /// </summary>
        [Theory]
        [InlineData("myNewCard", CardType.VISA, Currency.RUR)]
        [InlineData("otherName", CardType.MIR, Currency.USD)]
        [InlineData("megaCard", CardType.MASTERCARD, Currency.EUR)]
        public void NewCardPassed(string name, CardType type, Currency currency)
        {
            var newCard = new NewCard(name, currency.ToString(), type.ToString());
            // Test
            var card = controller.Post(newCard);

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

        /// <summary>
        /// Check controller behavior if in some place data not valid
        /// </summary>
        [Theory]
        [InlineData("my salary", "VISA", "RUR")] // Card with name "my salary" already exists
        [InlineData("otherName", "MIRR", "USD")]
        [InlineData("megaCard", "MAESTRO", "RUS")]
        public void NewCardException(string name, string type, string currency)
        {
            var newCard = new NewCard(name, currency, type);
            //Assert & Test
            Assert.Throws<UserDataException>(() => controller.Post(newCard));

            var cards = controller.Get();
            // Assert
            mock.Verify(r => r.GetCards(), Times.AtMostOnce());
            Assert.Equal(3, cards.Count());
        }

        /// <summary>
        /// Check controller behavior if card added twice
        /// </summary>
        [Fact]
        public void DubleCardException() {
            var newCard = new NewCard("MyNewCard", "USD", "MAESTRO");
            //Assert & Test
            controller.Post(newCard);
            Assert.Throws<UserDataException>(() => controller.Post(newCard));
        }

        /// <summary>
        /// Add new card and check all attributes in it
        /// </summary>
        [Theory]
        [InlineData("myNewCard", CardType.VISA, Currency.RUR)]
        [InlineData("otherName", CardType.MIR, Currency.USD)]
        [InlineData("megaCard", CardType.MASTERCARD, Currency.EUR)]
        public void GetNewCardPassed(string name, CardType type, Currency currency)
        {
            var newCard = new NewCard(name, currency.ToString(), type.ToString());
            // Test
            var card = controller.Post(newCard);
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

        /// <summary>
        /// Check controller behavior if GET takes wrong card number (Lun algorithm)
        /// </summary>
        [Theory]
        [InlineData("1234 1234 1233 1234")]
        [InlineData("12341233123")]
        [InlineData("")]
        [InlineData(null)]
        public void GetCardUserException_wrongCardNumber(string number) {
            Assert.Throws<UserDataException>(() => controller.Get(number));
        }

        /// <summary>
        /// Check controller behavior if GET takes wrong emmiter card number
        /// </summary>
        [Theory]
        [InlineData("5395029009021990")]
        [InlineData("4978588211036789")]
        public void GetCardUserException_wrongEmmiter(string number) {
            Assert.Throws<UserDataException>(() => controller.Get(number));
        }

        /// <summary>
        /// Delete always return status code 405 (card removal is prohibited)
        /// </summary>
        [Fact]
        public void DeleteCardException() {
            Assert.True(controller.StatusCode(405).ToString() == controller.Delete("Any string").ToString());
        }
    }
}