using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Server.Exceptions;
using Server.Models;
using Server.Services;
using Server.Infrastructure;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    public class CardsController : Controller
    {
        private readonly IBankRepository _repository;

        private readonly ICardService _cardService;

        public CardsController(IBankRepository repository, ICardService cardService)
        {
            _repository = repository;
            _cardService = cardService;
        }

        // GET api/cards
        [HttpGet]
        public IEnumerable<Card> Get() => _repository.GetCards();

        // GET api/cards/5334343434343...
        [HttpGet("{number}")]
        public Card Get(string number)
        {
            if (!_cardService.CheckCardNumber(number))
                throw new UserDataException("Incorrect cardNumber", number);
            if (!_cardService.CheckCardEmmiter(number))
                throw new UserDataException("Card emmiter is invalid", number);

            var card = _repository.GetCard(number);
            if  (card == null)
                throw new BusinessLogicException(TypeBusinessException.CARD, "Card is null", "Карта не найдена");
                
            return _repository.GetCard(number);
        }

        // POST api/cards
        [HttpPost]
        public Card Post([FromBody]NewCard card) {
            if(!Enum.IsDefined(typeof(CardType), card.CardType))
                throw new UserDataException("Wrong type card", card.CardType);

            if(!Enum.IsDefined(typeof(Currency), card.Currency))
                throw new UserDataException("Incorrect currency", card.Currency);
            
            var cardType = (CardType) Enum.Parse(typeof(CardType), card.CardType);
            var currency = (Currency) Enum.Parse(typeof(Currency), card.Currency);
            var user = _repository.GetCurrentUser();
            return user.OpenNewCard(card.CardName, currency, cardType);
        }

        // DELETE api/cards/5
        [HttpDelete("{number}")]
        public IActionResult Delete(string number) => StatusCode(405);

        //TODO PUT method
    }
}
