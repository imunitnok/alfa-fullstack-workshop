using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Server.Exceptions;
using Server.Infrastructure;
using Server.Services;

namespace Server.Models
{
    /// <summary>
    /// Card domain model
    /// </summary>
    public class Card
    {
        private string _cardNumber;

        private IList<Transaction> _transactions = new List<Transaction>();

        private readonly ICardService cardService = new CardService();
        private readonly IBusinessLogicService blService = new BusinessLogicService();

        /// <summary>
        /// Create new Card with validation
        /// </summary>
        /// <param name="cardNumber">Card number</param>
        /// <param name="cardName">Short card name</param>
        /// <param name="cardType">cardType with enum</param>
        /// <param name="currency">card currency</param>
        /// <param name="dtOpenCard">Date of card opening</param>
        /// <param name="validity">Number of years of validity</param>
        public Card(string cardNumber, string cardName, CardType cardType,
            Currency currency = Currency.RUR, DateTime? dtOpenCard = null, int validity = 3)
        {
            if (!cardService.CheckCardNumber(cardNumber))
                throw new UserDataException("Incorrect cardNumber", cardNumber);

            if (!cardService.CheckCardEmmiter(cardNumber))
                throw new UserDataException("Wrong emitted cardNumber", cardNumber);

            if (cardType == CardType.UNKNOWN)
                throw new UserDataException("Wrong type card", cardType.ToString());

            if (dtOpenCard == null)
                dtOpenCard = DateTime.Today;

            else if (dtOpenCard > DateTime.Today)
                new UserDataException("You can't open card in future", dtOpenCard.Value.ToString("yyyy-MM-dd"));

            if (validity <= 0 || validity > 5)
                new UserDataException("Incorrect validaty. Must be [1-5] years", validity.ToString());

            CardNumber = cardService.CreateNormalizeCardNumber(cardNumber);
            CardName = cardName;
            DTOpenCard = dtOpenCard.Value;
            Validity = validity;
            Currency = currency;
            CardType = cardType;
        }

        /// <summary>
        /// Card number. Set only from constructor.
        /// </summary>
        /// <returns><see langword="string"/> card number representation</returns>
        public string CardNumber { get => _cardNumber; private set => _cardNumber = value; }

        /// <summary>
        /// Short name of the cards
        /// </summary>
        /// <returns><see langword="string"/> short card name representation</returns>
        public string CardName { get; set; }

        /// <summary>
        /// Card <see cref="Currency"/>
        /// </summary>
        public Currency Currency { get; }

        /// <summary>
        /// Card <see cref="CardType"/>
        /// </summary>
        public CardType CardType { get; }

        /// <summary>
        /// DTOpenCard
        /// </summary>
        public DateTime DTOpenCard { get; }

        /// <summary>
        /// Count year's
        /// </summary>
        public int Validity { get; }

        /// <summary>
        /// List of card transactions
        /// </summary>
        /// <returns><see cref="IList"/> of Transactions</returns>
        public IList<Transaction> Transactions => new ReadOnlyCollection<Transaction>(_transactions);

        /// <summary>
        /// This method add new transaction to list of card transaction
        /// </summary>
        /// <param name="transaction"></param>
        public Transaction AddTransaction(Transaction transaction)
        {
            _transactions.Add(transaction);
            return transaction;
        }

        /// <summary>
        /// This method return transactions in range of this card
        /// </summary>
        /// <param name="from">range starts (include element index of from)</param>
        /// <param name="to">range ends (include element index of to)</param>
        public IEnumerable<Transaction> GetTransactions(int from, int to) {
            to++;
            if(from < to && to <=0 || to > from && from > _transactions.Count - 1) return new List<Transaction>();
            if(from < 0) from = 0;
            if(to > _transactions.Count) to = _transactions.Count;
            if(from >= to) new UserDataException("Incorrect range", "from: " + from + " to: " + to);
            return new ReadOnlyCollection<Transaction>(((List<Transaction>) _transactions).GetRange(from, to - from));
        }
    }

    /// <summary>
    /// Class contains neccessary data for card creation
    /// </summary>
    public class NewCard {
        public NewCard(string cardName, string currency, string cardType) 
        {
            CardName = cardName;
            Currency = currency;
            CardType = cardType;
        }
        public string CardName {get;}
        public string Currency {get;}
        public string CardType {get;}
    }
}