using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Characters;
using Objects;



namespace Game
{
    ///<summary>Class to control all aspects of the game play.</summary>
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private GameObject player;         
        [SerializeField] private GameObject pumpkinPrefab;
        [SerializeField] private GameObject zombieMalePrefab;
        [SerializeField] private GameObject zombieFemalePrefab;

        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private GameObject boxPrefab;
        [SerializeField] private GameObject doorPrefab;
        [SerializeField] private GameObject keyPrefab;

        [SerializeField] AudioClip gameMusic;
        [SerializeField] AudioClip gameOverMusic;

        [SerializeField] Text textLives;
        [SerializeField] Text textCoins;
        [SerializeField] Text textKeys;
        [SerializeField] Text info;
        [SerializeField] Text infoGamePLay;

        private AudioSource musicPlayer;
        private Player playerClass;
        private GameObject gameObjAttacked;

        private List<Vector3> pumpkinPositions = new List<Vector3>();
        private List<Vector3> zombieFemalePositions = new List<Vector3>();
        private List<Vector3> zombieMalePositions = new List<Vector3>();
        private List<Vector3> coinsPositions = new List<Vector3>();
        private List<Vector3> boxesPositions = new List<Vector3>();        
        private List<Vector3> doorsPositions = new List<Vector3>();
        private List<Vector3> keysPositions = new List<Vector3>();

        private Dictionary<GameObject, Enemy> enemies = new Dictionary<GameObject, Enemy>();
        private Dictionary<GameObject, Box> boxes = new Dictionary<GameObject, Box>();
        private Dictionary<GameObject, Door> doors = new Dictionary<GameObject, Door>();

        private Vector3[] resurrectionPoints =
        {
            new Vector3(-32f, -3.1f, 0f),
            new Vector3(11f, 3.9f, 0f),
            new Vector3(55f, 7.9f, 0f),
            new Vector3(161f, 3.9f, 0f),
            new Vector3(220f, -3.1f, 0f)
        };

        private short resurrPoint = 0;
        private short pumpkinIndex = 0;
        private short zombieFIndex = 0;
        private short zombieMIndex = 0;
        private short coinIndex = 0;
        private short boxIndex = 0;
        private short doorIndex = 0;
        private short keyIndex = 0;

        private bool gameOver = false;

        

        void Start()
        {
            playerClass = player.GetComponent<Player>();

            musicPlayer = GetComponent<AudioSource>();
            musicPlayer.volume = 0.3f;
            musicPlayer.Play();
            musicPlayer.loop = true;

            LoadGameObjectsPositions();

            textLives.text = Constants.Player.LIVES.ToString();
            textCoins.text = "0";
            textKeys.text = "0";
        }

        
        void Update()
        {
            textLives.text = playerClass.GetPlayerLives().ToString();
            textCoins.text = playerClass.GetNumberOfCoins().ToString();
            textKeys.text = playerClass.GetNumberOfKeys().ToString();
            info.text = " ";

            //Keep the mechanics info visible
            if (player.transform.position.x < -18f)
            {
                infoGamePLay.enabled = true;
            }
            else
            {
                infoGamePLay.enabled = false;
            }

            //COMO BACK TO LIFE - THIS IS FOR TEST ONLY 
            if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.DownArrow))
            {
                playerClass.Resurrect(resurrectionPoints[resurrPoint]);

            }

            //If Kill enemies
            if (playerClass.AttackSuccess())
            {
                gameObjAttacked = playerClass.GameObjectAttacked();
                if (isCharacter(gameObjAttacked))
                {
                    enemies[gameObjAttacked].Kill();
                    enemies[gameObjAttacked].Disappear();
                    enemies.Remove(gameObjAttacked);
                }                
            }

            //Verifies if a box or door near to player (uses a raycast)
            if (playerClass.ObjectClose() != null)
            {
                GameObject ObjClose = playerClass.ObjectClose().gameObject;
                if (playerClass.ObjectClose().tag == "Box")
                {
                    info.text = "Rompa la caja con la espada (Tecla espacio)";
                    if (playerClass.TryHitBox() && boxes[ObjClose].TryBreak())
                    {
                        Instantiate(coinPrefab, ObjClose.transform.position, Quaternion.identity);
                        boxes.Remove(ObjClose);
                    }
                }
                else if (playerClass.ObjectClose().tag == "Door")
                {
                    info.text = "Use una llave para abrir la puerta (Tecla O)";
                    if (Input.GetKeyDown(KeyCode.O) && playerClass.GetNumberOfKeys() > 0)
                    {                        
                        playerClass.OpenDoor();
                        doors[ObjClose].Open();
                    }
                }
            }

            //Change the come back to life point
            if (resurrPoint < 4 && player.transform.position.x > resurrectionPoints[resurrPoint + 1].x)
            {
                resurrPoint++;
            }

            //verifies if player died or arrive to goal
            if ((!playerClass.LiveAvailable() || playerClass.InFinishLine()) && !gameOver)
            {
                gameOver = true;
                playerClass.StopMovement();
                musicPlayer.Stop();
                musicPlayer.PlayOneShot(gameOverMusic);
                StartCoroutine(ChangeScene());
            }
            else if(playerClass.IsReadyToResurrect())
            {
                playerClass.Resurrect(resurrectionPoints[resurrPoint]);
            }
        }

        IEnumerator ChangeScene()
        {
            yield return new WaitForSecondsRealtime(6f);
            SceneManager.LoadScene("GameOver");
        }

        private void LateUpdate()
        {
            InstantiateObjects(player.transform.position.x + 25);
        }

        ///<summary>Instantiates the game objects if player reaches an x distance.</summary>
        ///<param name="xPlayer">The x position of the player.</param>
        private void InstantiateObjects(float xPlayer)
        {
            pumpkinIndex = InstantiateEnemies(xPlayer, pumpkinPositions, pumpkinIndex, pumpkinPrefab);
            zombieFIndex = InstantiateEnemies(xPlayer, zombieFemalePositions, zombieFIndex, zombieFemalePrefab);
            zombieMIndex = InstantiateEnemies(xPlayer, zombieMalePositions, zombieMIndex, zombieMalePrefab);

            coinIndex = InstantiateElements(xPlayer, coinsPositions, coinIndex, coinPrefab);
            doorIndex = InstantiateElements(xPlayer, doorsPositions, doorIndex, doorPrefab);
            boxIndex = InstantiateElements(xPlayer, boxesPositions, boxIndex, boxPrefab);
            keyIndex = InstantiateElements(xPlayer, keysPositions, keyIndex, keyPrefab);
        }

        ///<summary> Instantiates the enemies.</summary>
        ///<return>The last index used. To update</return>
        ///<param name="xPlayer">The x position of the player.</param>
        ///<param name="list">The position list of the enemies.</param>
        ///<param name="index">The last index used.</param>
        ///<param name="prefab">The prefab of the enemy.</param>
        private short InstantiateEnemies(float xPlayer, List<Vector3> list, short index, GameObject prefab)
        {
            GameObject enemyGo;
            while (index < list.Count && list[index].x <= xPlayer)
            {
                enemyGo = Instantiate(prefab, list[index], Quaternion.identity);
                enemies.Add(enemyGo, enemyGo.GetComponent<Enemy>());
                enemies[enemyGo].SetPlayer(player);
                index++;
            }
            return index;
        }

        ///<summary> Instantiates the objects (no enemies).</summary>
        ///<return>The last index used. To update</return>
        ///<param name="xPlayer">The x position of the player.</param>
        ///<param name="list">The position list of the objects.</param>
        ///<param name="index">The last index used.</param>
        ///<param name="prefab">The prefab of the object.</param>
        private short InstantiateElements(float xPlayer, List<Vector3> list, short index, GameObject prefab)
        {
            GameObject instance;
            while (index < list.Count && list[index].x <= xPlayer)
            {
                instance = Instantiate(prefab, list[index], Quaternion.identity);
                if (instance.tag == "Box")
                {
                    boxes.Add(instance, instance.GetComponent<Box>());
                }
                if (instance.tag == "Door")
                {
                    doors.Add(instance, instance.GetComponent<Door>());
                }

                index++;
            }
            return index;
        }

        ///<summary>Load the game objects positions from a text file.</summary>
        private void LoadGameObjectsPositions()
        {
            TextAsset positionsFile = Resources.Load("GameObjectsPositions") as TextAsset;
            string[] gameObjects = positionsFile.text.Split(';');

            for (short i=0; i<gameObjects.Length; i++)
            {
                if (gameObjects[i].Contains("Coin"))
                {
                    CreateVector(coinsPositions, gameObjects, i+1);
                }
                else if (gameObjects[i].Contains("Pumpkin"))
                {
                    CreateVector(pumpkinPositions, gameObjects, i + 1);
                }
                else if (gameObjects[i].Contains("ZombieFemale"))
                {
                    CreateVector(zombieFemalePositions, gameObjects, i + 1);
                }
                else if (gameObjects[i].Contains("ZombieMale"))
                {
                    CreateVector(zombieMalePositions, gameObjects, i + 1);
                }
                else if (gameObjects[i].Contains("Box"))
                {
                    CreateVector(boxesPositions, gameObjects, i + 1);
                }
                else if (gameObjects[i].Contains("Key"))
                {
                    CreateVector(keysPositions, gameObjects, i + 1);
                }
                else if (gameObjects[i].Contains("Door"))
                {
                    CreateVector(doorsPositions, gameObjects, i + 1);
                }
            }
        }

        ///<summary>
        ///Creates a vector3 with the position loaded from a text file 
        ///and inserts in a list
        ///</summary>
        ///<param name="list">The list where the vector3 will be inserted.</param>
        ///<param name="array">The array that contains the position.</param>
        ///<param name="i">The index of the position in the array.</param>
        private void CreateVector(List<Vector3> list, string[] array, int i)
        {
            float x = float.Parse(array[i]);
            float y = float.Parse(array[i+1]);
            float z = float.Parse(array[i+2]);
            list.Add(new Vector3(x, y, z));
        }

        ///<summary>Verifies if an object is a enemy.</summary>
        ///<return>true if the object is an enemy.</return>
        ///<param name="gObj">The object to verify.</param>      
        private bool isCharacter(GameObject gObj)
        {
            if(gObj.tag == "Pumpkin" || gObj.tag == "ZombieFemale" || gObj.tag == "ZombieMale" || gObj.tag == "Player")
            {
                return true;
            }
            return false;
        }


    }
}