using UnityEngine;


namespace Game
{
    ///<summary>Class to create a parallax effect with the background.</summary>
    public class Parallax : MonoBehaviour
    {
        private float spriteLength;
        private float startPosition;

        [SerializeField] private GameObject mainCamera;
        [SerializeField] private float offset;

        private void Start()
        {
            startPosition = transform.position.x;
            spriteLength = GetComponent<SpriteRenderer>().bounds.size.x;
        }

        private void FixedUpdate()
        {
            float temp = mainCamera.transform.position.x * (1 - offset);
            float distance = mainCamera.transform.position.x * offset;

            //Moves the background regarding the camera
            transform.position = new Vector2(startPosition + distance, transform.position.y);

            if (temp > startPosition + spriteLength)
            {
                startPosition += spriteLength;
            }
            else if (temp < startPosition - spriteLength)
            {
                startPosition -= spriteLength;
            }
        }
    }
}