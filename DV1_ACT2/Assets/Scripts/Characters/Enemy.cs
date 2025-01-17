using System.Collections;
using UnityEngine;

namespace Characters
{
    ///<summary>Class to control the enemies.</summary>
    public class Enemy : MonoBehaviour
    {
        private static readonly Vector2 LEFT = Vector2.left;
        private static readonly Vector2 RIGHT = Vector2.right;
        private static readonly Vector2 UP = Vector2.up;
        private static readonly Vector2 DOWN = Vector2.down;

        private Transform player;

        private bool goingToTheLeft;

        private MoveController moveCtl;
        private AnimationController animationCtl;
        private string enemyAction;

        private float xInitPosition;
        private float leftBoundary;
        private float rightBoundary;
        private bool isFixedLeftBound;
        private bool isFixedRightBound;
        private bool isDead;
        private CharacterType charType;

        private AudioSource soundPlayer;
        [SerializeField] AudioClip deadClip;

        void Start()
        {
            //Set the enemy type
            if (tag == "Pumpkin")
            {
                charType = CharacterType.ENEMY_PUMPKIN;
            }
            else if (tag == "ZombieFemale")
            {
                charType = CharacterType.ENEMY_ZOMBIE_FEMALE;
            }
            else if (tag == "ZombieMale")
            {
                charType = CharacterType.ENEMY_ZOMBIE_MALE;
            }

            moveCtl = new MoveController(gameObject, charType);
            animationCtl = new AnimationController(gameObject);
            enemyAction = Constants.ACTION_IDLE;
            goingToTheLeft = true;
            xInitPosition = transform.position.x;
            leftBoundary = xInitPosition - Constants.Enemy.MAX_TRAVEL_DISTANCE / 2;
            rightBoundary = xInitPosition + Constants.Enemy.MAX_TRAVEL_DISTANCE / 2;

            isFixedLeftBound = false;
            isFixedRightBound = false;
            isDead = false;
            enabled = false;

            soundPlayer = GetComponent<AudioSource>();
        }


        private void Update()
        {
            //Attack when player is close 
            if (Vector2.Distance(transform.position, player.position) < Constants.Enemy.ATTACK_DISTANCE)
            {
                AttackPlayer();
            }
            else
            {
                Walk();
            }

            //Enemy dead
            if (isDead)
            {
                animationCtl.Animate(Constants.ACTION_DEAD);
                moveCtl.Stop();
                enabled = false;
                soundPlayer.PlayOneShot(deadClip);
                soundPlayer.volume = 0.3f;
            }
            //Move the enemy
            else if (goingToTheLeft)
            {
                MoveToThe(LEFT);
            }
            //Move the enemy
            else
            {
                MoveToThe(RIGHT);
            }

            //Set the movement limit of the enemy
            if (moveCtl.IsThereGap())
            {
                if (goingToTheLeft && !isFixedLeftBound)
                {                    
                    FixLeftBound();
                }
                else if (!goingToTheLeft && !isFixedRightBound)
                {
                    FixRightBound();
                }
            }

            //Change the horizontal direction
            if (transform.position.x < leftBoundary)
            {
                goingToTheLeft = false;
            }
            if (transform.position.x > rightBoundary)
            {
                goingToTheLeft = true;
            }
        }


        ///<summary>Moves the enemy.</summary>
        ///<param name="direction">The horizontal direction to move.</param>
        private void MoveToThe(Vector2 direction)
        {
            transform.localScale = new Vector2(direction.x, 1);
            moveCtl.Move(direction, enemyAction);
        }


        ///<summary>Attack the player.</summary>
        private void AttackPlayer()
        {
            if ((goingToTheLeft && (transform.position.x > player.position.x)) ||
                    (!goingToTheLeft && (transform.position.x < player.position.x)))
            {
                Attack();
            }
        }

        ///<summary>Set the walk action an animate.</summary>
        private void Walk()
        {
            enemyAction = Constants.ACTION_WALK;
            animationCtl.Animate(enemyAction);
        }

        ///<summary>Set the attack action an animate.</summary>
        private void Attack()
        {
            if (charType == CharacterType.ENEMY_PUMPKIN)
            {
                enemyAction = Constants.ACTION_RUN;
            }
            else
            {
                enemyAction = Constants.ACTION_ATTACK;
            }  
            animationCtl.Animate(enemyAction);
        }


        void OnCollisionEnter2D(Collision2D collision)
        {
            //Activates the enemy when touch the foor after the instantiate
            if (collision.collider.tag == "Ground" || collision.collider.tag == "Platforms")
            {
                enabled = true;
            }
            //Set limits of the enemy
            if (isObject(collision.collider.gameObject) || collision.collider.tag == "Ground_Sides")
            {
                if (goingToTheLeft)
                {
                    FixLeftBound();
                }
                else
                {
                    FixRightBound();
                }
            }

            //Change the horizontal direction
            if (isCharacter(collision.collider.gameObject))
            {
                if (goingToTheLeft)
                {
                    goingToTheLeft = false;
                }
                else
                {
                    goingToTheLeft = true;
                }
            }

        }

        ///<summary>Set the left limit of the movement.</summary>
        private void FixLeftBound()
        {
            if (!isFixedLeftBound)
            {
                leftBoundary = transform.position.x + Constants.POSITION_ADJUSTMENT;
                isFixedLeftBound = true;
            }
            if (!isFixedRightBound)
            {
                rightBoundary = leftBoundary + Constants.Enemy.MAX_TRAVEL_DISTANCE;
            }
        }

        ///<summary>Set the right limit of the movement.</summary>
        private void FixRightBound()
        {
            if (!isFixedRightBound)
            {
                rightBoundary = transform.position.x - Constants.POSITION_ADJUSTMENT;
                isFixedRightBound = true;
            }            
            if (!isFixedLeftBound)
            {
                leftBoundary = rightBoundary - Constants.Enemy.MAX_TRAVEL_DISTANCE;
            }
        }

        ///<summary>Set the player game object.</summary>
        public void SetPlayer(GameObject gO)
        {
            player = gO.GetComponent<Transform>();
        }

        ///<summary>Kill the enemy.</summary>
        public void Kill()
        {
            isDead = true;
        }

        ///<summary>Wait to detroy the game object.</summary>
        public void Disappear()
        {
            StartCoroutine(WaitToDisappear());
        }

        ///<summary>Wait to detroy the game object.</summary>
        IEnumerator WaitToDisappear()
        {
            yield return new WaitForSecondsRealtime(5f);
            Destroy(gameObject);
        }

        ///<summary>Verifies if an game object is an character.</summary>
        ///<param name="gObj">The game object to verify.</param>
        private bool isCharacter(GameObject gObj)
        {
            if (gObj.tag == "Pumpkin" || gObj.tag == "ZombieFemale" || gObj.tag == "ZombieMale" || gObj.tag == "Player")
            {
                return true;
            }
            return false;
        }

        ///<summary>Verifies if an game object is an object.</summary>
        ///<param name="gObj">The game object to verify.</param>
        private bool isObject(GameObject gObj)
        {
            if (gObj.tag == "Door" || gObj.tag == "Box" )
            {
                return true;
            }
            return false;
        }

    }
}