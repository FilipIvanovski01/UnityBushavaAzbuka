using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static IInputHandler InputHandler
    {
        get
        {
            if (inputHandler == null)
                inputHandler = new InputSystemPointerHandler();

            return inputHandler;
        }
    }

    private static IInputHandler inputHandler;

    private void Awake()
    {
        inputHandler = new InputSystemPointerHandler();
    }
}
