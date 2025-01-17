using System;
using System.Collections;
using UnityEngine;

namespace Characters
{
    ///<summary>Controls the player behaviours.</summary>
    public class Player : MonoBehaviour
    {
        private static readonly Vector2 LEFT = Vector2.left;
        private static readonly Vector2 RIGHT = Vector2.right;
        private static readonly Vector2 UP = Vector2.up;
        private static readonly Vector2 DOWN = Vector2.down;

        private bool isStanding = false;
        private bool jumping = false;
        private bool attacking = false;
        private bool attackingJump = false;
        private bool isDead = false;
        private bool attackSuccess = false;
        private bool hitBox = false;
        private bool inFinishLine = false;
        private bool readyToResurrect = false;
        
        private GameObject objAttacked;

        private MoveController moveCtl;
        private AnimationController animationCtl;

        private Vector2 collisionSide;

        private string playerAction;

        private short playerDeads = 0;
        private short numCoins = 0;
        private short numKeys = 0;

        private Vector2 rayDirection;
        private RaycastHit2D rayHit;

        private AudioSource soundPlayer;
        [SerializeField] private AudioClip impactClip;
        [SerializeField] private AudioClip deadClip;
        [SerializeField] private AudioClip coinClip;
        [SerializeField] private AudioClip keyClip;


        void Start()
        {
            playerAction = Constants.ACTION_IDLE;

            moveCtl = new MoveController(gameObject, CharacterType.PLAYER);
            animationCtl = new AnimationController(gameObject);
            soundPlayer = GetComponent <AudioSource>();

            soundPlayer.volume = 0.5f;
        }

        void Update()
        {
            //RESET PLAYER DEADS - THIS IS FOR TEST ONLY 
            if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.DownArrow))
            {
                playerDeads = 0;
            }

            //To the jump
            if (Input.GetKeyDown(KeyCode.UpArrow) && moveCtl.CanJump())
            {
                jumping = true;
            }

            //To the attack
            if (Input.GetKeyDown(KeyCode.Space))
            {
                attacking = true;
            }

            //Player attack
            if (attacking && !isDead)
            {
                if (isStanding && !attackingJump)
                {
                    playerAction = Constants.ACTION_ATTACK;
                }
                else
                {
                    attackingJump = true;
                    playerAction = Constants.ACTION_JUMP_ATTACK;
                }
            }
            //Player move
            else if (Input.GetKey(KeyCode.RightArrow) && !attacking && isStanding && !isDead)
            {
                MoveToThe(RIGHT);
            }
            //Player move
            else if (Input.GetKey(KeyCode.LeftArrow) && !attacking && isStanding && !isDead)
            {
                MoveToThe(LEFT);
            }
            //Player stop
            else if (isStanding && !attacking && !isDead)
            {
                playerAction = Constants.ACTION_IDLE;
            }
            //Player dead
            else if (isDead)
            {               
                playerAction = Constants.ACTION_DEAD;
            }

            //Player jump
            if (jumping && !isDead)
            {
                playerAction = Constants.ACTION_JUMP;            
                moveCtl.Jump();
                jumping = false;
                isStanding = false;               
            }

            //Ray to verifies the collision with boxes and doors
            rayHit = Physics2D.Raycast(transform.position, rayDirection, 0.85f, LayerMask.GetMask("Boxes", "Doors"));

            //SHOW THE RAY - THIS IS FOR TEST ONLY 
            Debug.DrawRay(transform.position, rayDirection * 0.85f, Color.red);

            animationCtl.Animate(playerAction);
            //moveCtl.ImproveVerticalVelocity();
        }


        void FixedUpdate()
        {
            moveCtl.ImproveVerticalVelocity();
        }


        ///<summary>Moves the player.</summary>
        ///<param name="direction">The horizontal direction to move.</param>
        private void MoveToThe(Vector2 direction)
        {
            if (!isStanding)
            {
                return;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerAction = Constants.ACTION_RUN;
            }
            else
            {
                playerAction = Constants.ACTION_WALK;
            }
            transform.localScale = new Vector2(direction.x, 1);
            moveCtl.Move(direction, playerAction);
            rayDirection = direction;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            DetectCollisionSide(collision);

            //Detects if player fall in to a gap
            if (collision.collider.tag == "Gaps")
            {
                isDead = true;
            }

            //If player over something
            if (collisionSide == DOWN || collision.collider.tag == "Ground_Sides")
            {
                moveCtl.ResetJumps();
                isStanding = true;
            }
            
            //Collisions with enemies
            if (collision.collider.tag == "Pumpkin" && !attacking)
            {
                isDead = true;
                attackSuccess = false;
            }
            else if ((collision.collider.tag == "ZombieFemale" || collision.collider.tag == "ZombieMale") && !attackingJump)
            {
                isDead = true;
                attackSuccess = false;
            }
            else if (collision.collider.tag == "Pumpkin" && attacking)
            {
                attackSuccess = true;
                objAttacked = collision.gameObject;
                soundPlayer.PlayOneShot(impactClip);
            }
            else if ((collision.collider.tag == "ZombieFemale" || collision.collider.tag == "ZombieMale") && attackingJump)
            {
                attackSuccess = true;
                objAttacked = collision.gameObject;
                soundPlayer.PlayOneShot(impactClip);
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            GameObject gameObj = collider.gameObject;
            //To catch a coin
            if (gameObj.tag == "Coin")
            {
                numCoins++;
                soundPlayer.PlayOneShot(coinClip);
                Destroy(gameObj);
            }
            //To catch a key
            if (gameObj.tag == "Key")
            {
                numKeys++;
                soundPlayer.PlayOneShot(keyClip);
                Destroy(gameObj);
            }
            //When player reaches the goal
            if (gameObj.tag == "Flag")
            {
                inFinishLine = true;
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            //When player jump
            if (collision.collider.tag == "Ground" || collision.collider.tag == "Platforms")
            {
                isStanding = false;
            }
        }

        ///<summary>Determines the collision side.</summary>
        ///<param name="collision">The happened collision.</param>
        private void DetectCollisionSide(Collision2D collision)
        {
            foreach (ContactPoint2D cP in collision.contacts)
            {
                if (cP.normal == DOWN)
                {
                    collisionSide = UP;
                }
                if (cP.normal == UP)
                {
                    collisionSide = DOWN;
                }
                if (cP.normal == LEFT)
                {
                    collisionSide = RIGHT;
                }
                if (cP.normal == RIGHT)
                {
                    collisionSide = LEFT;
                }
            }
        }

        ///<summary>Reset variables when Attack animation ends.</summary>
        public void AttackAnimationEnd()
        {
            attacking = false;
            attackingJump = false;
            hitBox = false;
        }

        ///<summary>Stop the player and play sound when Dead animation starts.</summary>
        public void DeadAnimationStart()
        {
            soundPlayer.PlayOneShot(deadClip);
            moveCtl.Stop();
        }

        ///<summary>to come back to live when Dead animation starts.</summary>
        public void DeadAnimationEnd()
        {
            playerDeads++;
            if (playerDeads != Constants.Player.LIVES)
            {
                readyToResurrect = true;
                moveCtl.Alive();
            }
            
        }

        ///<summary>Determines if an attack was successful.</summary>
        ///<return>True if the player was attacking and collide with an enemy</return>
        public bool AttackSuccess()
        {
            return attackSuccess;
        }


        ///<summary>Return an object when attack was successful.</summary>
        ///<return>The obaject attacked</return>
        public GameObject GameObjectAttacked()
        {
            if (attackSuccess)
            {
                attackSuccess = false;
                return objAttacked;
            }
            return null;
        }

        ///<summary>Return the available lives of the player.</summary>
        ///<return>The number of lives available</return>
        public int GetPlayerLives()
        {
            return Constants.Player.LIVES - playerDeads;
        }

        ///<summary>Return the coins collected by the player.</summary>
        ///<return>The number of coins collected</return>
        public short GetNumberOfCoins()
        {
            return numCoins;
        }

        ///<summary>Return the keys collected by the player.</summary>
        ///<return>The number of keys collected</return>
        public short GetNumberOfKeys()
        {
            return numKeys;
        }

        ///<summary>Verifies if a box was hitting.</summary>
        ///<return>True if a box was hitting</return>
        public bool TryHitBox()
        {
            if (attacking && !hitBox)
            {
                hitBox = true;
                soundPlayer.PlayOneShot(impactClip);
                return true;
            }
            return false;
        }

        ///<summary>Subtracts one key when a door was opened</summary>
        public void OpenDoor()
        {
            numKeys--;
        }

        ///<summary>Verifies if the player arrives to goal.</summary>
        ///<return>True if the player arrives to goal</return>
        public bool InFinishLine()
        {
            return inFinishLine;
        }

        ///<summary>Verifies if dead animation ends</summary>
        ///<return>True if dead animation ends</return>
        public bool IsReadyToResurrect()
        {
            return readyToResurrect;
        }

        ///<summary>Comes to the live at the specified point.</summary>
        ///<param name="resurrPoint">The point where the player have to resurrect.</param>
        public void Resurrect(Vector3 resurrPoint)
        {
            transform.position = resurrPoint;
            isDead = false;
            readyToResurrect = false;
        }

        ///<summary>Verifies if player has available lives</summary>
        ///<return>True if player has available lives</return>
        public bool LiveAvailable()
        {
            if (playerDeads >= Constants.Player.LIVES)
            {
                return false;
            }
            return true;
        }

        ///<summary>Stops the player movement</summary>
        public void StopMovement()
        {
            playerAction = Constants.ACTION_IDLE;
            moveCtl.Stop();
        }

        ///<summary>Determines if the raycast detect an object</summary>
        ///<return>True the collider detected by the raycast</return>
        public Collider2D ObjectClose()
        {
            if (rayHit)
            {
                return rayHit.collider;
            }
            return null;
        }

    }

}