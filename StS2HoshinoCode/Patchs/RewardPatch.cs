using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;
using StS2Hoshino.StS2HoshinoCode.Rewards;

namespace StS2Hoshino.StS2HoshinoCode.Patchs;

[HarmonyPatch(typeof(Reward))]
public static class RewardPatch
{
    [HarmonyPatch(nameof(Reward.FromSerializable))]
    [HarmonyPrefix]
    public static bool FromSerializablePrefix(SerializableReward save, Player player, ref Reward __result)
    {
        if (save.RewardType == CardEnchantReward.EnchantRewardType)
        {
            __result = CardEnchantReward.FromSerializable(save, player);
            return false;
        }
        return true;
    }
}
