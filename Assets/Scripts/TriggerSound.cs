using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSound : MonoBehaviour
{

    [SerializeField] private AudioSource source = null;

    public float soundRange = 25f;
    [HideInInspector]public float soundRangeValue;

    [SerializeField] private Sound.SoundType soundType = Sound.SoundType.Danger;
   
    private void Start() {
        soundRangeValue = soundRange;
    }
  


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(transform.position, soundRange);
        Gizmos.DrawWireSphere(transform.position, soundRange);
    }

    public void PlaySound()
    {

        if (source.isPlaying) //If already playing a sound, don't allow overlapping sounds 
            return;

        source.Play();

        var sound = new Sound(transform.position, soundRange, soundType);

        Sounds.MakeSound(sound);
    }


    
}
