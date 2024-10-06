using System.Collections.Concurrent;
using Steamworks;
using Unturnov.Core.Formatting;
using Unturnov.Core.Players;
using Unturnov.Core.Roles;

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

        RegisterCommandParser(new RoleParser());

        RegisterCommandParser(new UnturnovPlayerParser());
        RegisterCommandParser(new CSteamIDParser());
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

    private class RoleParser : ArgumentParser<Role>
    {
        public override bool TryParse(string argument, out Role result)
        {
            result = RoleManager.GetRole(argument)!;
            if (result == null)
            {
                return false;
            }

            return true;
        }
    }

    private class UnturnovPlayerParser : ArgumentParser<UnturnovPlayer>
    {
        public override bool TryParse(string argument, out UnturnovPlayer result)
        {
            if (argument.StartsWith("765") && argument.Length == 17)
            {
                if (!ulong.TryParse(argument, out ulong id))
                {
                    result = null!;
                    return false;
                }

                CSteamID steamID = new(id);
                return UnturnovPlayerManager.Players.TryGetValue(steamID, out result);
            }

            result = UnturnovPlayerManager.Players.Values.FirstOrDefault(x => x.Name.Contains(argument, StringComparison.OrdinalIgnoreCase));
            if (result == null)
            {
                return false;
            }

            return true;
        }
    }

    private class CSteamIDParser : ArgumentParser<CSteamID>
    {
        public override bool TryParse(string argument, out CSteamID result)
        {
            if (!argument.StartsWith("765") || argument.Length != 17)
            {
                result = CSteamID.Nil;
                return false;
            }

            if (!ulong.TryParse(argument, out ulong id))
            {
                result = CSteamID.Nil;
                return false;
            }

            result = new(id);
            return true;
        }
    }
}
