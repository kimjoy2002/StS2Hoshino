using System.Collections.Generic;
using System.Reflection;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace StS2Hoshino.StS2HoshinoCode.Extensions;

public static class CardModelExtensions
{
    private static readonly FieldInfo? TagsField = typeof(CardModel)
        .GetField("_tags", BindingFlags.NonPublic | BindingFlags.Instance);
    
    public static void AddTagSafely(this CardModel card, CardTag tag)
    {
        var tags = new HashSet<CardTag>(card.Tags) { tag };
        TagsField?.SetValue(card, tags);
    }

    private static readonly MethodInfo? CreateDupeMainMethod = typeof(CardModel)
        .GetMethods()
        .FirstOrDefault(method => method.Name == nameof(CardModel.CreateDupe) && method.GetParameters().Length == 0);

    private static readonly MethodInfo? CreateDupeBetaMethod = typeof(CardModel)
        .GetMethods()
        .FirstOrDefault(method =>
            method.Name == nameof(CardModel.CreateDupe) &&
            method.GetParameters() is { Length: 1 } parameters &&
            parameters[0].ParameterType == typeof(Player));

    private static readonly MethodInfo? CreateCloneForPlayerMethod = typeof(CardModel)
        .GetMethods()
        .FirstOrDefault(method =>
            method.Name == "CreateCloneForPlayer" &&
            method.GetParameters() is { Length: 1 } parameters &&
            parameters[0].ParameterType == typeof(Player));

    private static readonly FieldInfo? OwnerField = typeof(CardModel)
        .GetField("_owner", BindingFlags.NonPublic | BindingFlags.Instance);

    public static CardModel CreateDupeCompat(this CardModel card)
    {
        if (CreateDupeMainMethod != null)
        {
            return (CardModel)CreateDupeMainMethod.Invoke(card, [])!;
        }

        if (CreateDupeBetaMethod != null)
        {
            return (CardModel)CreateDupeBetaMethod.Invoke(card, [card.Owner])!;
        }

        throw new MissingMethodException(typeof(CardModel).FullName, nameof(CardModel.CreateDupe));
    }

    public static CardModel CreateCloneForPlayerCompat(this CardModel card, Player player)
    {
        if (CreateCloneForPlayerMethod != null)
            return (CardModel)CreateCloneForPlayerMethod.Invoke(card, [player])!;

        CardModel clone = card.CreateClone();
        if (OwnerField == null)
            throw new MissingFieldException(typeof(CardModel).FullName, "_owner");

        OwnerField.SetValue(clone, player);
        return clone;
    }
}
