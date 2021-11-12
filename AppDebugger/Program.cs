using System;
using System.Linq;
using System.Collections.Generic;
using pheval;

namespace AppDebugger
{
    class Program
    {

        static Card[] CreateDeck()
        {
            int rankAmount = Card.RanksArray.Length;
            int suitAmount = Card.SuitsArray.Length;
            int deckSize = rankAmount * suitAmount;

            Card[] deck = new Card[deckSize];

            for (int i = 0; i < deckSize; i++)
            {
                deck[i] = new Card((byte)i);
            }
            return deck;
        }

        static Stack<Card> CreateDealerDeck(IEnumerable<Card> deck)
        {

            List<Card> ogDeck = deck.ToList();
            /*
            //List<string> forceTypes = ForcedDeck.TwoPairs;
            //List<string> forceTypes = ForcedDeck.HighFlush;
            //List<string> forceTypes = ForcedDeck.AceKickerTwoPair; 
            List<string> forceTypes = ForcedDeck.AceTieTwoPair; 

            //Console.WriteLine(string.Join(',', forceTypes.Select(x => $"\"{x}\"").ToArray()));

            for (int i = 0; i < forceTypes.Count; i++)
            {
                string fct = forceTypes[i];
                Card card = ogDeck.Find(x => x.rank == fct[0] && x.suit == fct[1]);
                int index = ogDeck.FindIndex(x => x.rank == card.rank && x.suit == card.suit);

                ogDeck.RemoveAt(index);
                ogDeck.Insert(i, card);
            }
            ogDeck.Reverse();*/


            Stack<Card> dealerDeck = new Stack<Card>(ogDeck);

            return dealerDeck;
        }

        static void Main(string[] args)
        {
            string input = string.Empty;
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Hello Input {input}!");




                Card[] deck = CreateDeck();

                Console.WriteLine();
                Console.Write(string.Join(',', deck.Select(x => $"{x.rank}{x.suit}")));
                Console.WriteLine();

                deck.Shuffle();

                Console.WriteLine();
                Console.Write(string.Join(',', deck.Select(x => $"{x.rank}{x.suit}")));
                Console.WriteLine();
                Console.WriteLine();

                Stack<Card> dealerDeck = CreateDealerDeck(deck);

                int playerAmount = 4;
                int cardAmount = 2;

                Dictionary<int, string> playerHand = new Dictionary<int, string>(playerAmount);

                for (int i = 0; i < cardAmount; i++)
                {
                    for (int j = 0; j < playerAmount; j++)
                    {
                        Card card = dealerDeck.Pop();
                        if (playerHand.ContainsKey(j) == false) playerHand.Add(j, string.Empty);

                        playerHand[j] += card.ToString();
                    }
                }

                dealerDeck.Pop(); // burn

                string communityHand = string.Empty;
                int flopAmount = 3;
                for (int i = 0; i < flopAmount; i++)
                {
                    Card card = dealerDeck.Pop();
                    communityHand += card.ToString();
                }

                dealerDeck.Pop(); // burn

                int turnAmount = 1;
                for (int i = 0; i < turnAmount; i++)
                {
                    Card card = dealerDeck.Pop();
                    communityHand += card.ToString();
                }

                dealerDeck.Pop(); // burn

                int riverAmount = 1;
                for (int i = 0; i < riverAmount; i++)
                {
                    Card card = dealerDeck.Pop();
                    communityHand += card.ToString();
                }

                Dictionary<int, int> bestHandRank = new Dictionary<int, int>(playerAmount);
                for (int i = 0; i < playerAmount; i++)
                {
                    string hand = playerHand[i] + communityHand;
                    int rank = Eval.Eval7String(hand);
                    string handStr = string.Empty;
                    int amount = 0;
                    while (amount < hand.Length)
                    {
                        string val = hand.Substring(amount, 2);
                        handStr += val + " ";
                        amount += 2;
                    }

                    Console.WriteLine($"P{i} {hand} {handStr} {Rank.GetCategory(rank)} {Rank.DescribeRank(rank)}");

                    bestHandRank.Add(i, rank);
                }
                Console.WriteLine();

                IOrderedEnumerable<KeyValuePair<int, int>> winHands = bestHandRank.OrderBy(x => x.Value);

                foreach (var item in winHands)
                {
                    Console.WriteLine($"{item.Key} {item.Value} {Rank.GetCategory(item.Value)} {Rank.DescribeRank(item.Value)} - {Rank.DescribeRankShort(item.Value)}");
                }
                Console.WriteLine();

                IEnumerable<(int rank, IEnumerable<KeyValuePair<int, int>> sets)> groups = winHands.GroupBy(x => x.Value, (rank, sets) => (rank, sets));

                Console.WriteLine($"Count: {groups.Count()}");
                foreach (var item in groups.Take(1))
                {
                    foreach (var set in item.sets)
                    {
                        Console.WriteLine($"{set.Key} {set.Value} {Rank.GetCategory(set.Value)} {Rank.DescribeRank(set.Value)} - {Rank.DescribeRankShort(set.Value)}");
                    }
                }


                input = Console.ReadLine();
            }

            /*int rankP1 = Eval.Eval7String("acqdqcjctc3h8s");
            int rankP2 = Eval.Eval7String("7s5s4s3s2d3h8h");
            Console.WriteLine("P1 " + Rank.GetCategory(rankP1));
            Console.WriteLine("P2 " + Rank.GetCategory(rankP2));
            Console.WriteLine($"P1 {rankP1} {Rank.DescribeRank(rankP1)}");
            Console.WriteLine($"P2 {rankP2} {Rank.DescribeRank(rankP2)}");
            if (rankP1 > rankP2) Console.WriteLine("Player 2 wins");
            if (rankP2 > rankP1) Console.WriteLine("Player 1 wins");*/
        }

        public static class ForcedDeck
        {
            public static List<string> TwoPairs = new List<string>()
            {
                "3s", "4c", "2h", "As", // first deal round
                "7s", "Kd", "Ts", "9d", // second deal round
                "3h",                   // pre flop burn
                "5h", "5d", "9h",       // flop
                "Kc",                   // pre turn burn
                "Jd",                   // turn
                "Tc",                   // pre river burn
                "Ad",                   // river
            };

            public static List<string> HighFlush = new List<string>()
            {
                "Jd", "As", "Kd", "3c", // first deal round
                "6d", "Td", "Ts", "8s", // second deal round
                "9c",                   // pre flop burn
                "3d", "Ad", "5d",       // flop
                "6c",                   // pre turn burn
                "Ah",                   // turn
                "7d",                   // pre river burn
                "Qd",                   // river
            };
            public static List<string> AceKickerTwoPair = new List<string>()
            {
                "6d","2d","2h","Ah",    // first deal round
                "Ad","8c","6h","5c",    // second deal round
                "Js",                   // pre flop burn
                "3h","Kh","Kc",         // flop
                "Qd",                   // pre turn burn
                "6c",                   // turn
                "5h",                   // pre river burn
                "2c"                    // river
            };
            public static List<string> AceTieTwoPair = new List<string>()
            {
                "6d","2d","Ac","Ah",    // first deal round
                "Ad","8c","6h","5c",    // second deal round
                "Js",                   // pre flop burn
                "3h","Kh","Kc",         // flop
                "Qd",                   // pre turn burn
                "6c",                   // turn
                "5h",                   // pre river burn
                "2c"                    // river
            };
        }
    }

    public static class Extensions
    {
        /*
         * https://stackoverflow.com/questions/273313/randomize-a-listt
         */

        static Random s_Rnd = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = s_Rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
