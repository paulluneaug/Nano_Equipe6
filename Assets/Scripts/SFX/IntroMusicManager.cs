using UnityEngine;
using UnityUtility.Singletons;

namespace SFX
{
    public class IntroMusicManager : MonoBehaviourSingleton<IntroMusicManager>
    {
        [SerializeField] AudioSource IntroMusic;

        public void StopIntroMusic()
        {
            IntroMusic.Stop();
        }
    }
}