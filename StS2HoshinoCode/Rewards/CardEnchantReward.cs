using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;
using StS2Hoshino.StS2HoshinoCode.Extensions;

namespace StS2Hoshino.StS2HoshinoCode.Rewards;

public class CardEnchantReward : Reward
{
    // 임시 RewardType 값. 나중에 BaseLib의 시스템으로 교체될 예정.
    // 999 같은 큰 값을 사용하거나, 적절한 정수 값을 사용.
    public const RewardType EnchantRewardType = (RewardType)222;

    private readonly int _amount;

    private static string RewardIcon => "reward_icon_sharpcard.png".CharacterUiPath();

    protected override RewardType RewardType => EnchantRewardType;

    public override int RewardsSetIndex => 7;

    protected override string IconPath => RewardIcon;

    public static IEnumerable<string> AssetPaths => [RewardIcon];

    public override bool IsPopulated => true;

    public override LocString Description
    {
        get
        {
            LocString locString = new LocString("gameplay_ui", "COMBAT_REWARD_CARD_ENCHANT");
            locString.Add("Amount", _amount);
            return locString;
        }
    }

    public CardEnchantReward(Player player, int amount) : base(player)
    {
        _amount = amount;
    }

    public override void Populate()
    {
    }

    protected override async Task<bool> OnSelect()
    {
        Log.Info($"Enchant card reward select with amount: {_amount}");
        
        CardSelectorPrefs prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);
        Sharp canonicalEnchantment = ModelDb.Enchantment<Sharp>();
        
        var selectedCards = (await CardSelectCmd.FromDeckGeneric(base.Player, prefs, (CardModel c) => 
            c.Type == CardType.Attack && (c.Enchantment == null || c.Enchantment is Sharp)
        )).ToList();
        
        CardModel? cardModel = selectedCards.FirstOrDefault();
        
        if (cardModel != null)
        {
            if (cardModel.Enchantment is Sharp)
            {
                Log.Info($"Card already has Sharp, manually increasing by 1 to bypass CanEnchant check");
                cardModel.Enchantment.Amount += 1;
                cardModel.FinalizeUpgradeInternal();
            }
            else
            {
                Log.Info($"Applying new Sharp with amount {_amount}");
                CardCmd.Enchant(canonicalEnchantment.ToMutable(), cardModel, _amount);
            }

            CardCmd.Preview(cardModel);
            return true;
        }
        
        return false;
    }

    public override void MarkContentAsSeen()
    {
    }

    public override SerializableReward ToSerializable()
    {
        var save = base.ToSerializable();
        save.GoldAmount = _amount; // amount를 GoldAmount 필드에 임시 저장
        return save;
    }

    public static new CardEnchantReward FromSerializable(SerializableReward save, Player player)
    {
        return new CardEnchantReward(player, save.GoldAmount);
    }
}