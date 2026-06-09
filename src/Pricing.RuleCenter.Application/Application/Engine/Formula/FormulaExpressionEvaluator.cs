using System.Globalization;

namespace Pricing.RuleCenter.Core.Engine.Formula;

/// <summary>
/// 受控表达式公式求值器。
/// </summary>
/// <remarks>
/// <para>
/// 该求值器只支持 decimal 四则运算、括号、白名单变量和白名单函数，不执行脚本、不反射、不访问外部对象。
/// 它服务于 FORMULA_CALC / EXPRESSION_FORMULA 动作，目的是让少量公式可配置，同时保持资金计算可控。
/// </para>
/// <para>
/// 解析器采用递归下降实现，语法优先级为：括号/变量/函数/数字 → 乘除 → 加减。
/// 所有数值使用 decimal，避免 double/float 带来的金额精度问题。
/// </para>
/// </remarks>
public sealed class FormulaExpressionEvaluator
{
    /// <summary>
    /// 白名单公式函数注册表。
    /// </summary>
    private readonly FormulaFunctionRegistry _functions;

    /// <summary>
    /// 初始化表达式公式求值器。
    /// </summary>
    /// <param name="functions">白名单公式函数注册表。</param>
    public FormulaExpressionEvaluator(FormulaFunctionRegistry functions)
    {
        _functions = functions;
    }

    /// <summary>
    /// 对表达式执行求值。
    /// </summary>
    /// <param name="expression">表达式文本。</param>
    /// <param name="context">公式求值上下文，只暴露允许参与计算的数值变量。</param>
    /// <returns>表达式计算结果。</returns>
    public decimal Evaluate(string expression, FormulaEvaluationContext context)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new InvalidOperationException("表达式不能为空");
        }

        var parser = new Parser(expression, context, _functions);
        return parser.Parse();
    }

    /// <summary>
    /// 递归下降表达式解析器。
    /// </summary>
    /// <remarks>
    /// Parser 是单次求值对象，内部维护当前位置，不跨请求复用，因此不需要线程同步。
    /// </remarks>
    private sealed class Parser
    {
        /// <summary>待解析表达式文本。</summary>
        private readonly string _text;
        /// <summary>公式变量上下文。</summary>
        private readonly FormulaEvaluationContext _context;
        /// <summary>白名单函数注册表。</summary>
        private readonly FormulaFunctionRegistry _functions;
        /// <summary>当前解析位置。</summary>
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
            // 顶层必须消耗完整表达式。若解析出一个合法前缀后仍有字符，视为非法配置。
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
            // 表达式层处理加减，优先级低于乘除。
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
            // 项层处理乘除。除数为 0 是资金计算错误，必须抛出并由动作管线按 STOP 回滚。
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
            // 因子层处理正负号、括号、变量/函数和数字。
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
            // 标识符后跟 "(" 表示函数调用；否则按变量名解析。
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

            // 未知变量不允许按 0 处理，否则配置拼写错误会静默改变金额。
            throw Error($"不支持的公式变量: {identifier}");
        }

        private decimal ParseFunctionCall(string name)
        {
            // 函数必须在注册表中声明，避免表达式调用任意方法。
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
            // 数字使用 InvariantCulture 解析，配置表达式统一使用 "." 作为小数点。
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
