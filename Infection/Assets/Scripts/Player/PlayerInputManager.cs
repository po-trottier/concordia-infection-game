using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputManager : MonoBehaviour
{
    [Header("Mouse Cursor Settings")] 
    [SerializeField] private bool cursorLocked = true;
		
    [Header("Character Input Values")]
    [Tooltip("Player movement input")]
    public Vector2 move;
    [Tooltip("Player jump input")]
    public bool jump;
    [Tooltip("Distribute mask input")]
    public bool mask;
    [Tooltip("Distribute vaccine input")]
    public bool vaccinate;
    [Tooltip("Crowds control input")]
    public bool cc;
    [Tooltip("Clean control input")]
    public bool clean;
    [Tooltip("Toggle pause menu input")]
    public bool pause;

    [Header("References")] 
    public PlayerInput reference;

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

    public void OnMask(InputAction.CallbackContext context)
    {
        mask = context.ReadValueAsButton();
    }

    public void OnVaccinate(InputAction.CallbackContext context)
    {
        vaccinate = context.ReadValueAsButton();
    }

    public void OnCC(InputAction.CallbackContext context)
    {
        cc = context.ReadValueAsButton();
    }

    public void OnClean(InputAction.CallbackContext context)
    {
        clean = context.ReadValueAsButton();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        pause = context.ReadValueAsButton();
    }
		
    #endregion
}
