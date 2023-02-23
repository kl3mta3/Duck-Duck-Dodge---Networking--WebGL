using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Step
{
    public static bool Forward(ref float f, float step)
    {
        if (f < 0)
        {
            f = 0;
            return true;
        }

        if (f == 1)
            return false;

        f = f + step;
        if (f >= 1)
            f = 1;
        return true;
    }

    public static bool Backwards(ref float f, float step)
    {
        if (f > 1)
        {
            f = 1;
            return true;
        }

        if (f == 0)
            return false;

        f = f - step;
        if (f <= 0)
            f = 0;
        return true;
    }
}
