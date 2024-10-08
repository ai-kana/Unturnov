using Cysharp.Threading.Tasks;
using UnityEngine;
using Unturnov.Core.Commands.Framework;
using Unturnov.Core.Players;

namespace Unturnov.Core.Commands;

[CommandData("movement", "move")]
[CommandSyntax("<[player] [speed,s | jump,j | gravity,g | all,a] [value | reset,r]>")]
public class MovementCommand : Command
{
    public MovementCommand(CommandContext context) : base(context)
    {
    }

    public override UniTask ExecuteAsync()
    {
        Context.AssertPermission("movement");
        Context.AssertOnDuty();
        Context.AssertArguments(2);
        
        //If you think of a better way to do this, please go ahead;

        UnturnovPlayer player = Context.Parse<UnturnovPlayer>();
        Context.MoveNext();
        string option = Context.Current.ToLower();
        Context.MoveNext();

        float value;
        
        value = Context.MatchParameters("reset", "r") ? 1f : Context.Parse<float>();

        string resetString = "Successfully reset {0}'s {1} multiplier";
        string setString = "Successfully set {0}'s {2} multiplier to {1}";
        
        switch (option)
        {
            case "speed":
            case "s":
                player.Movement.sendPluginSpeedMultiplier(value);
                throw Mathf.Approximately(value, 1f) ? Context.Reply(resetString, player.Name, "speed") : Context.Reply(setString, player.Name, value, "speed");
            case "jump":
            case "j":
                player.Movement.sendPluginJumpMultiplier(value);
                throw Mathf.Approximately(value, 1f) ? Context.Reply(resetString, player.Name, "jump") : Context.Reply(setString, player.Name, value, "jump");
            case "gravity":
            case "g":
                player.Movement.sendPluginGravityMultiplier(value);
                throw Mathf.Approximately(value, 1f) ? Context.Reply(resetString, player.Name, "gravity") : Context.Reply(setString, player.Name, value, "gravity");
            case "all":
            case "a":
                player.Movement.sendPluginGravityMultiplier(value);
                player.Movement.sendPluginSpeedMultiplier(value);
                player.Movement.sendPluginJumpMultiplier(value);
                throw Mathf.Approximately(value, 1f) ? 
                    Context.Reply("Successfully reset {0}'s movement", player.Name) : 
                    Context.Reply("Successfully set {0}'s movement multipliers to {1}", player.Name, value);
            default:
                throw Context.Reply("<[player] [speed,s | jump,j | gravity,g | all,a] [value | reset,r]>");
        }
    }
}