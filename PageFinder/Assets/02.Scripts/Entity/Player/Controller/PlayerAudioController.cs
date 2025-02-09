using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip[] audioClips;
    private bool canPlayAudio = true;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = DebugUtils.GetComponentWithErrorLogging<AudioSource>(this.gameObject, "AudioSource");
    }

    public void PlayAudio(string state)
    {
        if (!canPlayAudio) return;
        switch (state)
        {
            case "Attack1":
                audioSource.clip = audioClips[0];
                if(audioSource.time >= audioSource.time * 0.7f)
                    audioSource.Play();
                break;
            case "Attack2":
                audioSource.clip = audioClips[1];
                if (audioSource.time >= audioSource.time * 0.7f)
                    audioSource.Play();
                break;
        }
        canPlayAudio = false;
        StartCoroutine(ResetAudioCooldown(0.2f)); // 0.2�� �� ��� ����
    }

    private IEnumerator ResetAudioCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        canPlayAudio = true; // ��Ÿ�� ����
    }
}
