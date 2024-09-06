using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bansha : HighEnemy
{
    [Header("SoundWave")]
    [SerializeField]
    private GameObject SoundWaveObject_Prefab;
    [SerializeField]
    private Transform soundWavePos;

    GameObject[] SoundWaves = new GameObject[3];

    [SerializeField]
    private int maxSoundWaveFireCnt;

    public override void Start()
    {
        // base.Start���� �ش� �ڷ�ƾ���� �̸� ������ �ʵ��� ����.
        isUpdaterCoroutineWorking = true;
        isAnimationCoroutineWorking = true;

        base.Start();

        // ����ü ����
        for (int i = 0; i < SoundWaves.Length; i++)
        {
            SoundWaves[i] = Instantiate(SoundWaveObject_Prefab, new Vector3(enemyTr.position.x, -10, enemyTr.position.z), Quaternion.identity, GameObject.Find("Projectiles").transform); //GameObject.Find("Bullet").transform
            SoundWaves[i].GetComponent<Projectile>().Init(gameObject.name, gameObject.name + " - SoundWaves" + i, 10, soundWavePos, playerObj);
        }

        // ��� �����̱� ������ ��ų ������ true�� �����Ͽ� ��Ÿ�ӿ��� �����ϰ� �Ѵ�.
        skillCondition[0] = true;

        StartCoroutine(Updater());
        StartCoroutine(Animation());
    }

    private void Skill0()
    {
        StartCoroutine(SoundWave());
    }

    private IEnumerator SoundWave()
    {
        int soundWavesIndex;

        for (int i = 0; i < maxSoundWaveFireCnt; i++)
        {
            soundWavesIndex = FindSoundWaveThatCanBeUsed();

            if (soundWavesIndex == -1) // ����� �� �ִ� ����ü�� ���� ��� 
            {
                Debug.LogWarning("����� �� �ִ� SoundWave ������");
                continue;
            }

            SoundWaves[soundWavesIndex].SetActive(true);
            SoundWaves[soundWavesIndex].GetComponent<Projectile>().SetDirToMove();

            yield return new WaitForSeconds(0.5f);
        }
    }

    int FindSoundWaveThatCanBeUsed()
    {
        for (int i = 0; i < SoundWaves.Length; i++)
        {
            if (SoundWaves[i].activeSelf) // ������� �Ѿ� 
                continue;
            return i;
        }
        return -1;
    }
}
