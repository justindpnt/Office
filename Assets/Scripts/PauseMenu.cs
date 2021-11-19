using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    //Player settings pointer
    public PlayerSettings settings;

    //Menu State
    bool menuOpen = false;

    // Cache
    public GameObject menu;
    public PlayerController controller;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
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

    public void SetMouseSens(float sensitivity)
    {
        controller.updateMouseSensMultiplier(sensitivity);
    }

    private void openMenu()
    {
        menu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        controller.canLook = false;
        controller.canMove = false;
    }

    private void closeMenu()
    {
        menu.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        controller.canLook = true;
        controller.canMove = true;
        controller.updateMouseSensMultiplier(settings.mouseSensetivity);
    }
}
