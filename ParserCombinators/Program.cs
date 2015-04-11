using System;
using System.Collections.Generic;
using System.Linq;

namespace ParserCombinators
{
    internal class Program
    {
        struct Fish
        {
            public string Species { get { return _species; } }
            private readonly string _species;

            public string Body { get { return _body; } }
            private readonly string _body;

            public Fish(string species, string body)
            {
                _species = species;
                _body = body;
            }

            public override string ToString()
            {
                return string.Concat(Species, "[ \"", Body, "\" ]");
            }
        }

        private static void Main(string[] args)
        {  
            var normalFish = new LiteralParser("><>").Or(new LiteralParser("<><")).As(x => new Fish("Normal", x));
            var sturdyFish = new LiteralParser("><>>").Or(new LiteralParser("<<><")).As(x => new Fish("Sturdy", x));
            var speedyFish = new LiteralParser(">><>").Or(new LiteralParser("<><<")).As(x => new Fish("Speedy", x));
            var stretchyFish = new LiteralParser("><>>>").Or(new LiteralParser("<<<><")).As(x => new Fish("Stretchy", x));
            var crab = new LiteralParser(",<..>,").As(x => new Fish("Crab", x));

            var parser = crab.Or(stretchyFish).Or(speedyFish).Or(sturdyFish).Or(normalFish).Repeat1().End();

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == null || input.ToLowerInvariant() == "exit")
                {
                    break;
                }

                var result = parser.Parse(input);
                Console.WriteLine(result);
                if (result.Success)
                {
                    Console.WriteLine(string.Join(Environment.NewLine, result.Value));
                }
            }
            Console.ReadLine();
        }
    }
}
