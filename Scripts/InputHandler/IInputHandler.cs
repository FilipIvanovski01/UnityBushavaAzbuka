using UnityEngine;

public interface IInputHandler
{
    bool PointerDownThisFrame { get; }
    bool PointerUpThisFrame { get; }
    bool PointerHeld { get; }
    Vector2 PointerPosition { get; }
}
