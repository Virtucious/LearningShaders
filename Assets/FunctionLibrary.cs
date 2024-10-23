using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

public static class FunctionLibrary
{
    public delegate float Function(float x, float z, float t);
    public enum FunctionName { Wave, MultiWave, Ripple, DiagonalWave, HillyWave};
    static Function[] functions = { Wave, MultiWave, Ripple, DiagonalWave, HillyWave};

    public static Function GetFunction(FunctionName name)
    {
        return functions[(int)name];
    }

    public static float Wave(float x, float z, float t)
    {
        return Sin(PI * (x + t));
    }

    public static float MultiWave(float x, float z, float t)
    {
        float y = Sin(PI * (x + t));
        y += Sin(2 * PI * (x + t)) * 0.5f;
        return y * (2f/3f);
    }

    public static float Ripple(float x, float z, float t)
    {
        float d = Sqrt(x * x + z * z);
        float y = Sin(PI * (4f * d - t)) / (1f + 10f*d);
        return y;
    }

    public static float DiagonalWave(float x, float z, float t)
    {
        return Sin(PI * (x + z + t));
    }

    public static float HillyWave(float x, float z, float t)
    {
        float y = Sin(PI * (x + 0.5f * t));
        y += 0.5f * Sin(2f * PI * (z + t));
        y += Sin(PI * (x + z + 0.25f * t));
        return y * 1f / 2.5f;
    }

   
}