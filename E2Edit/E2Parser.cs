using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace E2Edit
{
    class E2Parser
    {
        private readonly string _text;
        private readonly IList<string> _lines;
        private int _pos;
        private int _line;
        private static readonly char[] Whitespace = new[]{' ', '\t'};
        public IList<Variable> Variables{ get; private set; }

        public E2Parser(string text)
        {
            _text = text;
            _lines = text.Split(new[] {'\r', '\n'});
            Variables = new List<Variable>();
        }

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

    internal class Variable
    {
        public enum Source
        {
            Normal,
            Input,
            Output,
            Persist,
        }

        public string Name;
        public string Type;
        public Source VariableSource;
    }
}
