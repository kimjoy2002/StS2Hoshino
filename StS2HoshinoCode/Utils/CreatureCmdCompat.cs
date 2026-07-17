using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace StS2Hoshino.StS2HoshinoCode.Utils;

public static class CreatureCmdCompat
{
    private static readonly MethodInfo? LoseBlockMainMethod = typeof(CreatureCmd)
        .GetMethods()
        .FirstOrDefault(method =>
            method.Name == nameof(CreatureCmd.LoseBlock) &&
            method.GetParameters() is { Length: 2 } parameters &&
            parameters[0].ParameterType == typeof(Creature));

    private static readonly MethodInfo? LoseBlockBetaMethod = typeof(CreatureCmd)
        .GetMethods()
        .FirstOrDefault(method =>
            method.Name == nameof(CreatureCmd.LoseBlock) &&
            method.GetParameters() is { Length: 4 } parameters &&
            parameters[0].ParameterType == typeof(PlayerChoiceContext));

    public static Task LoseBlock(Creature target, decimal amount, Creature? remover = null)
    {
        if (LoseBlockMainMethod != null)
        {
            return (Task)LoseBlockMainMethod.Invoke(null, [target, amount])!;
        }

        if (LoseBlockBetaMethod != null)
        {
            return (Task)LoseBlockBetaMethod.Invoke(null, [new BlockingPlayerChoiceContext(), target, amount, remover])!;
        }

        throw new MissingMethodException(typeof(CreatureCmd).FullName, nameof(CreatureCmd.LoseBlock));
    }
}
