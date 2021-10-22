using Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private PlayerInputController inputController;

    public UnityEvent<bool> gamePaused;
    
    private void Start()
    {
        canvas.SetActive(false);
        
        inputController.playerInput.actions[ActionTypes.Pause].performed += ToggleMenu;
    }

    private void ToggleMenu(InputAction.CallbackContext _)
    {
        canvas.SetActive(!canvas.activeSelf);

        gamePaused.Invoke(canvas.activeSelf);
        
        // If the menu is visible
        if (canvas.activeSelf)
        {
            // Enable the mouse cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // Freeze the game
            Time.timeScale = 0f;
        }
        else
        {
            // Disable the mouse cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;       
            
            // Unfreeze the game
            Time.timeScale = 1f;
        }
    }
}
