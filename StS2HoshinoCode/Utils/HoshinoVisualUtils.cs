using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using StS2Hoshino.StS2HoshinoCode.Extensions;

namespace StS2Hoshino.StS2HoshinoCode.Utils;

public static class HoshinoVisualUtils
{
    private static float _movingPos = 120f;
    private const string BarrierFloorName = "HoshinoBarrierFloor";
    private const string BarrierOutlineName = "HoshinoBarrierOutline";
    private const string ShieldPersistentName = "HoshinoShieldPersistent";

    public static void ApplyBarrierVisual(Creature creature)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node == null) return;
        
        var visuals = node.Visuals;
        if (visuals == null) return;

        if (visuals.HasNode(BarrierFloorName)) return;

        visuals.SetMeta("HoshinoBarrierOriginalX", visuals.Position.X);

        float moveOffset = GetMoveOffset(creature);
        var moveTween = visuals.CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        moveTween.TweenProperty(visuals, "position:x", visuals.Position.X + moveOffset, 0.4);

        // barrier_floor (Bottom)
        var floor = new Sprite2D();
        floor.Texture = ResourceLoader.Load<Texture2D>("barrier_floor.png".CharacterUiPath());
        floor.Name = BarrierFloorName;
        floor.ZIndex = 0;
        floor.Position = new Vector2(0, -100); 
        visuals.AddChild(floor);
        visuals.MoveChild(floor, 0);

        // barrier_outline (Top)
        var outline = new Sprite2D();
        outline.Texture = ResourceLoader.Load<Texture2D>("barrier_outline.png".CharacterUiPath());
        outline.Name = BarrierOutlineName;
        outline.ZIndex = 1;
        outline.Position = new Vector2(0, -100); 
        visuals.AddChild(outline);
    }

    public static void RemoveBarrierVisual(Creature creature)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node == null) return;

        var visuals = node.Visuals;
        if (visuals != null)
        {
            if (visuals.HasNode(BarrierFloorName))
            {
                // 저장된 원래 위치로 복구
                float originalX = (float)visuals.GetMeta("HoshinoBarrierOriginalX", 0f);
                var moveTween = visuals.CreateTween().SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
                moveTween.TweenProperty(visuals, "position:x", originalX, 0.4);
            }

            visuals.GetNodeOrNull(BarrierFloorName)?.QueueFree();
            visuals.GetNodeOrNull(BarrierOutlineName)?.QueueFree();
        }
    }

    public static void ApplyShieldVisualPersistent(Creature creature)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node == null) return;

        var visuals = node.Visuals;
        if (visuals == null) return;

        if (visuals.HasNode(ShieldPersistentName)) return;

        var shield = new Sprite2D();
        shield.Name = ShieldPersistentName;
        shield.Texture = ResourceLoader.Load<Texture2D>("shield.png".CharacterUiPath());
        shield.Position = new Vector2(100, -80); 
        shield.ZIndex = 2;
        visuals.AddChild(shield);
    }

    public static void RemoveShieldVisualPersistent(Creature creature)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        var visuals = node?.Visuals;
        visuals?.GetNodeOrNull(ShieldPersistentName)?.QueueFree();
    }

    private static float GetMoveOffset(Creature creature)
    {
        if (creature.Side != CombatSide.Player) return -_movingPos;
        if (LocalContext.IsMe(creature)) return _movingPos;

        // 아군일 경우: 로컬 플레이어(호시노)의 현재 위치보다 더 앞으로 이동
        try
        {
            var me = LocalContext.GetMe(creature.CombatState);
            var myNode = NCombatRoom.Instance?.GetCreatureNode(me?.Creature);
            var targetNode = NCombatRoom.Instance?.GetCreatureNode(creature);

            if (myNode != null && targetNode != null)
            {
                float myVisualX = myNode.Position.X + myNode.Visuals.Position.X;
                float targetBaseX = targetNode.Position.X;

                float targetVisualX = myVisualX + 180f;
                return targetVisualX - targetBaseX;
            }
        }
        catch
        {
        }

        return _movingPos * 3f;
    }
}
