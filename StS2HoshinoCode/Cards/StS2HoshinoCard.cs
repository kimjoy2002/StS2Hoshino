using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using StS2Hoshino.StS2HoshinoCode.Utils;
using System.Threading.Tasks;
using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Random;
using StS2Hoshino.StS2HoshinoCode.CardModels;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Cards;

public abstract class StS2HoshinoCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public virtual int AmmoCost { get; set; } = 0;
    public virtual int AmmoCostMax { get; set; } = 0;
    public static bool IsLastShot { get; set; } = false;


    [CustomEnum]
    public static CardTag BulletCard;
    [CustomEnum]
    public static CardTag BulletBoxCard;
    
    protected override bool ShouldGlowGoldInternal 
    {
        get
        {
            if (this is IRunout)
            {
                return AmmoClass.isEmptyAmmo(base.Owner);
            }
            return false;
        }
    }
    
    protected override bool IsPlayable => base.IsPlayable && AmmoClass.hasAmmo(AmmoCost, Owner);
    public override string CustomPortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_p.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : (type==CardType.Attack?"temp_attack_p.png":
                (type==CardType.Power?"temp_power_p.png":"temp_skill_p.png")).CardImagePath();
        }
    }
    public override string PortraitPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
            return ResourceLoader.Exists(path) ? path : (type==CardType.Attack?"temp_attack.png":
                (type==CardType.Power?"temp_power.png":"temp_skill.png")).CardImagePath();
        }
    }

    protected virtual Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }

    
    public virtual Task OnChoose(PlayerChoiceContext choiceContext, CardPlay play)
    {
        return Task.CompletedTask;
    }
    
    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        IsLastShot = false;
        int ammoNeeded = AmmoCost;

        int beforeAmmo = AmmoClass.GetCurrentAmmo(Owner);
        if (beforeAmmo == 1)
        {
            IsLastShot = true;
        }
        if (!AmmoClass.hasAmmo(ammoNeeded, Owner))
        {
            var speaker = Owner?.Creature;
            if (speaker != null)
            {
                //MegaCrit.Sts2.Core.Nodes.Vfx.NSpeechBubbleVfx.Create("탄약이 부족해!", speaker, 2.0);
            }
            return;
        }

        bool onRunout = false;
        if (ammoNeeded > 0)
        {
            await AmmoClass.LoseAmmo(choiceContext, ammoNeeded, Owner);
        }

        if (AmmoClass.isEmptyAmmo(Owner))
        {
            onRunout = true;
        }

        await OnHoshinoPlay(choiceContext, play);
        if (onRunout && this is IRunout runout)
        {
            await runout.OnRunout(choiceContext, play);
        }
        IsLastShot = false;
    }
    
    // public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    // {
    //     if (card == this && this is IInvade invade)
    //     {
    //         return invade.OnInvade(choiceContext,card);
    //     }
    //     return Task.CompletedTask;
    // }

    public override async Task AfterCardChangedPiles(
        CardModel card,
        PileType oldPileType,
        AbstractModel? source)
    {
        if (card == this && oldPileType == PileType.Draw && this is IInvade invade)
        {
            CardPile? pile = base.Pile;
            if (pile != null && pile.Type == PileType.Hand)
            {
                AmmoClass.AddInvadeCount(base.Owner);
                await invade.OnInvade(new ThrowingPlayerChoiceContext(), base.Owner, card);
                await HoshinoHook.OnInvaded(new ThrowingPlayerChoiceContext(), Owner, card);
            }
        }
    }
    
    public static async Task OnlyDeckShuffle(PlayerChoiceContext choiceContext, Player player)
    {
        CardPile drawPile = PileType.Draw.GetPile(player);
        HashSet<CardModel> drawPileCards = drawPile.Cards.ToHashSet<CardModel>();
        List<CardModel> list = drawPile.Cards.ToList<CardModel>();
        list.StableShuffle<CardModel>(player.RunState.Rng.Shuffle);
        foreach (CardModel card in drawPileCards)
        {
            drawPile.RemoveInternal(card, true);
        }
        foreach (CardModel card in list)
        {
            drawPile.AddInternal(card, silent: true);
        }
        MegaCrit.Sts2.Core.Hooks.Hook.ModifyShuffleOrder(player.Creature.CombatState, player, list, false);
        if (CombatManager.Instance.DebugForcedTopCardOnNextShuffle != null)
        {
            if (!list.Remove(CombatManager.Instance.DebugForcedTopCardOnNextShuffle))
                    throw new InvalidOperationException($"Could not find card {CombatManager.Instance.DebugForcedTopCardOnNextShuffle.Id.Entry} in discard pile.");
            list.Insert(0, CombatManager.Instance.DebugForcedTopCardOnNextShuffle);
            CombatManager.Instance.DebugClearForcedTopCardOnNextShuffle();
        }
        await Cmd.CustomScaledWait(0.2f, 0.5f);
        await MegaCrit.Sts2.Core.Hooks.Hook.AfterShuffle(player.Creature.CombatState, choiceContext, player);
    }
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}