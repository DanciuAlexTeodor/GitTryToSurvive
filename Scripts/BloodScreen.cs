using UnityEngine;
using UnityEngine.UI;

public class BloodScreen : MonoBehaviour
{
    public Image MediumBlood;
    public Image FullBlood;
    public float currentHealth;

    private void Start()
    {
        
        MediumBlood = transform.Find("MediumBlood").GetComponent<Image>();
        FullBlood = transform.Find("FullBlood").GetComponent<Image>();
    }

    private void Update()
    {
        currentHealth = PlayerState.Instance.currentHealth;

        if (currentHealth <= 50 && currentHealth > 25)
        {
            FullBlood.enabled = false;
            MediumBlood.enabled = true;
            SoundManager.Instance.StopSound(SoundManager.Instance.HeartBeatAndDie);
            SoundManager.Instance.PlaySound(SoundManager.Instance.Heartbeat);
        }
        else if (currentHealth <= 25)
        {
            SoundManager.Instance.StopSound(SoundManager.Instance.Heartbeat);
            SoundManager.Instance.PlaySound(SoundManager.Instance.HeartBeatAndDie);
            MediumBlood.enabled = false;
            FullBlood.enabled = true;

        }
        else if (currentHealth > 50)
        {
           
            SoundManager.Instance.StopSound(SoundManager.Instance.Heartbeat);
            
           
            MediumBlood.enabled = false;
            FullBlood.enabled = false;

        }
    }
}
