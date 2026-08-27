using System.Collections.Generic;
using System.Linq;
using BaseLib.Abstracts;
using BaseLib.Patches.UI;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using StS2Hoshino.StS2HoshinoCode.Extensions;
using StS2Hoshino.StS2HoshinoCode.Cards.Basic;
using StS2Hoshino.StS2HoshinoCode.Powers;
using StS2Hoshino.StS2HoshinoCode.Relics;

namespace StS2Hoshino.StS2HoshinoCode.Character;


public class StS2Hoshino : PlaceholderCharacterModel

{
	public const string ModId = "StS2Hoshino";
	public const string CharacterId = "StS2Hoshino";

	public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } =
		new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

	public static readonly Color Color = new("ffd0dc");

	public override Color NameColor => Color;
	public override Color MapDrawingColor => Color;
	public override CharacterGender Gender => CharacterGender.Feminine;
	public override int StartingHp => 75;

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
	protected override IEnumerable<string> ExtraAssetPaths =>
	[
		"reload_turn_button.png".CharacterUiPath(),
		"reload_turn_button_glow.png".CharacterUiPath()
	];

	public override float AttackAnimDelay => 0.15f;

	public override float CastAnimDelay => 0.25f;
	
	public float ReloadAnimDelay => 1.00f;

	public override string CustomCharacterSelectBg  => "res://StS2Hoshino/scenes/char_select_bg_hoshino.tscn";
	//public override string CustomTrailPath => "res://StS2Hoshino/scenes/card_trail_hoshino.tscn";
	public override string CustomVisualPath   => "res://StS2Hoshino/scenes/char_hoshino.tscn";
	public override string CustomIconTexturePath => "character_icon_hoshino2.png".CharacterUiPath();
	
	
	public override RelicIconData CustomYummyCookie => new(
		"yummy_cookie_hoshino.png".BigRelicImagePath(),
		"yummy_cookie_hoshino.png".RelicImagePath(),
		"yummy_cookie_hoshino_outline.tres".RelicImagePath()
	);
	
	public override string CustomIconPath => "res://StS2Hoshino/scenes/icon_hoshino.tscn";
	public override string CustomIconOutlineTexturePath => "character_icon_hoshino2_outline.png".CharacterUiPath();
	public override string CustomCharacterSelectIconPath => "char_select_hoshino.png".CharacterUiPath();
	public override string CustomCharacterSelectLockedIconPath => "char_select_hoshino_locked.png".CharacterUiPath();
	public override string CustomMapMarkerPath => "map_marker_hoshino.png".CharacterUiPath();

	public override string CustomRestSiteAnimPath => "res://StS2Hoshino/scenes/hoshino_rest_site.tscn";
	public override string CustomMerchantAnimPath => "res://StS2Hoshino/scenes/hoshino_merchant.tscn";

	public override NCreatureVisuals CreateCustomVisuals()
	{
		return NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath);
	}

	
	public override string CustomArmPointingTexturePath =>
		"multiplayer_hand_hoshino_point.png".CharacterUiPath();

	public override string CustomArmRockTexturePath =>
		"multiplayer_hand_hoshino_rock.png".CharacterUiPath();

	public override string CustomArmPaperTexturePath =>
		"multiplayer_hand_hoshino_paper.png".CharacterUiPath();

	public override string CustomArmScissorsTexturePath =>
		"multiplayer_hand_hoshino_scissor.png".CharacterUiPath();

	public override CreatureAnimator SetupCustomAnimationStates(MegaSprite controller)
	{
		Logger.Info("[Hoshino] SetupCustomAnimationStates called");

		var idleNone      = new AnimState("idle_loop", true);
		var idleShield    = new AnimState("idle_loop_shield", true);
		var idleSwim      = new AnimState("idle_loop_swim", true);
		var idleSwimShield = new AnimState("idle_loop_swim_shield", true);
		var lowHealthNone = new AnimState("low_health_loop", true);
		var lowHealthShield = new AnimState("low_health_loop_shield", true);
		var lowHealthSwim = new AnimState("low_health_loop_swim", true);
		var lowHealthSwimShield = new AnimState("low_health_loop_swim_shield", true);
		var enterLowHealthNone = new AnimState("enter_low_health_loop");
		var enterLowHealthShield = new AnimState("enter_low_health_loop_shield");
		var enterLowHealthSwim = new AnimState("enter_low_health_loop_swim");
		var enterLowHealthSwimShield = new AnimState("enter_low_health_loop_swim_shield");

		var shieldUp     = new AnimState("shield_up");
		var shieldUpSwim = new AnimState("shield_up_swim");

		static AnimState MakeState(string name, bool swim, bool shield)
		{
			string suffix = swim ? "_swim" : "";
			if (shield) suffix += "_shield";
			return new AnimState(name + suffix);
		}

		var attacks = new[] { MakeState("attack", false, false), MakeState("attack", false, true), MakeState("attack", true, false), MakeState("attack", true, true) };
		var hurts   = new[] { MakeState("hurt",   false, false), MakeState("hurt",   false, true), MakeState("hurt",   true, false), MakeState("hurt",   true, true) };
		var casts   = new[] { MakeState("cast",   false, false), MakeState("cast",   false, true), MakeState("cast",   true, false), MakeState("cast",   true, true) };
		var dies    = new[] { MakeState("die",    false, false), MakeState("die",    false, true), MakeState("die",    true, false), MakeState("die",    true, true) };
		var reloads = new[] { MakeState("reload", false, false), MakeState("reload", false, true), MakeState("reload", true, false), MakeState("reload", true, true) };
		var swings  = new[] { MakeState("swing",  false, false), MakeState("swing",  false, true), MakeState("swing",  true, false), MakeState("swing",  true, true) };
		var charges  = new[] { MakeState("charge",  false, false), MakeState("charge",  false, false), MakeState("charge",  true, false), MakeState("charge",  true, false) };

		MegaCrit.Sts2.Core.Entities.Creatures.Creature? GetCreature()
		{
			var current = controller.BoundObject as Godot.Node;
			while (current != null)
			{
				if (current is NCreature nCreature)
					return nCreature.Entity;
				current = current.GetParent();
			}
			return null;
		}

		bool HasShield() => GetCreature()?.HasPower<ShieldPower>() == true;
		bool HasSwim()   => GetCreature()?.HasPower<SwimsuitFormPower>() == true;
		bool IsLowHealth() => GetCreature()?.GetHpPercentRemaining() <= 0.25f;

		var conditionalNextStateMethod = typeof(AnimState)
			.GetMethods()
			.FirstOrDefault(method =>
			{
				if (method.Name != "AddNextState")
					return false;

				var parameters = method.GetParameters();
				return parameters.Length == 2
				       && parameters[0].ParameterType == typeof(AnimState)
				       && parameters[1].ParameterType == typeof(System.Func<bool>);
			});

		void SetIdleAfterAction(AnimState actionState, AnimState normalIdle, AnimState enterLowHealth)
		{
			// Conditional next states were added in the beta API. Main keeps the normal idle fallback.
			conditionalNextStateMethod?.Invoke(
				actionState,
				new object[] { enterLowHealth, (System.Func<bool>)IsLowHealth });
			actionState.NextState = normalIdle;
		}

		var idles = new[] { idleNone, idleShield, idleSwim, idleSwimShield };
		var lowHealthIdles = new[] { lowHealthNone, lowHealthShield, lowHealthSwim, lowHealthSwimShield };
		var enterLowHealthStates = new[]
		{
			enterLowHealthNone,
			enterLowHealthShield,
			enterLowHealthSwim,
			enterLowHealthSwimShield
		};
		for (int i = 0; i < enterLowHealthStates.Length; i++)
			enterLowHealthStates[i].NextState = lowHealthIdles[i];

		var allActionStates = new[] { attacks, hurts, casts, reloads, swings, charges };
		foreach (var actionArr in allActionStates)
		{
			for (int i = 0; i < actionArr.Length; i++)
				SetIdleAfterAction(actionArr[i], idles[i], enterLowHealthStates[i]);
		}

		SetIdleAfterAction(shieldUp, idleShield, enterLowHealthShield);
		SetIdleAfterAction(shieldUpSwim, idleSwimShield, enterLowHealthSwimShield);

		int GetIdleIndex() => HasSwim() ? (HasShield() ? 3 : 2) : (HasShield() ? 1 : 0);
		var animator = new CreatureAnimator(
			IsLowHealth() ? lowHealthIdles[GetIdleIndex()] : idles[GetIdleIndex()],
			controller);

		void AddBranches(string trigger, AnimState[] states)
		{
			animator.AddAnyState(trigger, states[3], () => HasSwim() && HasShield());
			animator.AddAnyState(trigger, states[2], () => HasSwim());
			animator.AddAnyState(trigger, states[1], () => HasShield());
			animator.AddAnyState(trigger, states[0]);
		}

		animator.AddAnyState("Idle", enterLowHealthSwimShield, () => IsLowHealth() && HasSwim() && HasShield());
		animator.AddAnyState("Idle", enterLowHealthSwim, () => IsLowHealth() && HasSwim());
		animator.AddAnyState("Idle", enterLowHealthShield, () => IsLowHealth() && HasShield());
		animator.AddAnyState("Idle", enterLowHealthNone, IsLowHealth);
		AddBranches("Idle",   new[] { idleNone, idleShield, idleSwim, idleSwimShield });
		AddBranches("Attack", attacks);
		AddBranches("Swing",  swings);
		AddBranches("Hit",    hurts);
		AddBranches("Cast",   casts);
		AddBranches("Dead",   dies);
		AddBranches("Reload", reloads);
		AddBranches("Charge", charges);

		animator.AddAnyState("ShieldUp", shieldUpSwim, () => HasSwim());
		animator.AddAnyState("ShieldUp", shieldUp);

		return animator;
	}
}
