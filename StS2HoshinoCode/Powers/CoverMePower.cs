using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public sealed class CoverMePower : StS2HoshinoPower, IOnReloaded
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnReload(PlayerChoiceContext choiceContext, Player player, bool useButton)
    {
        if (Owner.Player != player || CombatState == null)
            return;

        Flash();
        foreach (var ally in CombatState.Players)
        {
            if (ally != player && ally.Creature.IsAlive)
            {
                await CardPileCmd.Draw(choiceContext, Amount, ally);
                await PowerCmd.Apply<CoverMeStrengthPower>(
                    choiceContext,
                    ally.Creature,
                    Amount,
                    Owner,
                    null);
            }
        }
    }

    public override async Task AfterSideTurnEnd(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner))
            await PowerCmd.Remove(this);
    }
}
