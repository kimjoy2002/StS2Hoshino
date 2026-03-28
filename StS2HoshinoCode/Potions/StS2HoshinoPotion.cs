using BaseLib.Abstracts;
using BaseLib.Utils;
using StS2Hoshino.StS2HoshinoCode.Character;

namespace StS2Hoshino.StS2HoshinoCode.Potions;

[Pool(typeof(StS2HoshinoPotionPool))]
public abstract class StS2HoshinoPotion : CustomPotionModel;