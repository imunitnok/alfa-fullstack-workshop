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
            => currentUser == null ? currentUser : throw new BusinessLogicException(TypeBusinessException.USER, "User is null");

        /// <summary>
        /// Get range of transactions
        /// </summary>
        /// <param name="cardnumber"></param>
        /// <param name="from">from range</param>
        /// <param name="to">to range</param>
        public IEnumerable<Transaction> GetTranasctions(string cardnumber, int from, int to)
            => throw new NotImplementedException();

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
        public void TransferMoney(decimal sum, string from, string to)
            => throw new NotImplementedException();
    }
}