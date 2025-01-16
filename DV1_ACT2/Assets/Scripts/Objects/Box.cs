using System.Collections;
using UnityEngine;


namespace Objects
{
    ///<summary>Class to cotrol the Box behaviours.</summary>
    public class Box : MonoBehaviour
    {
        private Animator animator;
        private Collider2D boxCollider;
        private Rigidbody2D boxRigBOdy;

        private short numAttacks = 0;
        private string[] hitsAnim = { "Box_Hit1", "Box_Hit2", "Box_Hit3" };

        [SerializeField] AudioClip boxExplosion;
        private AudioSource soundPlayer;


        void Start()
        {
            animator = GetComponent<Animator>();
            boxCollider = GetComponent<Collider2D>();
            boxRigBOdy = GetComponent<Rigidbody2D>();
            soundPlayer = GetComponent<AudioSource>();
        }

        ///<summary>Verifies if the attack broke the box.</summary>
        ///<return>Return true if the box was broken.</return>
        public bool TryBreak()
        {
            numAttacks++;
            if (numAttacks < 4)
            {
                StartCoroutine(showHit());
                return false;
            }
            else
            {
                animator.Play("Box_Destruction");
                boxRigBOdy.constraints = RigidbodyConstraints2D.FreezeAll;
                boxCollider.enabled = false;
                soundPlayer.PlayOneShot(boxExplosion);
                return true;
            }
        }

        ///<summary>Choose the animation regarding the attacks.</summary>
        IEnumerator showHit()
        {
            yield return new WaitForSecondsRealtime(0.3f);
            animator.Play(hitsAnim[numAttacks - 1]);
        }


        ///<summary>Destroy the gameobject when box was broken.</summary>
        public void DestructionAnimationEnd()
        {
            Destroy(gameObject);
        }

    }
}