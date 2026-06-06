using System.Globalization;

namespace Pricing.RuleCenter.Core.Engine.Formula;

/// <summary>
/// 受控表达式公式求值器。
/// </summary>
public sealed class FormulaExpressionEvaluator
{
    private readonly FormulaFunctionRegistry _functions;

    /// <summary>
    /// 初始化表达式公式求值器。
    /// </summary>
    public FormulaExpressionEvaluator(FormulaFunctionRegistry functions)
    {
        _functions = functions;
    }

    /// <summary>
    /// 对表达式执行求值。
    /// </summary>
    public decimal Evaluate(string expression, FormulaEvaluationContext context)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new InvalidOperationException("表达式不能为空");
        }

        var parser = new Parser(expression, context, _functions);
        return parser.Parse();
    }

    private sealed class Parser
    {
        private readonly string _text;
        private readonly FormulaEvaluationContext _context;
        private readonly FormulaFunctionRegistry _functions;
        private int _position;

        public Parser(
            string text,
            FormulaEvaluationContext context,
            FormulaFunctionRegistry functions)
        {
            _text = text;
            _context = context;
            _functions = functions;
        }

        public decimal Parse()
        {
            var value = ParseExpression();
            SkipWhiteSpace();
            if (!IsAtEnd)
            {
                throw Error($"非法字符: '{Current}'");
            }

            return value;
        }

        private decimal ParseExpression()
        {
            var value = ParseTerm();
            while (true)
            {
                SkipWhiteSpace();
                if (Match('+'))
                {
                    value += ParseTerm();
                    continue;
                }

                if (Match('-'))
                {
                    value -= ParseTerm();
                    continue;
                }

                return value;
            }
        }

        private decimal ParseTerm()
        {
            var value = ParseFactor();
            while (true)
            {
                SkipWhiteSpace();
                if (Match('*'))
                {
                    value *= ParseFactor();
                    continue;
                }

                if (Match('/'))
                {
                    var divisor = ParseFactor();
                    if (divisor == 0m)
                    {
                        throw Error("表达式除数不能为 0");
                    }

                    value /= divisor;
                    continue;
                }

                return value;
            }
        }

        private decimal ParseFactor()
        {
            SkipWhiteSpace();
            if (Match('+'))
            {
                return ParseFactor();
            }

            if (Match('-'))
            {
                return -ParseFactor();
            }

            if (Match('('))
            {
                var value = ParseExpression();
                Expect(')');
                return value;
            }

            if (IsIdentifierStart(Current))
            {
                return ParseIdentifier();
            }

            if (char.IsDigit(Current) || Current == '.')
            {
                return ParseNumber();
            }

            throw Error("缺少数字、变量、函数或括号");
        }

        private decimal ParseIdentifier()
        {
            var start = _position;
            Advance();
            while (IsIdentifierPart(Current))
            {
                Advance();
            }

            var identifier = _text[start.._position];
            SkipWhiteSpace();
            if (Match('('))
            {
                return ParseFunctionCall(identifier);
            }

            if (_context.TryGetVariable(identifier, out var value))
            {
                return value;
            }

            throw Error($"不支持的公式变量: {identifier}");
        }

        private decimal ParseFunctionCall(string name)
        {
            if (!_functions.IsSupported(name))
            {
                throw Error($"不支持的公式函数: {name}");
            }

            var args = new List<decimal>();
            SkipWhiteSpace();
            if (Match(')'))
            {
                return _functions.Invoke(name, args);
            }

            while (true)
            {
                args.Add(ParseExpression());
                SkipWhiteSpace();
                if (Match(')'))
                {
                    break;
                }

                Expect(',');
            }

            return _functions.Invoke(name, args);
        }

        private decimal ParseNumber()
        {
            var start = _position;
            var hasDot = false;
            while (char.IsDigit(Current) || Current == '.')
            {
                if (Current == '.')
                {
                    if (hasDot)
                    {
                        throw Error("数字格式非法");
                    }

                    hasDot = true;
                }

                Advance();
            }

            var literal = _text[start.._position];
            if (!decimal.TryParse(
                literal,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var value))
            {
                throw Error($"数字格式非法: {literal}");
            }

            return value;
        }

        private void Expect(char value)
        {
            SkipWhiteSpace();
            if (!Match(value))
            {
                throw Error($"缺少 '{value}'");
            }
        }

        private bool Match(char value)
        {
            if (Current != value)
            {
                return false;
            }

            Advance();
            return true;
        }

        private void SkipWhiteSpace()
        {
            while (char.IsWhiteSpace(Current))
            {
                Advance();
            }
        }

        private char Current => _position < _text.Length ? _text[_position] : '\0';

        private bool IsAtEnd => _position >= _text.Length;

        private void Advance()
        {
            if (!IsAtEnd)
            {
                _position++;
            }
        }

        private InvalidOperationException Error(string message)
        {
            return new InvalidOperationException($"{message}，位置 {_position}");
        }

        private static bool IsIdentifierStart(char value)
        {
            return char.IsLetter(value) || value == '_';
        }

        private static bool IsIdentifierPart(char value)
        {
            return char.IsLetterOrDigit(value) || value == '_';
        }
    }
}
