using UnityEngine;
using System.Collections;

public static class UIExtensions
{
    public static void NameComponent(this CustomSpriteAnimation spriteAnimation, string name)
    {
        // Implementation goes here
        spriteAnimation.name = name;
    }
}