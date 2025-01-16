using System;
using UnityEngine;


namespace Characters
{
    ///<summary>Controls the character movement.</summary>
    public class MoveController
    {
        private readonly float DELTA_TIME_MULTIPLIER;
        private readonly float WALK_SPEED;
        private readonly float RUN_SPEED;
        private readonly float JUMP_FORCE;
        private readonly float OVER_JUMP_FORCE;
        private readonly short MAX_JUMPS;
        private readonly float ATTACK_DISTANCE;
        private readonly float FALL_MULTIPLIER;
        private readonly float LOW_JUMP_MULTIPLIER;
        private static readonly Vector2 LEFT = Vector2.left;
        private static readonly Vector2 RIGHT = Vector2.right;
        private static readonly Vector2 UP = Vector2.up;
        private static readonly Vector2 DOWN = Vector2.down;

        private Rigidbody2D charRigidB;
        private Collider2D charCollider;
        private Vector2 direction;
        private short availableJumps;
        private Vector2 rayDirection;
        private RaycastHit2D fallCtl;
        
        public MoveController(GameObject character, CharacterType charType)
        {
            charRigidB = character.GetComponent<Rigidbody2D>();
            charCollider = character.GetComponent<Collider2D>();
            availableJumps = MAX_JUMPS;

            if (charType == CharacterType.PLAYER)
            {
                DELTA_TIME_MULTIPLIER = Constants.Player.DELTA_TIME_MULTIPLIER;
                WALK_SPEED = Constants.Player.WALK_SPEED;
                RUN_SPEED = Constants.Player.RUN_SPEED;
                JUMP_FORCE = Constants.Player.JUMP_FORCE;
                OVER_JUMP_FORCE = Constants.Player.OVER_JUMP_FORCE;
                MAX_JUMPS = Constants.Player.MAX_JUMPS;
                FALL_MULTIPLIER = Constants.Player.FALL_MULTIPLIER;
                LOW_JUMP_MULTIPLIER = Constants.Player.LOW_JUMP_MULTIPLIER;
            }
            else
            {
                DELTA_TIME_MULTIPLIER = Constants.Enemy.DELTA_TIME_MULTIPLIER;
                WALK_SPEED = Constants.Enemy.WALK_SPEED;
                RUN_SPEED = Constants.Enemy.RUN_SPEED;
                JUMP_FORCE = Constants.Enemy.JUMP_FORCE;
                OVER_JUMP_FORCE = Constants.Enemy.OVER_JUMP_FORCE;
                MAX_JUMPS = Constants.Enemy.MAX_JUMPS;
                ATTACK_DISTANCE = Constants.Enemy.ATTACK_DISTANCE;
                FALL_MULTIPLIER = Constants.Enemy.FALL_MULTIPLIER;
                LOW_JUMP_MULTIPLIER = Constants.Enemy.LOW_JUMP_MULTIPLIER;
            }          
        }

        ///<summary>Move the character horizontally.</summary>
        ///<param name="orientation">The direction of the movement.</param>
        ///<param name="action">The action of character.</param>
        public void Move(Vector2 orientation, string action)
        {
            float speed;
            direction = orientation;
            if (action == Constants.ACTION_RUN || action == Constants.ACTION_ATTACK)
            {
                speed = RUN_SPEED;
            }
            else
            {
                speed = WALK_SPEED;
            }
            charRigidB.velocity = new Vector2(direction.x * speed, charRigidB.velocity.y);
        }

        ///<summary>Apply a vertical force to the character.</summary>
        public void Jump()
        {
            if (availableJumps == MAX_JUMPS)
            {
                charRigidB.velocity += UP * JUMP_FORCE;
            }
            else
            {
                charRigidB.velocity = new Vector2(charRigidB.velocity.x, OVER_JUMP_FORCE);
            }
            availableJumps--;
        }

        ///<summary>Reset the number of jumps in a row.</summary>
        public void ResetJumps()
        {
            availableJumps = 2;
        }

        ///<summary>Verifies if there is jumps available.</summary>
        ///<return>True if there is jumps available</return>
        public bool CanJump()
        {
            if (availableJumps > 0)
            {
                return true;
            }
            return false;
        }

        ///<summary>Do the jumps more realistics increasing the speed when falling</summary>
        public void ImproveVerticalVelocity()
        {
            if (charRigidB.velocity.y < 0)
            {
                charRigidB.velocity += UP * Physics2D.gravity * FALL_MULTIPLIER;
            }
            else if (charRigidB.velocity.y > 0)
            {
                charRigidB.velocity += UP * Physics2D.gravity * LOW_JUMP_MULTIPLIER;
            }
        }

        ///<summary>Return the velocity of the character.</summary>
        ///<return>The velocity</return>
        public Vector2 GetVelocity()
        {
            return charRigidB.velocity;
        }

        ///<summary>Set the velocity of the character.</summary>
        ///<param name="velocity">The velocity to set.</param>
        public void SetVelocity(Vector2 velocity)
        {
            charRigidB.velocity = velocity;
        }

        ///<summary>Increase the velocity of the character.</summary>
        ///<param name="velocity">The velocity to increse.</param>
        public void AddVelocity(Vector2 velocity)
        {
            charRigidB.velocity += velocity;
        }

        ///<summary>Verifies if the character is close to a gap.</summary>
        ///<return>True if the raycast don't detect the floor</return>
        public bool IsThereGap()
        {
            rayDirection = new Vector2(direction.x, -1);
            fallCtl = Physics2D.Raycast(charRigidB.transform.position, rayDirection, 1.5f, LayerMask.GetMask("Ground", "Platforms"));
            Debug.DrawRay(charRigidB.transform.position, rayDirection * 1.5f, Color.red);
            if (fallCtl)
            {
                return false;
            }
            return true;
        }

        ///<summary>Stop the character movements.</summary>
        /*public void Dead()
        {
            charRigidB.constraints = RigidbodyConstraints2D.FreezeAll;
            charCollider.enabled = false;
        }*/

        ///<summary>Restore the character movements.</summary>
        public void Alive()
        {
            charRigidB.constraints = RigidbodyConstraints2D.FreezeRotation;
            charCollider.enabled = true;
        }

        ///<summary>Stop the character movements.</summary>
        public void Stop()
        {
            charRigidB.constraints = RigidbodyConstraints2D.FreezeAll;
            charCollider.enabled = false;
        }
    }
}