using SappUnityUtils.Numbers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Vector2Double
{
    public double x;
    public double y;

    public Vector2Double(double x, double y)
    {
        this.x = x;
        this.y = y;
    }

    public double sqrMagnitude => x * x + y * y;
    public double magnitude => Math.Sqrt(x * x + y * y);

    public static bool operator ==(Vector2Double a, Vector2Double b)
    {
        return a.x == b.x
            && a.y == b.y;
    }

    public static bool operator !=(Vector2Double a, Vector2Double b)
    {
        return a.x != b.x
            || a.y != b.y;
    }

    public static Vector2Double operator +(Vector2Double a, Vector2Double b)
    {
        return new Vector2Double(a.x + b.x, a.y + b.y);
    }

    public static Vector2Double operator -(Vector2Double a, Vector2Double b)
    {
        return new Vector2Double(a.x - b.x, a.y - b.y);
    }

    public static Vector2Double operator /(Vector2Double a, double scalar)
    {
        return new Vector2Double(a.x / scalar, a.y / scalar);
    }

    public static Vector2Double operator -(Vector2Double a)
    {
        return new Vector2Double(-a.x, -a.y);
    }

    public static Vector2Double Cross(Vector2Double a, Vector2Double b)
    {
        return new Vector2Double(a.x * b.y, a.y * b.x);
    }

    public static Vector2Double operator *(Vector2Double a, double scalar)
    {
        return new Vector2Double(a.x * scalar, a.y * scalar);
    }

    public static double Distance(Vector2Double a, Vector2Double b)
    {
        return Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y));
    }
}
