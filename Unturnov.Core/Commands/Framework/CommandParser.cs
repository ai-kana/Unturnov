using System.Collections.Concurrent;
using Unturnov.Core.Formatting;

namespace Unturnov.Core.Commands.Framework;

public class CommandParser
{
    private static ConcurrentDictionary<Type, IArgumentParser> _Parsers;

    static CommandParser()
    {
        _Parsers = new();

        RegisterCommandParser(new ByteParser());
        RegisterCommandParser(new ShortParser());
        RegisterCommandParser(new IntParser());
        RegisterCommandParser(new LongParser());

        RegisterCommandParser(new UShortParser());
        RegisterCommandParser(new UIntParser());
        RegisterCommandParser(new ULongParser());

        RegisterCommandParser(new FloatParser());
        RegisterCommandParser(new BoolParser());

        RegisterCommandParser(new BoolParser());
    }

    public static void RegisterCommandParser<T>(ArgumentParser<T> parser)
    {
        _Parsers.TryAdd(parser.Type, parser);
    }

    private static bool TryGetParser<T>(out IArgumentParser parser)
    {
        return _Parsers.TryGetValue(typeof(T), out parser);
    }

    public static T Parse<T>(string argument)
    {
        if (!TryGetParser<T>(out IArgumentParser parser))
        {
            throw new KeyNotFoundException(Formatter.Format("{0} is not a parseable type", typeof(T).Name));
        }

        if (!parser.TryParseArgument(argument, out object? result))
        {
            throw new UserMessageException(Formatter.Format("{0} is not a valid {1}", argument, typeof(T).Name));
        }

        return (T)result!;
    }

    public static bool TryParse<T>(string argument, out T result)
    {
        result = default(T)!;
        if (!TryGetParser<T>(out IArgumentParser parser))
        {
            throw new (Formatter.Format("{0} is not a parseable type", typeof(T).Name));
        }

        if (!parser.TryParseArgument(argument, out object? temp))
        {
            return false;
        }

        result = (T)temp!;
        return true;
    }

    private class ByteParser : ArgumentParser<byte>
    {
        public override bool TryParse(string argument, out byte result)
        {
            return byte.TryParse(argument, out result);
        }
    }

    private class ShortParser : ArgumentParser<short>
    {
        public override bool TryParse(string argument, out short result)
        {
            return short.TryParse(argument, out result);
        }
    }

    private class IntParser : ArgumentParser<int>
    {
        public override bool TryParse(string argument, out int result)
        {
            return int.TryParse(argument, out result);
        }
    }

    private class LongParser : ArgumentParser<long>
    {
        public override bool TryParse(string argument, out long result)
        {
            return long.TryParse(argument, out result);
        }
    }

    private class UShortParser : ArgumentParser<ushort>
    {
        public override bool TryParse(string argument, out ushort result)
        {
            return ushort.TryParse(argument, out result);
        }
    }

    private class UIntParser : ArgumentParser<uint>
    {
        public override bool TryParse(string argument, out uint result)
        {
            return uint.TryParse(argument, out result);
        }
    }

    private class ULongParser : ArgumentParser<ulong>
    {
        public override bool TryParse(string argument, out ulong result)
        {
            return ulong.TryParse(argument, out result);
        }
    }

    private class FloatParser : ArgumentParser<float>
    {
        public override bool TryParse(string argument, out float result)
        {
            return float.TryParse(argument, out result);
        }
    }

    private class DoubleParser : ArgumentParser<double>
    {
        public override bool TryParse(string argument, out double result)
        {
            return double.TryParse(argument, out result);
        }
    }

    private class BoolParser : ArgumentParser<bool>
    {
        private string[] TrueStrings = 
        {
            "y",
            "yes",
            "true"
        };

        private string[] FalseStrings = 
        {
            "n",
            "no",
            "false"
        };

        public override bool TryParse(string argument, out bool result)
        {
            if (TrueStrings.Contains(argument, StringComparer.OrdinalIgnoreCase))
            {
                result = true;
                return true;
            }

            if (FalseStrings.Contains(argument, StringComparer.OrdinalIgnoreCase))
            {
                result = false;
                return true;
            }

            result = false;
            return false;
        }
    }
}
