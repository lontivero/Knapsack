using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Transactions;

namespace ConsoleApp14
{
    static class Program
    {
        static void Main(string[] args)
        {
            var txCount = int.Parse(args[0]);

            Console.WriteLine("==============================================================================");
            Console.WriteLine("== Transactions to mix  ======================================================");
            Console.WriteLine("==============================================================================");

            var txs = Utility.RandomTransactions().Take(txCount).OrderByDescending(x=>x.Outputs.Sum()).ToList();

            foreach (var tx in txs) 
            {
                Console.WriteLine(tx);
            }
            Console.WriteLine();


            Console.WriteLine("==============================================================================");
            Console.WriteLine("== Output mixed transaction  =================================================");
            Console.WriteLine("==============================================================================");
            var cjTx = txs.Aggregate((a,b)=> MixTransactions1(a, b));
            Console.WriteLine(cjTx);
            
            Console.WriteLine("==============================================================================");
            Console.WriteLine("== Analyzing mixed transaction  ==============================================");
            Console.WriteLine("==============================================================================");

            var sw = new Stopwatch();
            sw.Start();
            Analyse(cjTx);
            sw.Stop();
            Console.WriteLine($"\tAnalysis completed after {sw.Elapsed}");
        }

        public static Transaction MixTransactions1(Transaction a, Transaction b) 
        {
            var newInputs = new List<decimal>(a.Inputs.Concat(b.Inputs));
            if (a.Inputs.Sum() == b.Inputs.Sum()) 
            {
                return new Transaction 
                { 
                    Inputs=newInputs,
                    Outputs = a.Outputs.Concat(b.Outputs).ToList()
                };
            }

            IEnumerable<decimal> newOutputs = null;
            if (a.Outputs.Sum() > b.Outputs.Sum()) 
            {
                var diff = a.Outputs.Sum() - b.Outputs.Sum();
                var subsumOuts = RealizeSubsum(a.Outputs, diff).ToList();
                newOutputs = subsumOuts.Concat(b.Outputs);
            }
            else if (a.Outputs.Sum() < b.Outputs.Sum()) 
            {
                var diff = b.Outputs.Sum() - a.Outputs.Sum();
                var subsumOuts = RealizeSubsum(b.Outputs, diff).ToList();
                newOutputs = subsumOuts.Concat(a.Outputs);
            }
            return new Transaction
            {
                Inputs = newInputs.ToList(),
                Outputs = newOutputs.ToList()
            };
        }

        public static IEnumerable<decimal> RealizeSubsum(IEnumerable<decimal> coins, decimal subsum) 
        {
            foreach (var coin in coins.OrderByDescending(x=>x)) 
            {
                if (subsum == 0)
                {
                    yield return coin;
                } 
                else if (coin <= subsum)
                {
                    yield return coin;
                    subsum -= coin;
                }
                else if (coin > subsum) 
                {
                    yield return subsum;
                    yield return coin - subsum;
                    subsum = 0;
                }
            }
        }

        public static void Analyse(Transaction tx)
        {
            var inpComb = tx.Inputs .CombinationsWithoutRepetition(1, 7);
            var outComb = tx.Outputs.CombinationsWithoutRepetition(1, 2);

            var i = 0UL;
            var n = 0;
            foreach(var outs in outComb)
            {
                foreach(var inps in inpComb)
                {
                    if(i % 1237 == 0 && !Console.IsOutputRedirected)
                    {
                        Console.SetCursorPosition(0,Console.CursorTop);
                        Console.Write($"Analizing {i} combinations");
                    }
                    i++;
                    if (inps.Sum() == outs.Sum())
                    {
                        Console.WriteLine($"\t# {n++}\t Tx found!!!");
                        Console.WriteLine(new Transaction{
                            Inputs = inps.ToList(),
                            Outputs= outs.ToList()
                        });
                    }
                }
            }
        }
    }

    public static class Utility 
    {
        private static Random rng = new Random();

        public static IEnumerable<decimal> RandomAmount(decimal min, decimal max) 
        {
            while (true)
                yield return min + Math.Round((decimal)(rng.NextDouble() * (double)(max - min)), 4);
        }

        const decimal MIN_COIN_VALUE = 0.00001m;

        public static IEnumerable<Transaction> RandomTransactions() 
        {
            while (true) 
            {
                var inputCount = rng.Next(1, 7);
                var coinList = RandomAmount(MIN_COIN_VALUE, 2m).Take(inputCount).OrderByDescending(x=>x).ToList();
                var smallestCoin = coinList.Min();
                var change = RandomAmount(MIN_COIN_VALUE, smallestCoin).Take(1).Single();
                var amountToSend = coinList.Sum() - change; 

                yield return new Transaction 
                {
                    Inputs = coinList,
                    Outputs = new List<decimal>(new []{amountToSend, change})
                };
            }
        }
    }
    public class Transaction
    {
        public List<decimal> Inputs;
        public List<decimal> Outputs;

        public override string ToString()
        {
            var sb = new StringBuilder();
            var rowCount = Math.Max(Inputs.Count, Outputs.Count);
            for(var i= 0; i < rowCount; i++ ){
                sb.Append(i < Inputs.Count ? $"\t[In]: {Inputs[i]}" : "\t\t");
                if(i < Outputs.Count)
                {
                    sb.Append($"\t\t[Out]: {Outputs[i]}");
                }
                sb.AppendLine();
            }
            sb.AppendLine($"\t##### {Inputs.Sum()}\t\t###### {Outputs.Sum()}" );
            sb.AppendLine("\t" + new String('-', 70));

            return sb.ToString();
        }
    }


    static class LinqExtensions
    {
        public static IEnumerable<IEnumerable<T>> CombinationsWithoutRepetition<T>(
            this IEnumerable<T> items,
            int ofLength)
        {
            return (ofLength == 1) ?
                items.Select(item => new[] { item }) :
                items.SelectMany((item, i) => items.Skip(i + 1)
                    .CombinationsWithoutRepetition(ofLength - 1)
                    .Select(result => new T[] { item }.Concat(result)));
        }

        public static IEnumerable<IEnumerable<T>> CombinationsWithoutRepetition<T>(
            this IEnumerable<T> items,
            int ofLength,
            int upToLength)
        {
            return Enumerable.Range(1, Math.Max(0, upToLength))
                            .SelectMany(len => items.CombinationsWithoutRepetition(ofLength: len));
        }
    }
}
