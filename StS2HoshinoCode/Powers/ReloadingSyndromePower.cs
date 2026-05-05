using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using StS2Hoshino.StS2HoshinoCode.Cards;
using StS2Hoshino.StS2HoshinoCode.Cards.Uncommon;
using StS2Hoshino.StS2HoshinoCode.Character;
using StS2Hoshino.StS2HoshinoCode.Utils;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public sealed class ReloadingSyndromePower : StS2HoshinoPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == base.Owner.Player && cardPlay.Card is StS2HoshinoCard hCard)
        {
            if ((hCard.AmmoCost > 0 || hCard is SnapShot) && cardPlay.Card.EnergyCost.GetResolved() >= 1)
            {
                Flash();
                for (int i = 0; i < Amount; i++)
                {
                    await ReloadCmd.Execute(choiceContext, base.Owner.Player);
                }
            }
        }
    }
}
