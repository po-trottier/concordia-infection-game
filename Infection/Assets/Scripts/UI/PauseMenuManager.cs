using Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject canvas;
    [SerializeField] private PlayerInputManager inputManager;

    public UnityEvent<bool> gamePaused;
    
    private void Start()
    {
        ResumeGame();
        canvas.SetActive(false);
        
        inputManager.reference.actions[ActionTypes.Pause].performed += ToggleMenu;
    }

    private void ToggleMenu(InputAction.CallbackContext _)
    {
        if (canvas != null)
            canvas.SetActive(!canvas.activeSelf);

        // If the menu is visible
        if (canvas.activeSelf)
            PauseGame();
        else
            ResumeGame();
        
        gamePaused.Invoke(canvas.activeSelf);
    }

    private void PauseGame()
    {
        // Enable the mouse cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
            
        // Freeze the game
        Time.timeScale = 0f;
    }

    private void ResumeGame()
    {
        // Disable the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;       
            
        // Unfreeze the game
        Time.timeScale = 1f;
    }
}
