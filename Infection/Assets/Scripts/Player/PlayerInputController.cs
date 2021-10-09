using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    [Header("Mouse Cursor Settings")] 
    [SerializeField] private bool cursorLocked = true;
		
    [Header("Character Input Values")]
    public Vector2 move;
    public bool jump;
    public bool interact;
    public bool pause;

    [Header("References")] 
    public PlayerInput playerInput;

    private void Start()
    {
        if (cursorLocked)
        {
            // Disable the mouse cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
		
    #region Public Event Handlers
		
    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        jump = context.ReadValueAsButton();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        interact = context.ReadValueAsButton();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        pause = context.ReadValueAsButton();
    }
		
    #endregion
}
