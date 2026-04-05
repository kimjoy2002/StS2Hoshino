using System.Collections.Generic;
using BaseLib.Abstracts;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using StS2Hoshino.StS2HoshinoCode.Cards.Basic;
using StS2Hoshino.StS2HoshinoCode.Relics;

namespace StS2Hoshino.StS2HoshinoCode.Character;


public class StS2Hoshino : PlaceholderCharacterModel
{
	public const string ModId = "StS2Hoshino";
	public const string CharacterId = "StS2Hoshino";

	public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
		new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

	public static readonly Color Color = new("ffffff");

	public override Color NameColor => Color;
	public override CharacterGender Gender => CharacterGender.Neutral;
	public override int StartingHp => 80;

	public override IEnumerable<CardModel> StartingDeck =>
	[
		ModelDb.Card<StrikeHoshino>(),
		ModelDb.Card<StrikeHoshino>(),
		ModelDb.Card<StrikeHoshino>(),
		ModelDb.Card<StrikeHoshino>(),
		ModelDb.Card<DefendHoshino>(),
		ModelDb.Card<DefendHoshino>(),
		ModelDb.Card<DefendHoshino>(),
		ModelDb.Card<DefendHoshino>(),
		ModelDb.Card<FullBarrelFire>(),
		ModelDb.Card<QuickReloadSkill>()
	];

	public override IReadOnlyList<RelicModel> StartingRelics =>
	[
		ModelDb.Relic<HoshinoRelic>()
	];

	public override CardPoolModel CardPool => ModelDb.CardPool<StS2HoshinoCardPool>();
	public override RelicPoolModel RelicPool => ModelDb.RelicPool<StS2HoshinoRelicPool>();
	public override PotionPoolModel PotionPool => ModelDb.PotionPool<StS2HoshinoPotionPool>();


	public override string CustomCharacterSelectBg  => "res://StS2Hoshino/scenes/char_select_bg_hoshino.tscn";
	public override string CustomVisualPath   => "res://StS2Hoshino/scenes/char_hoshino.tscn";
	public override string CustomIconTexturePath => "character_icon_hoshino.png".CharacterUiPath();
	public override string CustomCharacterSelectIconPath => "char_select_hoshino.png".CharacterUiPath();
	public override string CustomCharacterSelectLockedIconPath => "char_select_hoshino_locked.png".CharacterUiPath();
	public override string CustomMapMarkerPath => "map_marker_hoshino.png".CharacterUiPath();
}
