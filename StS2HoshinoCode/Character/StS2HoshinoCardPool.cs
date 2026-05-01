using BaseLib.Abstracts;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace StS2Hoshino.StS2HoshinoCode.Character;

public class StS2HoshinoCardPool : CustomCardPoolModel
{
    public override string Title => StS2Hoshino.CharacterId; //This is not a display name.

    public override string BigEnergyIconPath => "charui/hoshino_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/hoshino_text_energy.png".ImagePath();


    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 1f; //Hue; changes the color.
    public override float S => 1f; //Saturation
    public override float V => 1f; //Brightness

    public override Texture2D? CustomFrame(CustomCardModel card)
    {
        return card.Type switch
        {
            CardType.Attack => PreloadManager.Cache.GetTexture2D("cards/frame_attack.png".ImagePath()),
            CardType.Power => PreloadManager.Cache.GetTexture2D("cards/frame_power.png".ImagePath()),
            CardType.Skill => PreloadManager.Cache.GetTexture2D("cards/frame_skill.png".ImagePath()),
            _ => base.CustomFrame(card)
        };
    }

    //Color of small card icons
    public override Color DeckEntryCardColor => new("ffd0dc");

    public override bool IsColorless => false;
}