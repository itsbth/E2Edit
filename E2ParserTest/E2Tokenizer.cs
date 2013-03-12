using System;
using System.Linq;
using System.Text;

namespace E2ParserTest
{
    internal class E2Tokenizer
    {
        private static readonly char[] Operators = new[] {'=', '!', '+', '-', '*', '/', '%', '&', '|', ','};
        private static readonly char[] DblOperators = new[] {'=', '!', '+', '-', '*', '/'};
        private readonly string _code;
        private readonly int _len;
        private int _i;

        public E2Tokenizer(string code)
        {
            _code = code;
            _len = code.Length;
        }

        public Token Read()
        {
            ConsumeWhitespace();
            if (_i >= _len) return new Token {Type = Token.TokenType.EOF};
            if (_code[_i] == '\n')
            {
                _i += 1;
                return new Token {Type = Token.TokenType.NewLine, Value = "\n"};
            }
            if (Char.IsLetter(_code[_i]))
            {
                return new Token {Type = Token.TokenType.Word, Value = ReadWord()};
            }
            if (Char.IsDigit(_code[_i]))
            {
                return new Token {Type = Token.TokenType.Number, Value = ReadNumber()};
            }
            switch (_code[_i])
            {
                case '@':
                    _i += 1;
                    return new Token {Type = Token.TokenType.DirectiveStart, Value = "@"};
                case '(':
                    _i += 1;
                    return new Token {Type = Token.TokenType.ParenStart, Value = "("};
                case ')':
                    _i += 1;
                    return new Token {Type = Token.TokenType.ParenEnd, Value = ")"};
                case '{':
                    _i += 1;
                    return new Token {Type = Token.TokenType.BracketStart, Value = "{"};
                case '}':
                    _i += 1;
                    return new Token {Type = Token.TokenType.BracketEnd, Value = "}"};
                case '[':
                    _i += 1;
                    return new Token {Type = Token.TokenType.SqBracketStart, Value = "["};
                case ']':
                    _i += 1;
                    return new Token {Type = Token.TokenType.SqBracketEnd, Value = "]"};
                case ':':
                    _i += 1;
                    return new Token {Type = Token.TokenType.Colon, Value = ":"};
                case '~':
                    _i += 1;
                    return new Token {Type = Token.TokenType.Changed, Value = "~"};
                case '$':
                    _i += 1;
                    return new Token {Type = Token.TokenType.Delta, Value = "$"};
            }
            if (Operators.Contains(_code[_i]))
            {
                if (_i > _len || !(DblOperators.Contains(_code[_i]) && _code[_i + 1] == '='))
                {
                    _i += 1;
                    return new Token {Type = Token.TokenType.Symbol, Value = _code[_i - 1].ToString()};
                }
                _i += 2;
                return new Token {Type = Token.TokenType.Symbol, Value = _code.Substring(_i - 2, 2)};
            }
            if (_code[_i] == '"')
            {
                var buff = new StringBuilder();
                buff.Append('"');
                _i += 1;
                while (_i < _len && !(_code[_i] == '"' && _code[_i - 1] != '\\'))
                {
                    buff.Append(_code[_i++]);
                }
                buff.Append('"');
                _i += 1;
                return new Token {Type = Token.TokenType.String, Value = buff.ToString()};
            }
            if (_code[_i] == '#')
            {
                int li = _code.IndexOf('\n', _i);
                if (li == -1) li = _len;
                string str = _code.Substring(_i, li - _i);
                _i = li;
                return new Token {Type = Token.TokenType.Comment, Value = str};
            }
            throw new Exception("Unexpected token " + _code[_i] + ".");
        }

        private string ReadNumber()
        {
            var buff = new StringBuilder();
            while (_i < _len && (Char.IsDigit(_code[_i]) || _code[_i] == '.'))
            {
                buff.Append(_code[_i]);
                _i += 1;
            }
            return buff.ToString();
        }

        private string ReadWord()
        {
            var buff = new StringBuilder();
            while (_i < _len && Char.IsLetterOrDigit(_code[_i]))
            {
                buff.Append(_code[_i]);
                _i += 1;
            }
            return buff.ToString();
        }

        private void ConsumeWhitespace()
        {
            while (_i < _len && Char.IsWhiteSpace(_code[_i]) && _code[_i] != '\n')
                _i += 1;
        }
    }

    internal struct Token
    {
        public TokenType Type;
        public string Value;

        public override string ToString()
        {
            return String.Format("<Token Type='{0}' Value='{1}'>", Type, Value);
        }

        #region Nested type: TokenType

        internal enum TokenType
        {
            EOF,
            Word,
            Number,
            NewLine,
            Comment,
            DirectiveStart,
            UnaryOperator,
            BinaryOperator,
            ParenStart,
            ParenEnd,
            BracketStart,
            BracketEnd,
            SqBracketStart,
            SqBracketEnd,
            Colon,
            Changed,
            Delta,
            Symbol,
            String
        }

        #endregion
    }

    public abstract class LineInfo
    {
        public int Line { get; set; }
        public int Offset { get; set; }
    }

    public class CodeLineInfo : LineInfo
    {
    }

    public class DirectiveLineInfo : LineInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}