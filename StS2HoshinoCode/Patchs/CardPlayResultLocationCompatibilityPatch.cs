using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Hoshino.StS2HoshinoCode.Cards.Rare;

namespace StS2Hoshino.StS2HoshinoCode.Patchs;

[HarmonyPatch]
public static class CardPlayResultPileCompatibilityPatch
{
    public static MethodBase? TargetMethod()
    {
        return AccessTools.DeclaredMethod(typeof(AbstractModel), "ModifyCardPlayResultPileTypeAndPosition");
    }

    public static bool Prepare()
    {
        return TargetMethod() != null;
    }

    public static void Postfix(
        AbstractModel __instance,
        CardModel card,
        ref (PileType, CardPilePosition) __result)
    {
        if (__instance is not SuppressionAttack suppressionAttack || card != suppressionAttack)
        {
            return;
        }

        if (__result.Item1 == PileType.Discard && PileType.Hand.GetPile(suppressionAttack.Owner).Cards.Count > 0)
        {
            __result = (PileType.Hand, __result.Item2);
        }
    }
}

[HarmonyPatch]
public static class CardPlayResultLocationCompatibilityPatch
{
    private static readonly Type? CardLocationType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.CardLocation");
    private static readonly FieldInfo? PlayerField = CardLocationType?.GetField("player");
    private static readonly FieldInfo? PileTypeField = CardLocationType?.GetField("pileType");
    private static readonly FieldInfo? PositionField = CardLocationType?.GetField("position");

    public static MethodBase? TargetMethod()
    {
        return AccessTools.DeclaredMethod(typeof(AbstractModel), "ModifyCardPlayResultLocation");
    }

    public static bool Prepare()
    {
        return TargetMethod() != null &&
               CardLocationType != null &&
               PlayerField != null &&
               PileTypeField != null &&
               PositionField != null;
    }

    public static void Postfix(AbstractModel __instance, CardModel card, ref object __result)
    {
        if (__instance is not SuppressionAttack suppressionAttack || card != suppressionAttack)
        {
            return;
        }

        if ((PileType?)PileTypeField?.GetValue(__result) != PileType.Discard ||
            PileType.Hand.GetPile(suppressionAttack.Owner).Cards.Count <= 0)
        {
            return;
        }

        __result = Activator.CreateInstance(
            CardLocationType!,
            PlayerField!.GetValue(__result),
            PileType.Hand,
            PositionField!.GetValue(__result))!;
    }
}
