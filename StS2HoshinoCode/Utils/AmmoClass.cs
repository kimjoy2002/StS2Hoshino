using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using StS2Hoshino.StS2HoshinoCode.Hook;

namespace StS2Hoshino.StS2HoshinoCode.Utils;

public static class AmmoClass
{
	internal sealed class CombatSnapshot
	{
		internal readonly Dictionary<Player, PlayerSnapshot> Players;

		internal CombatSnapshot(Dictionary<Player, PlayerSnapshot> players)
		{
			Players = players;
		}
	}

	internal sealed class PlayerSnapshot
	{
		internal int CurrentAmmo;
		internal int MaxAmmo;
		internal bool IsActive;
		internal int AmmoUsedThisTurn;
		internal int MenualedReloadedThisTurn;
		internal int InvadesThisCombat;
		internal int ReloadedThisCombat;
		internal int Slot3UsedThisCombat;
		internal int Slot4UsedThisCombat;
		internal CardModel? LastCardPlayed;
		internal bool IsLastShot;
		internal int ResolvingCardPlayDepth;
	}

	private class PlayerAmmoState
	{
		public int CurrentAmmo = 4;
		public int MaxAmmo = 4;
		public bool IsActive;

		public int AmmoUsedThisTurn;
		public int MenualedReloadedThisTurn;
		public int InvadesThisCombat;
		public int ReloadedThisCombat;
		public int Slot3UsedThisCombat;
		public int Slot4UsedThisCombat;

		public CardModel? LastCardPlayed;

		public bool IsLastShot;
		public int ResolvingCardPlayDepth;

		public readonly List<Func<PlayerChoiceContext, Task>> PendingTriggers = new();
	}

	private static int _defaultMaxAmmo = 4;

	private static readonly Dictionary<Player, PlayerAmmoState> _states = new Dictionary<Player, PlayerAmmoState>();

	private static readonly PlayerAmmoState _defaultState = new PlayerAmmoState();

	public static Player? CurrentAmmoGainer { get; private set; }


	public static event Action<int>? OnAmmoGained;

	public static event Action<int>? OnAmmoUsed;

	public static event Action? OnReload;
	public static event Action<Player, int, int>? OnChanged;

	private static PlayerAmmoState GetState(Player? player)
	{
		if (player == null)
		{
			return _defaultState;
		}
		if (!_states.TryGetValue(player, out PlayerAmmoState? value))
		{
			value = new PlayerAmmoState();
			_states[player] = value;
		}
		return value;
	}

	internal static CombatSnapshot CaptureCombatSnapshot(IEnumerable<Player> players)
	{
		Dictionary<Player, PlayerSnapshot> snapshots = new();
		foreach (Player player in players.Distinct())
		{
			PlayerAmmoState state = GetState(player);
			snapshots[player] = new PlayerSnapshot
			{
				CurrentAmmo = state.CurrentAmmo,
				MaxAmmo = state.MaxAmmo,
				IsActive = state.IsActive,
				AmmoUsedThisTurn = state.AmmoUsedThisTurn,
				MenualedReloadedThisTurn = state.MenualedReloadedThisTurn,
				InvadesThisCombat = state.InvadesThisCombat,
				ReloadedThisCombat = state.ReloadedThisCombat,
				Slot3UsedThisCombat = state.Slot3UsedThisCombat,
				Slot4UsedThisCombat = state.Slot4UsedThisCombat,
				LastCardPlayed = state.LastCardPlayed,
				IsLastShot = state.IsLastShot,
				ResolvingCardPlayDepth = state.ResolvingCardPlayDepth
			};
		}

		return new CombatSnapshot(snapshots);
	}

	internal static void RestoreCombatSnapshot(CombatSnapshot snapshot)
	{
		CurrentAmmoGainer = null;
		foreach ((Player player, PlayerSnapshot saved) in snapshot.Players)
		{
			PlayerAmmoState state = GetState(player);
			state.CurrentAmmo = saved.CurrentAmmo;
			state.MaxAmmo = saved.MaxAmmo;
			state.IsActive = saved.IsActive;
			state.AmmoUsedThisTurn = saved.AmmoUsedThisTurn;
			state.MenualedReloadedThisTurn = saved.MenualedReloadedThisTurn;
			state.InvadesThisCombat = saved.InvadesThisCombat;
			state.ReloadedThisCombat = saved.ReloadedThisCombat;
			state.Slot3UsedThisCombat = saved.Slot3UsedThisCombat;
			state.Slot4UsedThisCombat = saved.Slot4UsedThisCombat;
			state.LastCardPlayed = saved.LastCardPlayed;
			state.IsLastShot = saved.IsLastShot;
			state.ResolvingCardPlayDepth = saved.ResolvingCardPlayDepth;

			// These delegates belong to the abandoned action timeline, not the snapshot.
			state.PendingTriggers.Clear();
			OnChanged?.Invoke(player, state.CurrentAmmo, state.MaxAmmo);

			if (player.PlayerCombatState?.AllPiles == null)
			{
				continue;
			}

			foreach (CardModel card in player.PlayerCombatState.AllPiles.SelectMany(pile => pile.Cards).Distinct())
			{
				card.InvokeEnergyCostChanged();
			}
		}
	}

	public static int GetCurrentAmmo(Player? player)
	{
		return GetState(player).CurrentAmmo;
	}

	public static bool IsActive(Player? player)
	{
		return GetState(player).IsActive;
	}

	public static void SetActive(Player? player, bool active)
	{
		PlayerAmmoState state = GetState(player);
		if (state.IsActive == active)
		{
			return;
		}

		state.IsActive = active;
		if (player != null)
		{
			OnChanged?.Invoke(player, state.CurrentAmmo, state.MaxAmmo);
		}
	}

	private static void EnsureActive(Player player)
	{
		SetActive(player, true);
	}

	public static int SetMaxAmmo(Player? player, int max_ammo)
	{
		if (max_ammo > 10)
		{
			max_ammo = 10;
		}
		PlayerAmmoState state = GetState(player);
		state.MaxAmmo = max_ammo;
		if (state.CurrentAmmo > max_ammo)
		{
			state.CurrentAmmo = max_ammo;
		}

		if (player != null)
		{
			AmmoClass.OnChanged?.Invoke(player, state.CurrentAmmo, state.MaxAmmo);
		}
		return GetMaxAmmo(player);
	}
	public static int GetMaxAmmo(Player? player)
	{
		return GetState(player).MaxAmmo;
	}
	
	public static void DoingReload(Player? player, bool isButton)
	{
		if (isButton)
		{
			GetState(player).MenualedReloadedThisTurn++;
		}
		GetState(player).ReloadedThisCombat++;
	}


	public static int getReloadCount(Player? player)
	{
		return GetState(player).MenualedReloadedThisTurn;
	}

	public static int GetReloadCountThisCombat(Player? player)
	{
		return GetState(player).ReloadedThisCombat;
	}

	public static int GetAmmoUsedThisTurn(Player? player)
	{
		return GetState(player).AmmoUsedThisTurn;
	}


	public static CardModel? GetLastCardPlayed(Player? player)
	{
		return GetState(player).LastCardPlayed;
	}

	public static void SetLastCardPlayed(Player player, CardModel? card)
	{
		GetState(player).LastCardPlayed = card;
	}

	public static int GetInvadeCount(Player? player)
	{
		return GetState(player).InvadesThisCombat;
	}

	public static void AddInvadeCount(Player? player)
	{
		GetState(player).InvadesThisCombat++;
	}

	public static int GetSlot3UsedCount(Player? player)
	{
		return GetState(player).Slot3UsedThisCombat;
	}

	public static int GetSlot4UsedCount(Player? player)
	{
		return GetState(player).Slot4UsedThisCombat;
	}

	public static bool GetIsLastShot(Player? player)
	{
		return GetState(player).IsLastShot;
	}

	public static void SetIsLastShot(Player? player, bool value)
	{
		GetState(player).IsLastShot = value;
	}

	public static bool GetIsResolvingCardPlay(Player? player)
	{
		return GetState(player).ResolvingCardPlayDepth > 0;
	}

	public static void SetIsResolvingCardPlay(Player? player, bool value)
	{
		PlayerAmmoState state = GetState(player);
		if (value)
		{
			state.ResolvingCardPlayDepth++;
		}
		else if (state.ResolvingCardPlayDepth > 0)
		{
			state.ResolvingCardPlayDepth--;
		}
	}

	public static void QueueCountdownTrigger(Player player, Func<PlayerChoiceContext, Task> trigger)
	{
		GetState(player).PendingTriggers.Add(trigger);
	}


	public static bool isEmptyAmmo(Player player)
	{
		if (!IsActive(player))
		{
			return true;
		}

		return !hasAmmo(1, player);
	}

	public static bool hasAmmo(int amount, Player player)
	{
		if (amount <= 0)
		{
			return true;
		}
		if (!IsActive(player))
		{
			return true;
		}
		PlayerAmmoState state = GetState(player);
		if (state.CurrentAmmo >= amount)
		{
			return true;
		}
		return false;
	}

	public static async Task SetAmmo(PlayerChoiceContext choiceContext, int amount, bool reload, Player player)
	{
		PlayerAmmoState state = GetState(player);
		EnsureActive(player);
		CurrentAmmoGainer = player;
		int prevAmmo = state.CurrentAmmo;
		state.CurrentAmmo = amount;
		if (state.CurrentAmmo > state.MaxAmmo)
		{
			state.CurrentAmmo = state.MaxAmmo;
		}

		if (amount - prevAmmo > 0)
		{
			AmmoClass.OnAmmoGained?.Invoke(amount);
			if (reload)
			{
				AmmoClass.OnReload?.Invoke();
			}

			AmmoClass.OnChanged?.Invoke(player, state.CurrentAmmo, state.MaxAmmo);
		}

		if (amount != prevAmmo)
		{
			await HoshinoHook.OnBulletChanged(choiceContext, player, prevAmmo, amount);
			AmmoClass.OnChanged?.Invoke(player, state.CurrentAmmo, state.MaxAmmo);
		}

		CurrentAmmoGainer = null;
	}

	public static async Task LoseAmmo(PlayerChoiceContext choiceContext, int amount, Player player)
	{
		if (amount > 0)
		{
			PlayerAmmoState state = GetState(player);
			EnsureActive(player);
			StS2HoshinoMain.Logger.Info($"Lost ammo {amount} - {state.CurrentAmmo}/{state.MaxAmmo}");
			CurrentAmmoGainer = player;
			int prev_ammo = state.CurrentAmmo;
			state.CurrentAmmo -= amount;
			if (state.CurrentAmmo < 0)
			{
				amount += state.CurrentAmmo;
				state.CurrentAmmo = 0;
			}
			if (amount > 0)
			{
				for (int i = prev_ammo; i > state.CurrentAmmo; i--)
				{
					if (i == state.MaxAmmo - 2) state.Slot3UsedThisCombat++;
					if (i == state.MaxAmmo - 3) state.Slot4UsedThisCombat++;
				}
				AmmoClass.OnAmmoUsed?.Invoke(amount);
				await HoshinoHook.OnBulletChanged(choiceContext, player, prev_ammo, state.CurrentAmmo);
				AmmoClass.OnChanged?.Invoke(player, state.CurrentAmmo, state.MaxAmmo);
			}
			CurrentAmmoGainer = null;
		}
	}

	public static void ResetForTurnStart(Player player)
	{
		PlayerAmmoState state = GetState(player);
		state.AmmoUsedThisTurn = 0;
		state.MenualedReloadedThisTurn = 0;
		AmmoClass.OnChanged?.Invoke(player, state.CurrentAmmo, state.MaxAmmo);
	}


	public static void ResetFull(Player player)
	{
		PlayerAmmoState state = GetState(player);
		state.CurrentAmmo = _defaultMaxAmmo;
		state.MaxAmmo = _defaultMaxAmmo;
		state.IsActive = player.Character is StS2Hoshino.StS2HoshinoCode.Character.StS2Hoshino;
		state.AmmoUsedThisTurn = 0;
		state.MenualedReloadedThisTurn = 0;
		state.InvadesThisCombat = 0;
		state.ReloadedThisCombat = 0;
		state.Slot3UsedThisCombat = 0;
		state.Slot4UsedThisCombat = 0;
		state.IsLastShot = false;
		state.ResolvingCardPlayDepth = 0;
		state.PendingTriggers.Clear();
	}
}
