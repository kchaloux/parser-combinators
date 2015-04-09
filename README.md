# C# Parser Combinators
This is a curiosity-project to see if I could implement a halfway decent Parser Combinators library in C#, based on the one that ships with Scala (in *scala.util.parsing.combinator*). It's wildly incomplete, has terrible error reporting, and no unit tests, but it **does** actually provide support for creating recursive descent parsers from basic, smaller parsers.

Currently, it's going up on GitHub so I don't lose what I already have. Hopefully I add tests, get error reporting working better, and smooth out some of the general awfulness in the future.

You can use it, but I provide **absolutely no guarantees of quality** in its current state!

## The Inspiration & Concept
Implementing a recursive descent parser can be a pain. Regular Expressions are great, but they're just not up to the task. Furthermore, text parsed  out with regexes only stores matches as general groups and text. I wanted a way to define small, simple parsers that could parse text *into* the meaningful components of an Abstract Syntax Tree, and then chain those parsers together in such a way that they can handle larger, more complex grammars.

There are only a few basic types of Parser in this library, but they can be combined in powerful ways.

#### Literal Parser
This parser matches a simple *literal* string within another string.
```cs
var parser = new LiteralParser("some text");
var result1 = parser.Parse("some text"); // Returns a Successful string match
var result2 = parser.Parse("diff text"); // Returns a Failed string match
```

#### Regex Parser
Even though regular expressions can't handle the really complex stuff, they're still great for the smaller, regular parts of the grammar. This parser matches a pattern within a string.
```cs
var parser = new RegexParser(@"\d+\.\d+");
var result1 = parser.Parse("123.456"); // Returns a Successful string match of "123.456"
var result2 = parser.Parse("123"); // Returns a Failed string match
```

#### Conversion Parser
While the `LiteralParser` and `RegexParser`  match strings *by default*, they can turn their matches into something more meaningful with a `ConversionParser`.  This parser attempts to transform the contents matched by a single parser into a new form. It is typically constructed by using the `As` function.
```cs
var parser = new RegexParser(@"\d+").As(x => int.Parse(x)); // Parses a string and converts to an int
var result1 = parser.Parse("100"); // Returns a Success int match of 100
```

#### Sequence Parser
If you want to match two parsers in a sequence, you can chain them together with the `Then` function to create a new `SequenceParser`.
```cs
var wordParser = new RegexParser(@"\w+");
var numberParser = new RegexParser(@"\d+").As(int.Parse);
var seqParser =
    wordParser.Then(numberParser)
        .As((w, n) => string.Format("Word:{0}  Number:{1}", w, n));

var result1 = seqParser.Parse("cat100"); // Parses "Word:Cat  Number:100"
var result2 = seqParser.Parse("cat"); // Fails - expecting numberParser at index 3
```
Notice that the `As` function accepts a lambda with **2** arguments instead of one in this case. With sequence parsers, you can provide a delegate that captures the result of every parser in the chain. Unfortunately, this only works for up to 16 parsers chained directly together, as that is the maximum size of a `Func` in the current version of C#.

#### Or Parser
You can combine multiple parsers together by making them alternatives to one another. An Or-Parser will try to match each parser it has in sequence, stopping on the first one that succeeds. If none of the parsers match successfully, it will fail.
```cs
var catParser = new LiteralParser("cat");
var dogParser = new LiteralParser("dog");
var foxParser = new LiteralParser("fox");
var animalParser = catParser.Or(dogParser).Or(foxParser);

var result1 = animalParser.Parse("cat"); // Succeeds with "cat"
var result2 = animalParser.Parse("dog"); // Succeeds with "dog"
var result3 = animalParser.Parse("fox"); // Succeeds with "fox"
var result4 = animalParser.Parse("ferret"); // Fails to match any of the parsers
```
While a trivial example like this can be achieved with a simple Regex, remember that parsers of *any* complexity can be sequenced or or-ed with each other.

#### Repeat Parser
You can make a single parser repeat itself with the `Repeat` function.
```cs
var numCharParser = new RegexParser(@"\d[a-zA-Z]");

// Match the numCharParser zero or more times.
var repeatZeroOrMore = numCharParser.Repeat();
var result1 = repeatZeroOrMore.Parse("_______"); // Succeeds with ""
var result3 = repeatZeroOrMore.Parse("1a2b3c"); // Succeeds with "1a2b3d"

// Match the numCharParser one or more times.
var repeatOnceOrMore = numCharParser.Repeat1();
var result4 = repeatOnceOrMore.Parse("______"); // Fails to parse at least once.
var result5 = repeatOnceOrMore.Parse("1a2b__"); // Succeeds with "1a2b"
```
Repeating parsers can also be provided with a separator parser. This parser will be matched, but thrown away after the parsing is complete.
```cs
var numberParser = new RegexParser(@"\d+").As(int.Parse);
var commaParser = new RegexParser(@"\s*,\s*");
var numberListParser = numberParser.Repeat().WithSep(commaParser);

var result1 = numberListParser.Parse("1,2,3,4,5"); // Parses a list of ints [1, 2, 3, 4, 5];
var result2 = numberListParser.Parse("1  , 340  , 3,4"); // Parses a list of ints [1, 340, 3, 4]
var result3 = numberListParser.Parse(""); // Parses an empty list of ints
```

### Handling Recursive Descent
The problem with defining recursively descending parsers is that they need a reference to themselves, at some point or other, before they can be fully built. Since these parsers are objects, which are instantiated in some order, this frequently results in what appears to be a circular dependency - Parser 1 relies on Parser 2, which relies on Parser 1. This is solved by declaring the Parsers up front, and making them into `LazyParser`s. 

```cs
var wordParser = new RegexParser(@"\w+");

// Declare the mutually recursive parsers before defining them
// so that the compiler knows that they exist, even if they're 
// not yet instantiated.
Parser<string> groupParser = null;
Parser<string> groupElementParser = null; 

// Create a lazy parser that refers to 'groupElementParser'
groupParser = new LazyParser<string>(() =>
    new LiteralParser("(")
        .Then(groupElementParser.Repeat1().WithSep(new LiteralParser(" ")))
        .Then(new LiteralParser(")"))
        .As((open, elems, close) => string.Concat(open, string.Join(" ", elems), close)));

// Create a lazy parser that refers back to 'groupParser'
groupElementParser =
    new LazyParser<string>(() =>
        wordParser.Or(groupParser));

var result1 = groupParser.Parse("(one (two) (three four) five)"); // Succeeds
var result2 = groupParser.Parse("(one (two (three (((four))))))"); // Succeeds
var result3 = groupParser.Parse("(one (two three)"); // Fails - Expected ")" at index 16
```
With this, we've define a very simple recursive descent parser that ensures that it only matches groups with evenly matched parentheses. While this particular example only works on text, by using `ConversionParser`s, and defining classes to represent the distinct elements of our grammar, we can get it to parse out a proper Abstract Syntax Tree instead. While some flavors of Regex can enforce recursive matching of text, Regexes are incapable of transforming that text into meaningful data while it does the parsing.

### Notes
This is still heavily experimental, and a work in progress. As previously mentioned, there are **no** proper unit tests yet, and the error reporting isn't very useful. Nevertheless, the examples above all work, and, while the syntax isn't nearly as pretty as it is in Scala, it makes it much easier to define a quick recursive parser than trying to roll one by hand.
