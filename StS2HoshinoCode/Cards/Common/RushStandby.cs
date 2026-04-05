using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace StS2Hoshino.StS2HoshinoCode.Cards.Common;

public class RushStandby() : StS2HoshinoCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override HashSet<CardTag> CanonicalTags => [];
    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new BlockVar(1, ValueProp.Move),
        new DynamicVar("Power", 2m)];

    protected override async Task OnHoshinoPlay(PlayerChoiceContext choiceContext, CardPlay play)
    {
        await CommonActions.CardBlock(this, play);
        int amount = base.DynamicVars["Power"].IntValue;
        for (int i = 0; i < amount; i++)
        {
            await ReloadCmd.Execute(choiceContext, base.Owner);
        }
    }
    
    protected override void OnUpgrade()
    {
        base.DynamicVars["Power"].UpgradeValueBy(1m);
    }
}
