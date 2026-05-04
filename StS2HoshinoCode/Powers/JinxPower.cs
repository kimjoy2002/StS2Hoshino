using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace StS2Hoshino.StS2HoshinoCode.Powers;

public interface IOnBulletChanged
{
    Task OnBulletChanged(PlayerChoiceContext ctx, Player player, int beforeBullet, int afterBullet);
}

public class JinxPower : StS2HoshinoPower, IOnBulletChanged
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.None;

    public int Slot3UsedCount = 0;
    public int Slot4UsedCount = 0;

    public JinxPower()
    {
        // 아이콘은 적절한 것으로 설정 (필요시 수정)
    }

    public Task OnBulletChanged(PlayerChoiceContext ctx, Player player, int beforeBullet, int afterBullet)
    {
        if (player.Creature != base.Owner) return Task.CompletedTask;

        // 탄약 소모 시 (before > after)
        for (int i = beforeBullet; i > afterBullet; i--)
        {
            if (i == 3) Slot3UsedCount++;
            if (i == 4) Slot4UsedCount++;
        }

        return Task.CompletedTask;
    }
}
