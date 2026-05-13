using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public interface IBulletPowerInterface
{
    Task UseBullet(PlayerChoiceContext choiceContext, CardModel card, Creature? target, Creature? applier, int amount);
    Task UseBulletForMulti(PlayerChoiceContext choiceContext, CardModel card, IEnumerable<Creature> targets, Creature? applier, int amount);
}