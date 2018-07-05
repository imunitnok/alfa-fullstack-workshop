using System;
using System.Collections.Generic;
using System.Linq;
using Server.Exceptions;
using Server.Infrastructure;
using Server.Models;

namespace Server.Data
{
    /// <summary>
    /// Base implementation for onMemory Storage
    /// </summary>
    public class InMemoryBankRepository : IBankRepository
    {
        private readonly User currentUser;

        public InMemoryBankRepository()
        {
            currentUser = FakeDataGenerator.GenerateFakeUser();
            FakeDataGenerator.GenerateFakeCardsToUser(currentUser);
            //TODO other fakes
        }

        /// <summary>
        /// Get one card by number
        /// </summary>
        /// <param name="cardNumber">number of the cards</param>
        public Card GetCard(string cardNumber) {
            return GetCards().FirstOrDefault(x => x.CardNumber == cardNumber);
        }

        /// <summary>
        /// Getter for cards
        /// </summary>
        public IEnumerable<Card> GetCards() => GetCurrentUser().Cards;

        /// <summary>
        /// Get current logged user
        /// </summary>
        public User GetCurrentUser()
            => currentUser != null ? currentUser : throw new BusinessLogicException(TypeBusinessException.USER, "User is null");

        /// <summary>
        /// Get range of transactions
        /// </summary>
        /// <param name="cardnumber"></param>
        /// <param name="from">from range</param>
        /// <param name="to">to range</param>
        public IEnumerable<Transaction> GetTransactions(string cardNumber, int from, int to) {
            var card = GetCard(cardNumber);
            if(card == null) 
                throw new BusinessLogicException(TypeBusinessException.CARD, "Card is null", "Карта не найдена");
            return card.GetTransactions(from, to);
        }

        /// <summary>
        /// OpenNewCard
        /// </summary>
        /// <param name="cardType">type of the cards</param>
        public void OpenNewCard(CardType cardType) => throw new NotImplementedException();

        /// <summary>
        /// Transfer money
        /// </summary>
        /// <param name="sum">sum of operation</param>
        /// <param name="from">card number</param>
        /// <param name="to">card number</param>
        public Transaction TransferMoney(decimal sum, string from, string to)
        {
            var cardFrom = GetCard(from);
            var cardTo = GetCard(to);
            var transaction = new Transaction(sum, cardFrom, cardTo);
            cardFrom.AddTransaction(transaction);
            cardTo.AddTransaction(transaction);
            return transaction;
        }
    }
}