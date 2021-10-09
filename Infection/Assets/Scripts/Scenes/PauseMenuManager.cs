using Common;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private PlayerInputController inputController;
    
    private void Start()
    {
        inputController.playerInput.actions[ActionTypes.Pause].performed += ToggleMenu;
    }

    private void ToggleMenu(InputAction.CallbackContext _)
    {
        canvas.SetActive(!canvas.activeSelf);
        
        // If the menu is visible
        if (canvas.activeSelf)
        {
            // Enable the mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Disable the mouse cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;            
        }
    }
}
