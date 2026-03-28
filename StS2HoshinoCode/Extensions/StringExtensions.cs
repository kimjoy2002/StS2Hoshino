using System.IO;
using StS2Hoshino.StS2HoshinoCode.Character;

namespace StS2Hoshino.StS2HoshinoCode.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    public static string ImagePath(this string path)
    {
        return Path.Join(Character.StS2Hoshino.ModId, "images", path);
    }

    public static string CardImagePath(this string path)
    {
        return Path.Join(Character.StS2Hoshino.ModId, "images", "card_portraits", path);
    }

    public static string PowerImagePath(this string path)
    {
        return Path.Join(Character.StS2Hoshino.ModId, "images", "powers", path);
    }

    public static string BigPowerImagePath(this string path)
    {
        return Path.Join(Character.StS2Hoshino.ModId, "images", "powers", "big", path);
    }

    public static string RelicImagePath(this string path)
    {
        return Path.Join(Character.StS2Hoshino.ModId, "images", "relics", path);
    }

    public static string BigRelicImagePath(this string path)
    {
        return Path.Join(Character.StS2Hoshino.ModId, "images", "relics", "big", path);
    }

    public static string CharacterUiPath(this string path)
    {
        return Path.Join(Character.StS2Hoshino.ModId, "images", "charui", path);
    }
    
    
    public static string ScencesPath(this string path)
    {
        return Path.Join(Character.StS2Hoshino.ModId, "scenes", path);
    }
}