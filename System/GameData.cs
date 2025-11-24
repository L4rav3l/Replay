using System;
using Microsoft.Xna.Framework.Input;

namespace Replay;

public static class GameData
{
    public static KeyboardState previous {get;set;}
    public static bool Exit {get;set;}
    public static bool Move {get;set;}
    public static bool Hand {get;set;}
    public static bool Wood {get;set;}
    public static bool Honey {get;set;}
    public static bool Mushroom {get;set;}
    public static bool Key {get;set;}
    public static float Speed {get;set;}
    public static int Break {get;set;}
}