using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    //Player settings pointer
    PlayerSettings settings;

    //Menu State
    bool menuOpen = false;

    // Caches objects
    public GameObject menu;
    public Movement movementScript;

    //Slider
    public Slider slider;

    //Always have a fresh pointer to the one active game settings
    private void Awake()
    {
        settings = FindObjectOfType<PlayerSettings>() as PlayerSettings;
        slider.value = settings.mouseSensetivity;
        slider.onValueChanged.AddListener(delegate { settings.updateMouseSensitivity(slider.value); });
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!menuOpen)
            {
                menuOpen = true;
                openMenu();
            }
            else
            {
                menuOpen = false;
                closeMenu();
            }
        }
    }

    //Disable cursor and turn on menu UX
    private void openMenu()
    {
        menu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        movementScript.canLook = false;
        movementScript.canMove = false;
    }

    //Endable cursor, turn on menu UX, and writeback settings changes
    private void closeMenu()
    {
        updateGameValues();

        menu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        movementScript.canLook = true;
        movementScript.canMove = true;
    }

    //Writeback settings changes
    private void updateGameValues()
    {
        movementScript.updateMouseSensMultiplier(settings.mouseSensetivity);
    }
}
