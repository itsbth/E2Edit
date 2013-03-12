using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace E2ParserTest
{
    static class Program
    {
        static void Main(string[] args)
        {
            //TestTokenizer();
            TestGrammar();
        }

        private static void TestTokenizer()
        {
            var reader = new E2Tokenizer(File.ReadAllText(@"C:\Games\Steam\steamapps\itsbth\garrysmod\garrysmod\data\Expression2\datareceiver.txt"));
            Token t;
            int ind = 0;
            bool c = true;
            while ((t = reader.Read()).Type != Token.TokenType.EOF)
            {
                switch(t.Type)
                {
                    case Token.TokenType.String:
                        Console.ForegroundColor = ConsoleColor.Green;
                        break;
                    case Token.TokenType.Comment:
                        Console.ForegroundColor = ConsoleColor.Gray;
                        break;
                    case Token.TokenType.Symbol:
                        Console.ForegroundColor = ConsoleColor.Blue;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
                if (t.Type == Token.TokenType.Symbol && t.Value != ",") Console.Write(" ");
                Console.Write(t.Value);
                if (t.Type == Token.TokenType.Symbol) Console.Write(" ");
                if (t.Type == Token.TokenType.NewLine) Console.Write(new string('\t', ind));
                if (t.Type == Token.TokenType.BracketStart) ind += 1;
                else if (t.Type == Token.TokenType.BracketEnd) ind -= 1;
                c = !c;
            }
            Console.Read();
        }

        private static void TestGrammar()
        {
            var grammar = new E2Grammar();
            var compiler = new Parser(grammar);
            var tree = compiler.Parse(
                File.ReadAllText(
                    @"C:\Games\Steam\steamapps\itsbth\garrysmod\garrysmod\data\Expression2\datareceiver.txt"));
            Console.WriteLine(tree.ParserMessages.Aggregate("", (s, m) => s + m.Location + ": " + m.Message + " - " + m.Level + "\n"));
            Console.Read();
        }
    }
}
