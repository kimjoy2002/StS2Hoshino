using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using StS2Hoshino.StS2HoshinoCode.Extensions;

namespace StS2Hoshino.StS2HoshinoCode.Utils;

public static class HoshinoVisualUtils
{
    private static float _movingPos = 120f;
    public static void ApplyBarrierVisual(Creature creature)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node == null) return;

        if (node.HasNode("HoshinoBarrierFloor")) return;

        float moveOffset = (creature.Side == CombatSide.Player) ? _movingPos : -_movingPos;
        var moveTween = node.CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        moveTween.TweenProperty(node, "position:x", node.Position.X + moveOffset, 0.4);

        // barrier_floor (Bottom)
        var floor = new Sprite2D();
        floor.Texture = ResourceLoader.Load<Texture2D>("barrier_floor.png".CharacterUiPath());
        floor.Name = "HoshinoBarrierFloor";
        floor.ZIndex = 0;
        floor.Position = new Vector2(0, -100); 
        node.AddChild(floor);
        node.MoveChild(floor, 0);

        // barrier_outline (Top)
        var outline = new Sprite2D();
        outline.Texture = ResourceLoader.Load<Texture2D>("barrier_outline.png".CharacterUiPath());
        outline.Name = "HoshinoBarrierOutline";
        outline.ZIndex = 1;
        outline.Position = new Vector2(0, -100); 
        node.AddChild(outline);
    }

    public static void RemoveBarrierVisual(Creature creature)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node == null) return;

        if (node.HasNode("HoshinoBarrierFloor"))
        {
            float moveOffset = (creature.Side == CombatSide.Player) ? _movingPos : -_movingPos;
            var moveTween = node.CreateTween().SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
            moveTween.TweenProperty(node, "position:x", node.Position.X - moveOffset, 0.4);
        }

        node.GetNodeOrNull("HoshinoBarrierFloor")?.QueueFree();
        node.GetNodeOrNull("HoshinoBarrierOutline")?.QueueFree();
    }

    public static void ApplyShieldVisualPersistent(Creature creature)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (node == null) return;

        if (node.HasNode("HoshinoShieldPersistent")) return;

        var shield = new Sprite2D();
        shield.Name = "HoshinoShieldPersistent";
        shield.Texture = ResourceLoader.Load<Texture2D>("shield.png".CharacterUiPath());
        shield.Position = new Vector2(100, -80); 
        shield.ZIndex = 2;
        node.AddChild(shield);
    }

    public static void RemoveShieldVisualPersistent(Creature creature)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(creature);
        node?.GetNodeOrNull("HoshinoShieldPersistent")?.QueueFree();
    }
}
