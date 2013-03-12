using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace E2ParserTest
{
    internal class E2Parser
    {
        private static readonly char[] Whitespace = new[] {' ', '\t'};
        private readonly IList<string> _lines;
        private readonly string _text;
        private int _line;
        private int _pos;
        private E2Tokenizer _reader;

        public E2Parser(string text)
        {
            _text = text;
            _lines = text.Split(new[] {'\r', '\n'});
            _reader = new E2Tokenizer(text);
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
            return true;
        }

        private void ParseVariables(Variable.Source source)
        {
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