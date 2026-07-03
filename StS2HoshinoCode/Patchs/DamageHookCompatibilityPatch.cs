using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Patchs;

[HarmonyPatch]
public static class ModifyDamageAdditiveCompatibilityPatch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        return typeof(AbstractModel).GetMethods()
            .Where(method => method.Name == nameof(AbstractModel.ModifyDamageAdditive));
    }

    public static bool Prefix(
        AbstractModel __instance,
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        ref decimal __result)
    {
        switch (__instance)
        {
            case BulletVigorPower bulletVigorPower:
                __result = bulletVigorPower.ModifyDamageAdditiveCompat(target, amount, props, dealer, cardSource);
                return false;
            case TriggerHappyPower triggerHappyPower:
                __result = triggerHappyPower.ModifyDamageAdditiveCompat(target, amount, props, dealer, cardSource);
                return false;
            default:
                return true;
        }
    }
}

[HarmonyPatch]
public static class ModifyDamageMultiplicativeCompatibilityPatch
{
    public static IEnumerable<MethodBase> TargetMethods()
    {
        return typeof(AbstractModel).GetMethods()
            .Where(method => method.Name == nameof(AbstractModel.ModifyDamageMultiplicative));
    }

    public static bool Prefix(
        AbstractModel __instance,
        Creature? target,
        decimal amount,
        ValueProp props,
        Creature? dealer,
        CardModel? cardSource,
        ref decimal __result)
    {
        if (__instance is not DefensiveStancesPower defensiveStancesPower)
        {
            return true;
        }

        __result = defensiveStancesPower.ModifyDamageMultiplicativeCompat(target, amount, props, dealer, cardSource);
        return false;
    }
}
