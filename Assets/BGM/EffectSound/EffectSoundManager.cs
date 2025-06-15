using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EffectSoundManager : MonoBehaviour
{
    public AudioClip jump;
    public AudioClip playerWalk;
    public AudioClip respawn;

    public AudioClip playerHit; // 적에게 맞을 때
    public AudioClip playerDie;
    public AudioClip playerEat; // 컵케익(체력포션 먹을 때)
    public AudioClip playerIcicleDm; // 고드름에 맞을 때
    public AudioClip playerAttack; // 때릴 때

    public AudioClip snowAndGift; // 적이 물체 날릴 때
    public AudioClip magic;
    public AudioClip enemyDie;

    public AudioClip nextScene;
    public AudioClip save;

    public AudioClip uiClick;

    private AudioSource audioSource;

    
    // Start is called before the first frame update
    void Start()
    {
       audioSource = GetComponent<AudioSource>(); 
       string currentSceneName = SceneManager.GetActiveScene().name;

       if(currentSceneName != "start"){
            audioSource.PlayOneShot(nextScene);
       }
    }

    public void UIClick(){
        audioSource.PlayOneShot(uiClick);
    }

    public void PlayRespawn(){
        audioSource.PlayOneShot(respawn);
    }

    public void PlayPlayerWalk(){
        audioSource.PlayOneShot(playerWalk);
    }

    public void PlayJump(){
        audioSource.PlayOneShot(jump);
    }

    public void PlayPlayerHit(){
        audioSource.PlayOneShot(playerHit);
    }
    public void PlayPlayerDie(){
        audioSource.PlayOneShot(playerDie);
    }
    public void PlayPlayerEat(){
        audioSource.PlayOneShot(playerEat);
    }
    public void PlayPlayerIcicleDm(){
        audioSource.PlayOneShot(playerIcicleDm);
    }
    public void PlayPlayerAttack(){
        audioSource.PlayOneShot(playerAttack);
    }

    public void PlaySnowAndGift(){
        audioSource.PlayOneShot(snowAndGift);
    }
    public void PlayMagic(){
        audioSource.PlayOneShot(magic);
    }
    public void PlayEnemyDie(){
        audioSource.PlayOneShot(enemyDie);
    }

    public void PlaySave(){
        audioSource.PlayOneShot(save);
    }
}
