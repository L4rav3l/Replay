using System;
using Microsoft.Xna.Framework.Input;

namespace Replay;

public static class GameData
{
    public static KeyboardState previous {get;set;}
    public static bool Exit {get;set;}
}