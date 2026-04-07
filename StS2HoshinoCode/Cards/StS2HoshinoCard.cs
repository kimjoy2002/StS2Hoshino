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
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using StS2Hoshino.StS2HoshinoCode.CardModels;

namespace StS2Hoshino.StS2HoshinoCode.Cards;

[Pool(typeof(StS2HoshinoCardPool))]
public abstract class StS2HoshinoCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public virtual int AmmoCost { get; set; } = 0;
    public virtual int AmmoCostMax { get; set; } = 0;


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

    protected sealed override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        int ammoNeeded = AmmoCost;
        
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
            AmmoClass.LoseAmmo(ammoNeeded, Owner);
            await AmmoClass.ProcessPendingTriggers(choiceContext);
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
    }
    
    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card == this && this is IInvade invade)
        {
            return invade.OnInvade(choiceContext,card);
        }
        return Task.CompletedTask;
    }
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}