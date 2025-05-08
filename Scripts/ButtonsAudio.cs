using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonsAudio : MonoBehaviour, IPointerEnterHandler
{
    public static ButtonsAudio Instance
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
        button.onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.clickButton);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SoundManager.Instance == null)
        {
            //Debug.LogError("SoundManager instance is null!");
            return;
        }

        if (SoundManager.Instance.hoverButton == null)
        {
            //Debug.LogError("hoverButton AudioSource is not assigned in SoundManager!");
            return;
        }

        SoundManager.Instance.PlaySound(SoundManager.Instance.hoverButton);
    }
}
