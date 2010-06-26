using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace E2Edit.Editor
{
    internal class E2Parser
    {
        private static readonly char[] Whitespace = new[] {' ', '\t'};
        private readonly IList<string> _lines;
        private readonly string _text;
        private int _line;
        private int _pos;

        public E2Parser(string text)
        {
            _text = text;
            _lines = text.Split(new[] {'\r', '\n'});
            Variables = new List<Variable>();
        }

        public IList<Variable> Variables { get; private set; }

        public void Parse()
        {
            ParseDirectives();
        }

        private void ParseDirectives()
        {
            while (ParseDirective()) _line += 1;
        }

        private bool ParseDirective()
        {
            if (_lines[_line][0] != '@') return false;
            _pos += 1;
            switch (ReadWord())
            {
                case "name":
                    break;
                case "model":
                    break;
                case "inputs":
                    ParseVariables(Variable.Source.Input);
                    break;
            }
            return true;
        }

        private void ParseVariables(Variable.Source source)
        {
            Consume(Whitespace);
            if (_text[_pos] == '[')
            {
                _pos += 1;
                var variables = new List<string>();
                while (_pos < _lines[_line].Length && _text[_pos] != ']')
                {
                    Consume(Whitespace);
                    variables.Add(ReadWord());
                }
            }
        }

        private void Consume(IEnumerable<char> c)
        {
            while (_pos < _lines[_line].Length && c.Contains(_lines[_line][_pos]))
                _pos += 1;
        }

        private string ReadWord()
        {
            var buff = new StringBuilder();
            while (_pos < _lines[_line].Length && Char.IsLetter(_lines[_line][_pos]))
            {
                buff.Append(_lines[_line][_pos]);
            }
            return buff.ToString();
        }
    }

    internal class TokenReader
    {
        private readonly TextReader _reader;
        private int _offset;

        public TokenReader(TextReader reader)
        {
            _reader = reader;
        }

        public bool IgnoreWhitespace { get; set; }

        public Token Read()
        {
            var buff = new StringBuilder();
            char s;
            if (IgnoreWhitespace)
                do
                {
                    s = (char) _reader.Read();
                    _offset += 1;
                } while (Char.IsWhiteSpace(s));
            int start = _offset;
            do
            {
                s = (char) _reader.Read();
                buff.Append(s);
                _offset += 1;
            } while (Char.IsLetterOrDigit(s));
            return new Token {Value = buff.ToString(), Offset = start, Length = _offset - start};
        }

        #region Nested type: Token

        internal struct Token
        {
            public int Length;
            public int Offset;
            public string Value;
        }

        #endregion
    }

    internal class Variable
    {
        #region Source enum

        public enum Source
        {
            Normal,
            Input,
            Output,
            Persist,
        }

        #endregion

        public string Name;
        public string Type;
        public Source VariableSource;
    }
}