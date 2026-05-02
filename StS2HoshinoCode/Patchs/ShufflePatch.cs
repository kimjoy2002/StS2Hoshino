using System;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace StS2Hoshino.StS2HoshinoCode.Patchs;

[HarmonyPatch(typeof(CardPileCmd))]
public static class ShufflePatch
{
    public static bool IsShuffling = false;

    [HarmonyPatch(nameof(CardPileCmd.Shuffle))]
    [HarmonyPrefix]
    public static void ShufflePrefix(PlayerChoiceContext choiceContext, Player player)
    {
        IsShuffling = true;
    }

    [HarmonyPatch(nameof(CardPileCmd.Shuffle))]
    [HarmonyPostfix]
    public static void ShufflePostfix(ref Task __result)
    {
        __result = WrapShuffleTask(__result);
    }

    private static async Task WrapShuffleTask(Task originalTask)
    {
        try
        {
            await originalTask;
        }
        finally
        {
            IsShuffling = false;
        }
    }
}
