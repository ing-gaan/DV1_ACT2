using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Game
{
    ///<summary>Button actions class.</summary>
    public class ButtonsActions : MonoBehaviour
    {
        private AudioSource sfx;

        public void Start()
        {
            sfx = GetComponent<AudioSource>();
            sfx.loop = false;
            sfx.playOnAwake = false;
        }

        ///<summary>Action Menu button (Menu).</summary>
        public void GameMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        ///<summary>Action Play button (Jugar).</summary>
        public void playingGame()
        {
            sfx.Play();
            StartCoroutine(waitSfx());
        }

        //
        ///Action Exit button (Salir).</summary>
        public void ExitGame()
        {
            Application.Quit();
        }

        ///<summary>Until finish sound.</summary>
        IEnumerator waitSfx()
        {
            while (sfx.isPlaying)
            {
                yield return new WaitForSecondsRealtime(0.5f);
            }
            SceneManager.LoadScene("Level_1");
        }

    }
}