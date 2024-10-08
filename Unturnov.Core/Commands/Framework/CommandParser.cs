using System.Collections.Concurrent;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
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

        RegisterCommandParser(new Vector3Parser());
    }

    public static void RegisterCommandParser<T>(ArgumentParser<T> parser)
    {
        _Parsers.TryAdd(parser.Type, parser);
    }

    private static bool TryGetParser<T>(out IArgumentParser parser)
    {
        return _Parsers.TryGetValue(typeof(T), out parser);
    }

    public static T Parse<T>(IEnumerator<string> enumerator)
    {
        if (!TryGetParser<T>(out IArgumentParser parser))
        {
            throw new KeyNotFoundException(Formatter.Format("{0} is not a parseable type", typeof(T).Name));
        }

        if (!parser.TryParseArgument(enumerator, out object? result))
        {
            throw new UserMessageException(Formatter.Format("{0} is not a valid {1}", enumerator.Current, typeof(T).Name));
        }

        return (T)result!;
    }

    public static bool TryParse<T>(IEnumerator<string> enumerator, out T result)
    {
        result = default(T)!;
        if (!TryGetParser<T>(out IArgumentParser parser))
        {
            throw new (Formatter.Format("{0} is not a parseable type", typeof(T).Name));
        }

        if (!parser.TryParseArgument(enumerator, out object? temp))
        {
            return false;
        }

        result = (T)temp!;
        return true;
    }

    private class ByteParser : ArgumentParser<byte>
    {
        public override bool TryParse(IEnumerator<string> enumerator, out byte result)
        {
            return byte.TryParse(enumerator.Current, out result);
        }
    }

    private class ShortParser : ArgumentParser<short>
    {
        public override bool TryParse(IEnumerator<string> enumerator, out short result)
        {
            return short.TryParse(enumerator.Current, out result);
        }
    }

    private class IntParser : ArgumentParser<int>
    {
        public override bool TryParse(IEnumerator<string> enumerator, out int result)
        {
            return int.TryParse(enumerator.Current, out result);
        }
    }

    private class LongParser : ArgumentParser<long>
    {
        public override bool TryParse(IEnumerator<string> enumerator, out long result)
        {
            return long.TryParse(enumerator.Current, out result);
        }
    }

    private class UShortParser : ArgumentParser<ushort>
    {
        public override bool TryParse(IEnumerator<string> enumerator, out ushort result)
        {
            return ushort.TryParse(enumerator.Current, out result);
        }
    }

    private class UIntParser : ArgumentParser<uint>
    {
        public override bool TryParse(IEnumerator<string> enumerator, out uint result)
        {
            return uint.TryParse(enumerator.Current, out result);
        }
    }

    private class ULongParser : ArgumentParser<ulong>
    {
        public override bool TryParse(IEnumerator<string> enumerator, out ulong result)
        {
            return ulong.TryParse(enumerator.Current, out result);
        }
    }

    private class FloatParser : ArgumentParser<float>
    {
        public override bool TryParse(IEnumerator<string> enumerator, out float result)
        {
            return float.TryParse(enumerator.Current, out result);
        }
    }

    private class DoubleParser : ArgumentParser<double>
    {
        public override bool TryParse(IEnumerator<string> argument, out double result)
        {
            return double.TryParse(argument.Current, out result);
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

        public override bool TryParse(IEnumerator<string> enumerator, out bool result)
        {
            if (TrueStrings.Contains(enumerator.Current, StringComparer.OrdinalIgnoreCase))
            {
                result = true;
                return true;
            }

            if (FalseStrings.Contains(enumerator.Current, StringComparer.OrdinalIgnoreCase))
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
        public override bool TryParse(IEnumerator<string> enumerator, out Role result)
        {
            result = RoleManager.GetRole(enumerator.Current)!;
            if (result == null)
            {
                return false;
            }

            return true;
        }
    }

    private class UnturnovPlayerParser : ArgumentParser<UnturnovPlayer>
    {
        public override bool TryParse(IEnumerator<string> enumerator, out UnturnovPlayer result)
        {
            return UnturnovPlayerManager.TryFindPlayer(enumerator.Current, out result);
        }
    }

    private class CSteamIDParser : ArgumentParser<CSteamID>
    {
        public override bool TryParse(IEnumerator<string> argument, out CSteamID result)
        {
            return PlayerTool.tryGetSteamID(argument.Current, out result);
        }
    }

    private class Vector3Parser : ArgumentParser<Vector3>
    {
        public override bool TryParse(IEnumerator<string> enumerator, out Vector3 result)
        {
            result = Vector3.zero;
            if (!float.TryParse(enumerator.Current, out float x))
            {
                return false;
            }

            enumerator.MoveNext();
            if (!float.TryParse(enumerator.Current, out float y))
            {
                return false;
            }

            enumerator.MoveNext();
            if (!float.TryParse(enumerator.Current, out float z))
            {
                return false;
            }

            result = new(x, y, z);
            return true;
        }
    }
}
