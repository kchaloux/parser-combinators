using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace ParserCombinators
{
    internal class Program
    {
        #region Syntax Classes
        
        interface ILiteral
        {
            object Value { get; }
        }

        interface IExpression
        {
        }

        class ArgumentsList
        {
            public IReadOnlyList<IExpression> Arguments { get { return _arguments; } }
            private readonly IReadOnlyList<IExpression> _arguments;

            public ArgumentsList(IEnumerable<IExpression> arguments)
            {
                _arguments = new List<IExpression>(arguments);
            }

            public override string ToString()
            {
                return string.Concat("Args(", string.Join(", ", _arguments), ")");
            }
        }

        class Function : IExpression
        {
            public Identifier Identifier { get { return _identifier; } }
            private readonly Identifier _identifier;

            public ArgumentsList Arguments { get { return _arguments; } }
            private readonly ArgumentsList _arguments;

            public Function(Identifier identifier, ArgumentsList arguments)
            {
                _identifier = identifier;
                _arguments = arguments;
            }

            public override string ToString()
            {
                return string.Concat("Function(", Identifier, ", ", Arguments, ")");
            }
        }

        class Identifier : IExpression
        {
            public string Name { get { return _name; } }
            private readonly string _name;

            public Identifier(string name)
            {
                _name = name;
            }

            public override string ToString()
            {
                return string.Concat("Identifier(", Name, ")");
            }
        }

        class LiteralString : IExpression, ILiteral
        {
            public object Value { get { return _value; } }
            private readonly string _value;

            public LiteralString(string value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return string.Concat("String(\"", Value, "\")");
            }
        }

        class LiteralInteger : IExpression, ILiteral
        {
            public object Value { get { return _value; } }
            private readonly int _value;

            public LiteralInteger(int value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return string.Concat("Integer(", Value, ")");
            }
        }

        class LiteralDouble : IExpression, ILiteral
        {
            public object Value { get { return _value; } }
            private readonly double _value;

            public LiteralDouble(double value)
            {
                _value = value;
            }

            public override string ToString()
            {
                return string.Concat("Double(", Value, ")");
            }
        }
        #endregion

        static Parser<Identifier> IdentifierParser { get; set; }
        static Parser<ILiteral> LiteralParser { get; set; }
        static Parser<IExpression> ExpressionParser { get; set; }
        static Parser<Function> FunctionParser { get; set; }
        static Parser<ArgumentsList> ArgumentsListParser { get; set; }

        private static void Main(string[] args)
        {
            Parser<LiteralString> literalStringParser = 
                new RegexParser("\"([^\"]+)\"")
                    .As(x => new LiteralString(x.Substring(1, x.Length - 2)));

            Parser<LiteralInteger> literalIntegerParser =
                new RegexParser(@"\d+")
                    .As(x => new LiteralInteger(int.Parse(x)));

            Parser<LiteralDouble> literalDoubleParser =
                new RegexParser(@"-?(((\.\d+|\d+\.\d*)([eE]-?\d+)?)|Infinity|NaN)")
                    .As(x => new LiteralDouble(double.Parse(x)));

            IdentifierParser =
                new RegexParser(@"[a-zA-Z_][a-zA-Z0-9_]*")
                    .As(x => new Identifier(x));

            LiteralParser =
                literalStringParser.As(x => (ILiteral)x)
                    .Or(literalDoubleParser.As(x => (ILiteral)x))
                    .Or(literalIntegerParser.As(x => (ILiteral)x));

            var openParenParser = new RegexParser(@"\(\s*");
            var closeParenParser = new RegexParser(@"\s*\)");

            var nullaryFunctionParser =
                openParenParser
                    .Then(IdentifierParser)
                    .Then(closeParenParser)
                    .As((a1, ident, a2) => new Function(ident, new ArgumentsList(new IExpression[0])));

            var polyaryFunctionParser =
                Lazy.Parser(() =>
                    openParenParser.Then(IdentifierParser)
                        .ThenPattern(@"\s*")
                        .Then(ArgumentsListParser)
                        .Then(closeParenParser)
                        .As((a1, ident, a2, arg, a3) => new Function(ident, arg)));

            FunctionParser = nullaryFunctionParser.Or(polyaryFunctionParser);
            
            ExpressionParser =
                new LazyParser<IExpression>(() => 
                    LiteralParser.As(x => (IExpression)x)
                        .Or(IdentifierParser.As(x => (IExpression)x))
                        .Or(FunctionParser.As(x => (IExpression)x)));

            ArgumentsListParser =
                new LazyParser<ArgumentsList>(() =>
                    ExpressionParser.Repeat1().WithSepPattern(@"\s*")
                        .As(xs => new ArgumentsList(xs)));

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == null || input.ToLowerInvariant() == "exit")
                {
                    break;
                }

                var parsedData = FunctionParser.Parse(input);

                Console.WriteLine("Parsed As: ");
                Console.WriteLine("{0}", parsedData);
            }
            Console.ReadLine();
        }
    }
}
