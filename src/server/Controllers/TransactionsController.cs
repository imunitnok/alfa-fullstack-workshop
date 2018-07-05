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
    public class TransactionsController : Controller
    {
        private readonly IBankRepository _repository;

        private readonly ICardService _cardService;

        public TransactionsController(IBankRepository repository, ICardService cardService)
        {
            _repository = repository;
            _cardService = cardService;
        }

        // GET api/transactions?card=2352334...&from=12&to=24
        [HttpGet]
        public IEnumerable<Transaction> Get([FromQuery]string card, [FromQuery]int from = 0, [FromQuery]int to = int.MaxValue - 1) {
            if (!_cardService.CheckCardNumber(card))
                throw new UserDataException("Incorrect cardNumber", card);
            if (!_cardService.CheckCardEmmiter(card))
                throw new UserDataException("Card emmiter is invalid", card);

            return _repository.GetTransactions(card, from, to);
        }

        // POST api/cards
        [HttpPost]
        public Transaction Post([FromBody]NewTransaction transaction) {
            if (!_cardService.CheckCardNumber(transaction.From))
                throw new UserDataException("Incorrect cardNumber", transaction.From);
            if (!_cardService.CheckCardEmmiter(transaction.From))
                throw new UserDataException("Card emmiter is invalid", transaction.From);
            
            if (!_cardService.CheckCardNumber(transaction.To))
                throw new UserDataException("Incorrect cardNumber", transaction.To);
            if (!_cardService.CheckCardEmmiter(transaction.To))
                throw new UserDataException("Card emmiter is invalid", transaction.To);
            
            return _repository.TransferMoney(transaction.Sum, transaction.From, transaction.To);
        }

    }
}