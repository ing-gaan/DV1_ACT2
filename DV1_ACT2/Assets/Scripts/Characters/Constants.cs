using UnityEngine;

namespace Characters
{
    //Class that store the constants of the game
    public static class Constants
    {
        //General constatnts
        public static readonly string ACTION_ATTACK = "Attack";
        public static readonly string ACTION_DEAD = "Dead";
        public static readonly string ACTION_IDLE = "Idle";
        public static readonly string ACTION_JUMP = "Jump";
        public static readonly string ACTION_JUMP_ATTACK = "Jump_Attack";
        public static readonly string ACTION_RUN = "Run";
        public static readonly string ACTION_WALK = "Walk";
        public static readonly float POSITION_ADJUSTMENT = 0.2f;

        //The player constants
        public static class Player
        {
            public static readonly short LIVES = 5;
            public static readonly float DELTA_TIME_MULTIPLIER = 25f;
            public static readonly float WALK_SPEED = 4f;
            public static readonly float RUN_SPEED = 6f;
            

            public static readonly float JUMP_FORCE = 12f;
            public static readonly float OVER_JUMP_FORCE = JUMP_FORCE / 1.4f;
            public static readonly short MAX_JUMPS = 2;

            public static readonly float FALL_MULTIPLIER = 2.5f/50;
            public static readonly float LOW_JUMP_MULTIPLIER = 2f/50;
        }

        //The enemies constants
        public static class Enemy
        {
            public static readonly float DELTA_TIME_MULTIPLIER = 25f;
            public static readonly float WALK_SPEED = 3f;
            public static readonly float RUN_SPEED = 6f;

            public static readonly float ATTACK_DISTANCE = 8f;

            public static readonly float JUMP_FORCE = 10f;
            public static readonly float OVER_JUMP_FORCE = JUMP_FORCE / 1.3f;
            public static readonly short MAX_JUMPS = 2;

            public static readonly float FALL_MULTIPLIER = 2.5f;
            public static readonly float LOW_JUMP_MULTIPLIER = 2f;

            public static readonly float MAX_TRAVEL_DISTANCE = 12f;            
        }
    }
}