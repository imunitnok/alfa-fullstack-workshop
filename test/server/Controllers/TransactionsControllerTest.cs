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
using System;

namespace ServerTest.ControllersTest
{
    public class TransactionsControllerTest
    {
        private const decimal eps = 0.0000001M;
        private readonly ICardService _cardService = new CardService();

        private readonly IBusinessLogicService _blService = new BusinessLogicService();

        public TransactionsController controller;
        public Mock<IBankRepository> mock;

        //Generate arrange in constructor
        public TransactionsControllerTest() {
            mock = new Mock<IBankRepository>();
            var mockUser = FakeDataGenerator.GenerateFakeUser();
            var mockCards = FakeDataGenerator.GenerateFakeCardsToUser(mockUser);

            mock.Setup(r => r.GetCurrentUser()).Returns(mockUser);
            mock.Setup(r => r.GetCard(It.IsAny<string>()))
                .Returns((string s) => mockUser.Cards.FirstOrDefault(x => x.CardNumber == s));
            mock.Setup(r => r.GetTransactions(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns((string s, int from, int to) => {
                    var card = mock.Object.GetCard(s);
                    if(card == null)
                        throw new BusinessLogicException(TypeBusinessException.CARD, "Card is null", "Карта не найдена");
                    return card.GetTransactions(from, to);
                });
            mock.Setup(r => r.TransferMoney(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((decimal sum, string from, string to) => {
                    var cardFrom = mock.Object.GetCard(from);
                    var cardTo = mock.Object.GetCard(to);
                    var transaction = new Transaction(sum, cardFrom, cardTo);
                    cardFrom.AddTransaction(transaction);
                    cardTo.AddTransaction(transaction);
                    return transaction;
                });

            controller = new TransactionsController(mock.Object, _cardService);
        }

        [Fact]
        public void GetTransactionsPassed() {
            var card = mock.Object.GetCurrentUser().OpenNewCard("myTestCard", Currency.RUR, CardType.MAESTRO);
            var transactions = controller.Get(card.CardNumber);

            Assert.Equal(1, transactions.Count());
            var transaction = transactions.First();
            Assert.Equal(null, transaction.CardFromNumber);
            Assert.Equal(card.CardNumber, transaction.CardToNumber);
            Assert.True(Math.Abs(10M - _blService.GetConvertSum(transaction.ToSum, card.Currency, Currency.RUR)) < eps);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void GetTransactionsFromPassed(int from) {
            var card = mock.Object.GetCurrentUser().OpenNewCard("myTestCard", Currency.RUR, CardType.MAESTRO);
            var transactions = controller.Get(card.CardNumber, from);

            Assert.Equal(1 - from, transactions.Count());
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public void GetTransactionsToPassed(int to) {
            var card = mock.Object.GetCurrentUser().OpenNewCard("myTestCard", Currency.RUR, CardType.MAESTRO);
            var transactions = controller.Get(card.CardNumber, 0, to);

            Assert.Equal(1 + to, transactions.Count());
        }

        [Fact]
        public void TransferPassed() {
            var cardFrom = mock.Object.GetCurrentUser().OpenNewCard("myTestCard", Currency.RUR, CardType.MAESTRO);
            var cardTo = mock.Object.GetCurrentUser().OpenNewCard("otherCard", Currency.USD, CardType.VISA);

            var newTransaction = new NewTransaction(5M, cardFrom.CardNumber, cardTo.CardNumber); 
            var transaction = controller.Post(newTransaction);

            Assert.Equal(2, cardFrom.Transactions.Count());
            Assert.Equal(2, cardTo.Transactions.Count());

            var fcTrans = cardFrom.Transactions.Last();
            var tcTrans = cardTo.Transactions.Last();
            Assert.False(fcTrans == null);
            Assert.False(tcTrans == null);
            Assert.Equal(fcTrans, tcTrans);
            Assert.Equal(newTransaction.From, fcTrans.CardFromNumber);
            Assert.Equal(newTransaction.To, tcTrans.CardToNumber);
        }
    }
}