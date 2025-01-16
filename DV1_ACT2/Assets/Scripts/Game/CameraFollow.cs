using UnityEngine;
using Characters;

namespace Game
{
    ///<summary>Class to move the camara to follow the player.</summary>
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform player;
        private Rigidbody2D playerRb;

        private static readonly float PLAYER_WALK_SPEED = Constants.Player.WALK_SPEED;
        private static readonly float PLAYER_RUN_SPEED = Constants.Player.RUN_SPEED;
        private static readonly float PLAYER_DELTA_TIME_MULTIPLIER = Constants.Player.DELTA_TIME_MULTIPLIER;

        private static readonly float AHEAD_OFFSET = 2f;
        private static readonly float SMOOTH_MOVE = 0.05f;

        private static readonly float ZOOM_OUT = -20f;
        private static readonly float ZOOM_MIDDLE = -12f;
        private static readonly float ZOOM_IN = -10f;

        private static readonly float MIN_LEFT_BOUND = -34f;

        private static readonly float MIN_UP_BOUND = 11.5f;     //TO ZOOM OUT
        private static readonly float MAX_UP_BOUND = 17.5f;     //TO ZOOM IN     
        private static readonly float MIN_BOTTOM_BOUND = -0.5f; //TO ZOOM IN
        private static readonly float MAX_BOTTOM_BOUND = 5.5f;  //TO ZOOM OUT

        private float newCameraZ;
        private float upBound;
        private float bottomBound;



        private void Start()
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            changeBounds(ZOOM_MIDDLE);
        }

        private void FixedUpdate()
        {
            float newCameraX = player.position.x + AHEAD_OFFSET;
            
            //Sets the limits of the camera movement
            if (newCameraX < MIN_LEFT_BOUND)
            {
                newCameraX = MIN_LEFT_BOUND;
            }

            //Sets the camera zoom regarding the player speed
            if (Mathf.Abs(playerRb.velocity.x) < PLAYER_RUN_SPEED * PLAYER_DELTA_TIME_MULTIPLIER * Time.deltaTime)
            {
                changeBounds(ZOOM_IN);
            }
            else
            {
                changeBounds(ZOOM_MIDDLE);
            }

            if (Input.GetKey(KeyCode.Z))
            {
                changeBounds(ZOOM_OUT);
            }

            //Calculate an smooth camera movement
            float newCameraY = Mathf.Clamp(player.position.y, bottomBound, upBound);
            Vector3 cameraPosition = new Vector3(newCameraX, newCameraY, newCameraZ);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, cameraPosition, SMOOTH_MOVE);
            transform.position = smoothedPosition;

        }

        ///<summary>Changes the limits of the camera movement regarding the zoom.</summary>
        ///<param name="zoom">The camera zoom.</param>
        private void changeBounds(float zoom)
        {
            newCameraZ = zoom;
            if (zoom == ZOOM_IN)
            {
                upBound = MAX_UP_BOUND;
                bottomBound = MIN_BOTTOM_BOUND;
                return;
            }
            if (zoom == ZOOM_OUT)
            {
                upBound = MIN_UP_BOUND;
                bottomBound = MAX_BOTTOM_BOUND;
                return;
            }
            if (zoom == ZOOM_MIDDLE)
            {
                upBound = MAX_UP_BOUND;
                bottomBound = MIN_BOTTOM_BOUND;
                return;
            }
        }

    }
}