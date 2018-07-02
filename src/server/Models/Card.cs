using Server.Services;

namespace Server.Models
{
    /// <summary>
    /// Card domain model
    /// </summary>
    public class Card
    {
        private string _cardNumber;

        private readonly ICardService cardService = new CardService();

        public Card(string cardNumber, string cardName)
        {
            //TODO validation
            CardNumber = cardNumber;
            CardName = cardName;
        }

        /// <summary>
        /// Card number. Set only from constructor.
        /// </summary>
        /// <returns>string card number representation</returns>
        public string CardNumber { get => _cardNumber; private set => _cardNumber = value; }

        /// <summary>
        /// Short name of the cards
        /// </summary>
        /// <returns></returns>
        public string CardName { get; set; }

        // TODO add fields
    }
}