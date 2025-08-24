using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    // 매니저 보다는 UIPause, UISetting 의 역할
    [Header("UI")]
    public Animator MenuAnimator;

    public bool isMenuOpen = false;

    public Animator _potal;

    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        //AudioManager.Instance.AudioSliders(bgmSlider, sfxSlider);
    }

    public void OnEsc(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        isMenuOpen = !isMenuOpen;

        if (isMenuOpen)
        {
            Debug.Log("ON");
            MenuAnimator.SetTrigger("Open");
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Debug.Log("OFF");
            MenuAnimator.SetTrigger("Close");
            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OnCancel()
    {
        isMenuOpen = !isMenuOpen;

        AudioManager.Instance.PlaySFX("SFX_Button");
        Debug.Log("OFF");
        MenuAnimator.SetTrigger("Close");
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PotalA()
    {
        _potal.SetTrigger("blue");
        AudioManager.Instance.PlaySFX("SFX_Portal_Out");
    }
    public void PotalB()
    {
        _potal.SetTrigger("orange");
        AudioManager.Instance.PlaySFX("SFX_Portal_Out");
    }
}
