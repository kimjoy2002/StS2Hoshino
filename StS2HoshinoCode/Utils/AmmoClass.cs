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
	private class PlayerAmmoState
	{
		public int CurrentAmmo = 4;
		public int MaxAmmo = 4;

		public int AmmoUsedThisTurn;
		public int MenualedReloadedThisTurn;
		public int InvadesThisCombat;
		public int ReloadedThisCombat;
		public int Slot3UsedThisCombat;
		public int Slot4UsedThisCombat;

		public CardModel? LastCardPlayed;

		public bool IsLastShot;

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

	public static int GetCurrentAmmo(Player? player)
	{
		return GetState(player).CurrentAmmo;
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

	public static void QueueCountdownTrigger(Player player, Func<PlayerChoiceContext, Task> trigger)
	{
		GetState(player).PendingTriggers.Add(trigger);
	}


	public static bool isEmptyAmmo(Player player)
	{
		return !hasAmmo(1, player);
	}

	public static bool hasAmmo(int amount, Player player)
	{
		if (amount <= 0)
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
		state.AmmoUsedThisTurn = 0;
		state.MenualedReloadedThisTurn = 0;
		state.InvadesThisCombat = 0;
		state.ReloadedThisCombat = 0;
		state.Slot3UsedThisCombat = 0;
		state.Slot4UsedThisCombat = 0;
		state.IsLastShot = false;
		state.PendingTriggers.Clear();
	}
}