using System;
using System.Linq;
using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.TestSupport;

namespace StS2Hoshino.StS2HoshinoCode.Utils;

public static class CardSelectCmdCompat
{
    private static readonly MethodInfo? PushSelectorMethod = typeof(CardSelectCmd)
        .GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Where(method => method.Name == nameof(CardSelectCmd.PushSelector))
        .Where(method => typeof(IDisposable).IsAssignableFrom(method.ReturnType))
        .FirstOrDefault(method =>
        {
            ParameterInfo[] parameters = method.GetParameters();
            return parameters.Length is 1 or 2
                   && parameters[0].ParameterType == typeof(ICardSelector)
                   && (parameters.Length == 1 || parameters[1].ParameterType == typeof(bool));
        });

    public static IDisposable PushSelector(ICardSelector selector)
    {
        if (PushSelectorMethod == null)
        {
            throw new MissingMethodException(typeof(CardSelectCmd).FullName, nameof(CardSelectCmd.PushSelector));
        }

        object?[] args = PushSelectorMethod.GetParameters().Length == 1
            ? new object?[] { selector }
            : new object?[] { selector, false };

        try
        {
            return (IDisposable)PushSelectorMethod.Invoke(null, args)!;
        }
        catch (TargetInvocationException ex) when (ex.InnerException != null)
        {
            throw ex.InnerException;
        }
    }
}
