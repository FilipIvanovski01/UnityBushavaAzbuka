using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Pointer from <see cref="Touchscreen"/> when touching, otherwise <see cref="Mouse"/>.
/// Implements <see cref="IInputHandler"/> without legacy <c>UnityEngine.Input</c>.
/// </summary>
public sealed class InputSystemPointerHandler : IInputHandler
{
    public bool PointerDownThisFrame
    {
        get
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
                return true;

            return Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        }
    }

    public bool PointerUpThisFrame
    {
        get
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
                return true;

            return Mouse.current != null && Mouse.current.leftButton.wasReleasedThisFrame;
        }
    }

    public bool PointerHeld
    {
        get
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                return true;

            return Mouse.current != null && Mouse.current.leftButton.isPressed;
        }
    }

    public Vector2 PointerPosition
    {
        get
        {
            Touchscreen ts = Touchscreen.current;
            if (ts != null)
            {
                var press = ts.primaryTouch.press;
                if (press.isPressed || press.wasReleasedThisFrame)
                    return ts.primaryTouch.position.ReadValue();
            }

            return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        }
    }
}
