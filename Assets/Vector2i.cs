using UnityEngine;
using System.Collections;

public struct Vector2i {

    public int x;
    public int y;

    public Vector2i(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static Vector2i zero {
        get {
            return new Vector2i(0, 0);
        }
    }

    public static Vector2i operator + (Vector2i a, Vector2i b)
    {
        return new Vector2i (a.x + b.x, a.y + b.y);
    }
}
