using System;
using Server.Exceptions;
using Server.Infrastructure;
using Server.Services;

namespace Server.Models
{
    /// <summary>
    /// Transaction domain model
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Public Time of transaction
        /// </summary>
        public DateTime DateTime { get; } = DateTime.Now;

        /// <summary>
        /// Source Sum in transaction
        /// </summary>
        /// <returns><see langword="decimal"/>representation of the sum transaction</returns>
        public decimal FromSum { get; }

        /// <summary>
        /// Destination Sum in transaction
        /// </summary>
        /// <returns><see langword="decimal"/>representation of the sum transaction</returns>
        public decimal ToSum { get; }

        /// <summary>
        /// Link to valid card
        /// </summary>
        /// <returns><see cref="Card"/></returns>
        public string CardFromNumber { get; }

        /// <summary>
        /// Link to valid card
        /// </summary>
        /// <returns><see cref="Card"/></returns>
        public string CardToNumber { get; }

        private readonly IBusinessLogicService blService = new BusinessLogicService();

        /// <summary>
        /// Create new transaction with validation
        /// </summary>
        /// <param name="sum">Sum of the transaction</param>
        /// <param name="from">Link card from</param>
        /// <param name="to">Link card to</param>
        public Transaction(decimal sum, Card from, Card to)
        {
            if (from == null)
                throw new BusinessLogicException(TypeBusinessException.TRANSACTION,
                "From card is null", "Не найдена карта с которой совершается перевод");

            if (to == null)
                throw new BusinessLogicException(TypeBusinessException.TRANSACTION,
                "To card is null", "Не найдена карта на которую совершается перевод");

            if (from.CardNumber == to.CardNumber)
                throw new BusinessLogicException(TypeBusinessException.TRANSACTION,
               "From card and to card is Equal", "Нельзя перевести на туже карту");

            if (sum <= 0)
                throw new UserDataException("Transaction need more than 0", $"from {from.CardName} to {to.CardName}");

            if (!blService.CheckCardActivity(from))
                throw new BusinessLogicException(TypeBusinessException.CARD,
                "Card is expired", $"Карта {from.CardNumber } просрочена");

            if (!blService.CheckCardActivity(to))
                throw new BusinessLogicException(TypeBusinessException.CARD,
                "Card is expired", $"Карта {to.CardNumber } просрочена");

            if (blService.GetBalanceOfCard(from) < sum)
                throw new BusinessLogicException(TypeBusinessException.CARD,
                "No money", $" Недостаточно средств на карте {from.CardNumber }");

            CardFromNumber = from.CardNumber;
            CardToNumber = to.CardNumber;
            FromSum = sum;
            ToSum = blService.GetConvertSum(sum, from.Currency, to.Currency);
        }

        /// <summary>
        /// Create new Transactions
        /// </summary>
        /// <param name="sum">Sum of the transaction</param>
        /// <param name="to">Link card to</param>
        public Transaction(decimal sum, Card to)
        {
            if (to == null)
                throw new BusinessLogicException(TypeBusinessException.TRANSACTION, "To card is null",
                "Не найдена карта на которую совершается перевод");

            if (sum <= 0)
                throw new UserDataException("Sum of the transaction need more then 0", $"Add to {to.CardName}");

            if (!blService.CheckCardActivity(to))
                throw new BusinessLogicException(TypeBusinessException.CARD,
                "Card is expired", $"Карта {to.CardNumber } просрочена");

            CardToNumber = to.CardNumber;
            ToSum = blService.GetConvertSum(sum, Currency.RUR, to.Currency);
        }
    }

    /// <summary>
    /// Class contains neccessary data for transaction creation
    /// </summary>
    public class NewTransaction {
        public NewTransaction(decimal sum, string from, string to) {
            From = from;
            To = to;
            Sum = sum;
        }
        public string From { get; }
        public string To { get; }
        public decimal Sum { get; }
    }
}