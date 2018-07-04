using System.Collections.Generic;
using Server.Infrastructure;
using Server.Models;

namespace Server.Data
{
    public static class FakeDataGenerator
    {
        public static User GenerateFakeUser() => new User("admin@admin.net");

        public static IEnumerable<Card> GenerateFakeCardsToUser(User user)
        {
            // create fake cards
            user.OpenNewCard("my salary", Currency.RUR, CardType.MAESTRO);
            user.OpenNewCard("my debt", Currency.EUR, CardType.VISA);
            user.OpenNewCard("to my lovely cat", Currency.USD, CardType.MASTERCARD);

            return user.Cards;
        }
    }
}