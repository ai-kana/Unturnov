using Cysharp.Threading.Tasks;
using SDG.Unturned;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Translations;
using Command = Unturnov.Core.Commands.Framework.Command;

namespace Unturnov.Core.Commands;

[CommandData("time", "t")]
[CommandSyntax("<[get | set]>")]
public class TimeCommand : Command
{
    public TimeCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertOnDuty();
        Context.AssertPermission("time");
        throw Context.Reply("<[get | set]>");
    }
}

[CommandParent(typeof(TimeCommand))]
[CommandData("get")]
public class TimeGetCommand : Command
{
    public TimeGetCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("time");
        Context.AssertOnDuty();
        
        byte hour = (byte)(LightingManager.time / 150);
        byte minutes = (byte)((LightingManager.time % 150) / 2.5);
        
        // Offsetting time by 6 hours, so that when LightingManager.time is 0, it will show as 6:00.
        hour += 6;
        
        if (hour > 23) hour -= 24;
        
        string time = $"{hour:00}:{minutes:00}";

        throw Context.Reply(TranslationList.CurrentTime, time, LightingManager.time);
    }
}

[CommandParent(typeof(TimeCommand))]
[CommandData("set")]
[CommandSyntax("<[time]>")]
public class TimeSetCommand : Command
{
    public TimeSetCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("time");
        Context.AssertOnDuty();
        Context.AssertArguments(1);

        uint time = Context.Parse<uint>();
        
        LightingManager.time = time;
        throw Context.Reply(TranslationList.TimeSet, time);       
    }
}

[CommandParent(typeof(TimeSetCommand))]
[CommandData("day")]
public class TimeDayCommand : Command
{
    public TimeDayCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("time");
        Context.AssertOnDuty();

        LightingManager.time = (uint)(LightingManager.cycle * LevelLighting.transition);
        throw Context.Reply(TranslationList.TimeSetDayOrNight, new TranslationPackage(TranslationList.Day));
    }
}

[CommandParent(typeof(TimeSetCommand))]
[CommandData("night")]
public class TimeNightCommand : Command
{
    public TimeNightCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("time");
        Context.AssertOnDuty();

        LightingManager.time = (uint)(LightingManager.cycle * (LevelLighting.bias + LevelLighting.transition));
        throw Context.Reply(TranslationList.TimeSetDayOrNight, new TranslationPackage(TranslationList.Night));
    }
}