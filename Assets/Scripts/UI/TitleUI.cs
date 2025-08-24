using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class TitleUI : MonoBehaviour
{
    public Animator introAnimator;

    public UIManager uiManager;
    public PlayerInput playerInput;

    public Slider bgmSlider;
    public Slider sfxSlider;

    public GameObject hpgauge;
    public GameObject player;
    public GameObject intro;

    public Button continueButton;

    void Start()
    {
        AudioManager.Instance.PlayBGM("BGM_Title");
        //AudioManager.Instance.AudioSliders(bgmSlider, sfxSlider);

        Screen.SetResolution(1600, 900, true);


        StartCoroutine(CheckForSaveFile());
    }

    public void OnClickNewGame()
    {
        // 새 게임 데이터 저장
        SaveManager.Instance.NewGame();
        //게임 시작
        introAnimator.SetBool("NEW_GAME", true);
        StartCoroutine(GameStart());
    }

    private IEnumerator CheckForSaveFile()
    {
        // SaveManager 인스턴스가 로드될 때까지 한 프레임 기다립니다.
        yield return null;

        // 저장 파일이 존재하면 버튼을 활성화, 없으면 비활성화합니다.
        if (SaveManager.Instance.HasSaveFile)
        {
            continueButton.interactable = true;
        }
        else
        {
            continueButton.interactable = false;
        }
    }

    // 코드조작보다 애니메이션, 타임라인 등 활용하는게 좋을듯합니다
    IEnumerator GameStart()
    {
        AudioManager.Instance.PlaySFX("SFX_Button");
        yield return new WaitForSeconds(4.3f);
        AudioManager.Instance.StopBGM("BGM_Title");
        AudioManager.Instance.PlayBGM("BGM_Main");
        yield return new WaitForSeconds(3f);
        AudioManager.Instance.PlaySFX("SFX_Robot_On");
        yield return new WaitForSeconds(2f);
        uiManager.enabled = true;
        playerInput.enabled = true;
        StageManager.Instance.RespawnPlayer(player);
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlaySFX("SFX_Robot_On2");
        hpgauge.SetActive(true);
        yield return new WaitForSeconds(1f);
        player.SetActive(true);
        intro.SetActive(false);
    }

    IEnumerator LoadStart()
    {
        AudioManager.Instance.PlaySFX("SFX_Button");
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.StopBGM("BGM_Title");
        AudioManager.Instance.PlayBGM("BGM_Main");
        AudioManager.Instance.PlaySFX("SFX_Robot_On");
        uiManager.enabled = true;
        playerInput.enabled = true;
        yield return new WaitForSeconds(0.5f);
        player.SetActive(true);
        intro.SetActive(false);
        StageManager.Instance.RespawnPlayer(player);
        yield return new WaitForSeconds(2f);
        hpgauge.SetActive(true);
    }

    public void OnClickContinue()
    {
        //최근 세이브로 게임 시작
        // 키값 문자열 주의
        introAnimator.SetBool("LOAD_GAME", true);
        StartCoroutine(LoadStart());
    }

    public void OnClickSettings()
    {
        //세팅 패널
        AudioManager.Instance.PlaySFX("SFX_Button");
        introAnimator.SetBool("SETTINGS", true);
    }

    public void OnClickCancel()
    {
        //타이틀로 돌아가기
        AudioManager.Instance.PlaySFX("SFX_Button");
        introAnimator.SetBool("SETTINGS", false);
    }


    public void OnClickExit()
    {
        //게임 종료
        Application.Quit();
    }
}
