using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainButtonsAudio : MonoBehaviour, IPointerEnterHandler
{
    public static MainButtonsAudio Instance
    {
        get; private set;
    }
    private Button button;
    // Start is called before the first frame update
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        if(button != null)
        {
            button.onClick.AddListener(PlayClickSound);
        }
        button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        MainSceneSoundManager.Instance.PlaySound(MainSceneSoundManager.Instance.clickButton);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (MainSceneSoundManager.Instance == null)
        {
            //Debug.LogError("SoundManager instance is null!");
            return;
        }

        if (MainSceneSoundManager.Instance.hoverButton == null)
        {
            Debug.Log("hoverButton AudioSource is not assigned in SoundManager!");
            return;
        }

        MainSceneSoundManager.Instance.PlaySound(MainSceneSoundManager.Instance.hoverButton);
    }
}
