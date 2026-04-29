using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Powers;


public sealed class HopyungPower : StS2HoshinoPower
{
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterAutoPrePlayPhaseEnteredLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != base.Owner)
        {
            return;
        }
        ICombatState combatState = player.Creature.CombatState;
        Flash();
        bool flag;
        using (CardSelectCmd.PushSelector(new VakuuCardSelector()))
        {
            int cardsPlayed;
            for (cardsPlayed = 0; cardsPlayed < 13; cardsPlayed++)
            {
                if (CombatManager.Instance.IsOverOrEnding)
                {
                    break;
                }
                if (CombatManager.Instance.IsPlayerReadyToEndTurn(player))
                {
                    break;
                }
                CardPile pile = PileType.Hand.GetPile(base.Owner.Player);
                CardModel card = pile.Cards.FirstOrDefault((CardModel c) => c.CanPlay());
                if (card == null)
                {
                    break;
                }
                Creature? target = GetTarget(card, combatState);
                await card.SpendResources();
                await CardCmd.AutoPlay(choiceContext, card, target, AutoPlayType.Default, skipXCapture: true);
            }
            flag = cardsPlayed >= 13;
            if (cardsPlayed == 0)
            {
                return;
            }
        }
        LocString line = (flag ? new LocString("relics", "WHISPERING_EARRING.warning") : new LocString("relics", "WHISPERING_EARRING.approval"));
        TalkCmd.Play(line, base.Owner, VfxColor.Purple);
        
        await PowerCmd.Apply<HopyungPower>(choiceContext, player.Creature,
            -base.Amount,
            player.Creature,
            null);
    }

    private Creature? GetTarget(CardModel card, ICombatState combatState)
    {
        var ownerCombatState = base.Owner.CombatState;
        if (ownerCombatState != null)
        {
            Rng combatTargets = ownerCombatState.RunState.Rng.CombatTargets;
            return card.TargetType switch
            {
                TargetType.AnyEnemy => combatState.HittableEnemies.FirstOrDefault(), 
                TargetType.AnyAlly => combatTargets.NextItem(combatState.Allies.Where((Creature c) =>
                {
                    if (c == null || !c.IsAlive) return false;
                    return c.IsPlayer && c != base.Owner;
                })), 
                TargetType.AnyPlayer => base.Owner, 
                _ => null, 
            };
        }

        return null;
    }
}
