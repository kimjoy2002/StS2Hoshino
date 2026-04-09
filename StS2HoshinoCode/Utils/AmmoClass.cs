using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace StS2Hoshino.StS2HoshinoCode.Utils;

public static class AmmoClass
{
	private class PlayerAmmoState
	{
		public int CurrentAmmo = 4;
		public int MaxAmmo = 4;

		public int AmmoUsedThisTurn;


		public CardModel? LastCardPlayed;
	}

	private static int _defaultMaxAmmo = 4;

	private static readonly Dictionary<Player, PlayerAmmoState> _states = new Dictionary<Player, PlayerAmmoState>();

	private static readonly PlayerAmmoState _defaultState = new PlayerAmmoState();

	private static readonly List<Func<PlayerChoiceContext, Task>> _pendingTriggers = new List<Func<PlayerChoiceContext, Task>>();

	public static Player? CurrentAmmoGainer { get; private set; }


	public static event Action<int>? OnAmmoGained;

	public static event Action<int>? OnAmmoUsed;

	public static event Action? OnReload;
	public static event Action<int, int>? OnChanged;

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
		GetState(player).MaxAmmo = max_ammo;
		if (GetState(player).CurrentAmmo > max_ammo)
		{
			GetState(player).CurrentAmmo = max_ammo;
		}

		return GetMaxAmmo(player);
	}
	public static int GetMaxAmmo(Player? player)
	{
		return GetState(player).MaxAmmo;
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

	public static void QueueCountdownTrigger(Func<PlayerChoiceContext, Task> trigger)
	{
		_pendingTriggers.Add(trigger);
	}

	public static async Task ProcessPendingTriggers(PlayerChoiceContext context)
	{
		if (_pendingTriggers.Count == 0)
		{
			return;
		}
		List<Func<PlayerChoiceContext, Task>> list = new List<Func<PlayerChoiceContext, Task>>(_pendingTriggers);
		_pendingTriggers.Clear();
		foreach (Func<PlayerChoiceContext, Task> item in list)
		{
			await item(context);
		}
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

	public static int GainAmmo(int amount, bool reload, Player player)
	{
		if (amount <= 0)
		{
			return 0;
		}
		PlayerAmmoState state = GetState(player);
		CurrentAmmoGainer = player;
		state.CurrentAmmo += amount;
		int num = 0;
		if (state.CurrentAmmo > state.MaxAmmo)
		{
			state.CurrentAmmo = state.MaxAmmo;
		}

		if (amount > 0)
		{
			AmmoClass.OnAmmoGained?.Invoke(amount);
			if (reload)
			{
				AmmoClass.OnReload?.Invoke();
			}

			AmmoClass.OnChanged?.Invoke(state.CurrentAmmo, state.MaxAmmo);
		}

		CurrentAmmoGainer = null;
		return num;
	}

	public static int LoseAmmo(int amount, Player player)
	{
		if (amount > 0)
		{
			PlayerAmmoState state = GetState(player);
			StS2HoshinoMain.Logger.Info($"Lost ammo {amount} - {state.CurrentAmmo}/{state.MaxAmmo}");
			CurrentAmmoGainer = player;
			state.CurrentAmmo -= amount;
			if (state.CurrentAmmo < 0)
			{
				amount += state.CurrentAmmo;
				state.CurrentAmmo = 0;
			}
			if (amount > 0)
			{
				AmmoClass.OnAmmoUsed?.Invoke(amount);
				AmmoClass.OnChanged?.Invoke(state.CurrentAmmo, state.MaxAmmo);;
			}
			CurrentAmmoGainer = null;
		}

		return amount;
	}

	public static void ResetForTurnStart(Player player)
	{
		PlayerAmmoState state = GetState(player);
		state.AmmoUsedThisTurn = 0;
		AmmoClass.OnChanged?.Invoke(state.CurrentAmmo, state.MaxAmmo);
	}


	public static void ResetFull(Player player)
	{
		PlayerAmmoState state = GetState(player);
		state.CurrentAmmo = _defaultMaxAmmo;
		state.MaxAmmo = _defaultMaxAmmo;
		state.AmmoUsedThisTurn = 0;
		_pendingTriggers.Clear();
	}
}