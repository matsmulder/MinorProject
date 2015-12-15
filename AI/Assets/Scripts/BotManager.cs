using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class BotManager
{
    private static Calculator Cal = UnityEngine.Object.Instantiate();

    public static Calculator getCalculator()
    {
        return Cal;
    }    
}
