using System.Collections.Generic;
using System.Linq;

namespace pheval
{
    [System.Serializable]
    public class Card
    {
        static Dictionary<char, byte> rankMap = new Dictionary<char, byte>()
        {
            { '2', 0}, { '3', 1}, { '4', 2}, { '5', 3},
            { '6', 4}, { '7', 5}, { '8', 6}, { '9', 7},
            { 'T', 8}, { 'J', 9}, { 'Q', 10}, { 'K', 11}, { 'A', 12},
            { 't', 8}, { 'j', 9}, { 'q', 10}, { 'k', 11}, { 'a', 12},
        };

        static Dictionary<char, byte> suitMap = new Dictionary<char, byte>()
        {
            { 'C', 0}, { 'D', 1}, { 'H', 2}, { 'S', 3},
            { 'c', 0}, { 'd', 1}, { 'h', 2}, { 's', 3},
        };
        static char[] rankReverseArray = {
          '2', '3', '4', '5',
          '6', '7', '8', '9',
          'T', 'J', 'Q', 'K', 'A',
        };
        static char[] suitReverseArray = { 'c', 'd', 'h', 's' };

        public static char[] SuitsArray { get => suitReverseArray; }
        public static char[] RanksArray { get => rankReverseArray; }

        public byte id;

        public char rank;
        public char suit;

        public Card(string name)
        {
            rank = name[0];
            suit = name[1];
            id = (byte)(rankMap[rank] * 4 + suitMap[suit]);
        }

        public Card(byte id_)
        {
            id = id_;
            rank = describeRank();
            suit = describeSuit();
        }

        public static Card[] Cards(string hand)
        {
            Card[] result = new Card[hand.Length / 2];
            var i = 0;
            while (i + 1 < hand.Length)
            {
                result[i / 2] = new Card(hand.Substring(i, 2));
                i += 2;
            }
            return result;
        }

        public static Card[] Cards(byte[] ids)
        {
            return ids.Select(id => new Card(id)).ToArray();
        }

        char describeRank() { return rankReverseArray[id / 4]; }
        char describeSuit() { return suitReverseArray[id % 4]; }

        public override string ToString()
        {
            char[] chars = { describeRank(), describeSuit() };
            return new string(chars);
        }

        public static string CardsToString(Card[] cards)
        {
            var result = "";
            foreach (Card card in cards)
            {
                result += card.ToString();
            }
            return result;
        }
    }
}
