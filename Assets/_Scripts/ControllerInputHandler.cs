using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ControllerInputHandler : MonoBehaviour
{
    [Header ("Input Action")]
    public InputActionProperty rightGripInput;
    public InputActionProperty leftGripInput;

    [Header("Event Action")]
    public UnityEvent whenRightGripPressed;
    public UnityEvent whenLeftGripPressed;

    private void Update()
    {
        if (leftGripInput.action.IsPressed())
        {
            whenLeftGripPressed.Invoke();
        }

        if (rightGripInput.action.IsPressed())
        {
            whenRightGripPressed.Invoke();
        }
    }
}