using UnityEngine;

namespace Objects
{

    ///<summary>Class to cotrol the Doors behaviours.</summary>
    public class Door : MonoBehaviour
    {

        private Collider2D doorCollider;
        private Animator doorAnimator;

        [SerializeField] AudioClip openDoor;
        private AudioSource soundPlayer;

        void Start()
        {
            doorCollider = GetComponent<Collider2D>();
            doorAnimator = GetComponent<Animator>();
            soundPlayer = GetComponent<AudioSource>();
        }

        void Update()
        {

        }

        public void Open()
        {
            doorCollider.enabled = false;
            doorAnimator.Play("Door open");
            soundPlayer.PlayOneShot(openDoor);
        }


    }
}