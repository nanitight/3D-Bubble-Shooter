using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class GameManager : MonoBehaviour
{
    public GameObject spherePrefab;
    public GameObject aimPrefab;
    public Transform ballSpawnLocation; 
    public int numberRows = 3;
    public List<Material> ballMaterials;
    public List<Material> SelectableBallMaterials;
    public List<GameObject> allPlayerBalls;
    public GameOverScreen gameOverScreen, nextLevelScreen;
    public bool lostTheGame = false;
    private readonly float xMax = 9, zMax = 8;// xMin = 0 ;
    private readonly float yDefault = 0.5f;
    private Vector3 aimRotation = Vector3.forward;

    public bool another = false;
    void Start()
    {
        //Debug.Log("MGR START");
        Random.InitState(numberRows);
        AdjustMaterials();
        CreateInitialSetting();
        IdLikeToAddAnotherShooter(false);
        lostTheGame = false;
        gameOverScreen.gameObject.SetActive(false);
        nextLevelScreen.gameObject.SetActive(false);
        SetAimRotation(Vector3.forward);
    }
    private void FixedUpdate()
    {
        if (another)
        {
            IdLikeToAddAnotherShooter();
        }
        //else if (allPlayerBalls.Count <= 1)
        //{
        //    IdLikeToAddAnotherShooter();
        //}

    }

    private void AdjustMaterials()
    {
        int assignment;
        if (numberRows <= 4)
        {
            assignment = 2;
        }
        else if (numberRows <= 6)
        {
            assignment = 3;
        }
        else if (numberRows <= 8)
        {
            assignment = 4;
        }
        else if (numberRows <= 11)
        {
            assignment = 5;
        }
        else if (numberRows <= 13)
        {
            assignment = 6;
        }
        else if (numberRows <= 16)
        {
            assignment = 7;
        }
        else if (numberRows <= 18)
        {
            assignment= 8;
        }
        else
        {
            assignment = 9;
        }
        SelectableBallMaterials.Clear();
        for (int i = 0; i < assignment; i++)
        {
            SelectableBallMaterials.Add(ballMaterials[i]);
        }
    }

    private void CreateInitialSetting(bool resetEverythingRelatedToScore=true)
    {
        Random.InitState(numberRows);
        if (resetEverythingRelatedToScore)
        {
            ScoreManager.instance.ResetScoreToZero(); //reset level score to zero

        }
        else
        {
            ScoreManager.instance.ResetScoreToZeroExceptFinalScore(); 
        }
        float xPosMin;

        if (numberRows <= xMax) {
            xPosMin = ((xMax - numberRows) / 2 ) + 1 ;
            Debug.Log("First one at:" + xPosMin+ "max,rows"+xMax+" "+numberRows);
            TriangleSettingForLessThanNineRows(xPosMin);
        } 
        else if (numberRows <= 21) { 
            //xPosMin=xMin;
            // if the numRows is even,  we make a square of x= (numRows/2) , x by x . 
            //else it is odd,  we make rect of trunc(numRows/2) +- 1 ;
            int xRows , xCols ;
            if (numberRows % 2 == 0)
            {
                xRows = xCols = numberRows / 2;

                xPosMin = ((xMax - xCols) / 2) + 1;
                FourCornerSettingLessThan21Rows(xPosMin, xRows, xCols, false);
            }
            else
            {
                int res = Mathf.FloorToInt(numberRows/2);
                xRows = res+1;
                xCols = res;

                xPosMin = ((xMax - xCols) / 2) + 1;
                FourCornerSettingLessThan21Rows(xPosMin, xRows, xCols, Random.Range(0, 2) == 1); // simplified from Random.Range(0, 2) == 1 ? true : false
            }


            Debug.Log("Must be created");
        }
        else
        {
            numberRows = 21;
        }

        lostTheGame = false;
    }

    private void TriangleSettingForLessThanNineRows( float xPosMin)
    {
        float xMovement, zMovement = zMax, xOffset = 0.5f, accumulatedXOffset = 0;

        for (int x = numberRows; x > 0; x--)
        {
            xMovement = xPosMin;
            for (int i = x; i > 0; i--)
            {
                Vector3 pos = new(xMovement + accumulatedXOffset, yDefault, zMovement);
                GameObject obj = Instantiate(spherePrefab, pos, Quaternion.LookRotation(spherePrefab.transform.position));
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

                AssignRandomMaterial(obj, false);
                allPlayerBalls.Add(obj);
                //obj.AddComponent<LinkedGameBall>();
                //obj.AddComponent<ObserverConcrete>();
                //obj.AddComponent<SubjectConcrete>();
                obj.name = "Sphere" + i + " " + x;
                xMovement += 1;
            }
            accumulatedXOffset += xOffset;
            zMovement -= 0.88f; //instead of a whole 1 which leaves gaps,
        }
    }

    private void FourCornerSettingLessThan21Rows(float xPosMin, float xRows, float xCols, bool swap)
    {
        if (swap)
        {
            (xCols, xRows) = (xRows, xCols);
        } //swap values of rect

        float xMovement, zMovement = zMax;
        for (int x = 0; x < xRows; x++)
        {
            xMovement = xPosMin;
            for (int i = 0; i < xCols; i++)
            {
                Vector3 pos = new(xMovement, yDefault, zMovement);
                GameObject obj = Instantiate(spherePrefab, pos, Quaternion.LookRotation(spherePrefab.transform.position));
                obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;


                AssignRandomMaterial(obj, false);
                allPlayerBalls.Add(obj);
                obj.name = "Sphere" + i + " " + x;


                xMovement++;
            }
            zMovement--;
        }
    }
    
    private IEnumerator CreateShootingBall(bool negativeCount)
    {
        //Debug.Log("before creating Waiting shooter, alive" + ShooterStillAlive() + " enabled "+ShooterAlreadyExist());

        another = false;
        yield return new WaitUntil(() => ShooterStillAlive() == false) ;
        //int shooterCount = FindObjectsOfType<BallManagement>().Length; 
        //if (shooterCount == 0 )
        //{
        if (ShooterAlreadyExist() == null)
        {
            GameObject obj = Instantiate(spherePrefab, ballSpawnLocation.position, Quaternion.LookRotation(Vector3.forward));
            obj.AddComponent<BallManagement>();
            obj.GetComponent<LinkedGameBall>().shooter = true;
            AssignRandomMaterial(obj);
            allPlayerBalls.Add(obj);
            if (negativeCount)
            {
                ScoreManager.instance.AddShooter();
            }

        }


    }
    
    public void RemoveDestroyedGameBall(GameObject toRemove)
    {
        int i = allPlayerBalls.FindIndex(b => b.gameObject == toRemove);

        if (i >= 0)
        {
            allPlayerBalls.RemoveAt(i);
        }
        else
        {

        }

        //clean up, 
        for(int x = allPlayerBalls.Count -1 ; x >= 0; x--)
        {
            if (allPlayerBalls[x] == null )
            {
                allPlayerBalls.RemoveAt(x);
            }
        }

        //check if theres a ball with a controller script enabled if 
        if (allPlayerBalls.Count == 1)
        {
            IdLikeToAddAnotherShooter();
        }
        else if (allPlayerBalls.Count == 0)
        {
            // game over 
            //Debug.Log("GAME OVER!! RESTART");
            GameOverIncreaselevel();

        }


    }

    public void AdjustAimObject(Transform parent, bool hide = true)
    {
        if (aimPrefab != null)
        {
            aimPrefab.transform.SetParent(parent, false);
            if (hide)
            {
                aimPrefab.transform.position = new Vector3(1, -0.5f, -10f);
            }
            else
            {
                aimPrefab.transform.position = new Vector3(parent.position.x, parent.position.y + 0.01f, parent.position.z + 6);
            }
        }
    }

    public void SetAimRotation(Vector3 rotation)
    {
        //aimRotation = Vector3.zero ;
        aimRotation = rotation;
    }

    public Vector3 GetAimRotation()
    {
        return aimRotation;
    }

    void AssignRandomMaterial(GameObject obj, bool shooterBall = true)
    {
        if (obj == null) { return; }
        int i = Random.Range(0, SelectableBallMaterials.Count);

        if (shooterBall)
        {
            //check if the material still exists, if it does, its fine, use it, if not use one that exists
            if (!CheckIfMaterialExists(SelectableBallMaterials[i]))
            {
                //Debug.Log("Chosen material["+i+"] does not exist"+ SelectableBallMaterials[i].name);
                //material chosen is not existing in gameplay, choose one that exists
                i = FindExistingMaterial();
                if (i <= -1)
                {
                    //i = 0; //default, we couldn;t find a ball.
                    i = Random.Range(0,SelectableBallMaterials.Count);
                    Debug.Log("No mat found");
                }
            }
           
        }

        //assign the selected material
        List<Material> newMaterial = new (){ SelectableBallMaterials[i] }; 
        obj.GetComponent<MeshRenderer>().materials = newMaterial.ToArray();
    }

    bool CheckIfMaterialExists(Material mat)
    {
        for( int i = 0; i < allPlayerBalls.Count-1;i++)
        {
            if (allPlayerBalls[i] != null && allPlayerBalls[i].GetComponent<MeshRenderer>().materials[0].name.Contains(mat.name))
            {
                return true;
            }
        }
        //Debug.Log("Chosen" + mat.name+ " does not exist out of" + allPlayerBalls.Count);

        return false;
    }

    int FindExistingMaterial()
    {
        if (allPlayerBalls.Count> 0)
        {
            GameObject obj = allPlayerBalls[0]; 
            if (obj == null && allPlayerBalls.Count<2)
            {
                return allPlayerBalls.Count-1; 
            }
            if (obj == null ) { 
                obj = allPlayerBalls.Count > 2 ? allPlayerBalls[allPlayerBalls.Count - 1] : allPlayerBalls[0]; //if there's more than 2, choose last one, 
            }
            return obj != null ? SelectableBallMaterials.FindIndex(b=>obj.GetComponent<MeshRenderer>().materials[0].name.Contains(b.name)): -1;
        }

        return -1;
    }

    private void GameOverIncreaselevel()
    {
        if (ScoreManager.instance.GetCurrentScore() > 0)
        {
            // bring up restart/next menu
            //bring up the menu
            lostTheGame = true;
            nextLevelScreen.BringUpMenu(ScoreManager.instance.GetCurrentScore());
        }
        else
        {
            GameOverRestartLevel();
        }

    }

    public void ContinueToNextLevel()
    {
        StartCoroutine(NextLevelProcess());
    }

    private IEnumerator NextLevelProcess()
    {
        ++numberRows;
        AdjustMaterials();
        yield return new WaitForSeconds(.5f);
        CreateInitialSetting(false);
        yield return new WaitForSeconds(.5f);
        nextLevelScreen.gameObject.SetActive(false);
        lostTheGame = false;
        another = true;
    }

    public void GameOverRestartLevel()
    {
        gameOverScreen.BringUpMenu(ScoreManager.instance.GetCurrentScore());
        lostTheGame = true;
        

        //ContinueWithRestart();

    }

    public void ContinueWithRestart()
    {
        // remove all the active balls, 
        //recreate the level, this will happen with a different setting
        AdjustAimObject(null);
        foreach (GameObject obj in allPlayerBalls)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        //Random.InitState(numberRows);
        Start();
    }

    public void IdLikeToAddAnotherShooter(bool countedNegative = true)
    {
        if (!lostTheGame)
        {
            if (allPlayerBalls.Count >= 2)
            {
                StartCoroutine(CreateShootingBall(countedNegative));

                //another = true;
                //Debug.Log("ANOTHER grandted");
            }
            else
            {
                //check if shooterball exists, if it does detroy it 
                if (allPlayerBalls.Count <= 1)
                {
                    GameObject shooter = ShooterAlreadyExist();
                    //Debug.Log("Searched and found shooter-"+ shooter);
                    if (shooter != null)
                    {
                        AdjustAimObject(gameObject.transform);
                        //try with destroy
                        Destroy(shooter);
                        Debug.Log("WAIT HERE FOR ANOTHER");
                    }
                    else
                    {
                        StartCoroutine(CreateShootingBall(countedNegative));

                    }

                }

            }
        }

    }

    public GameObject ShooterAlreadyExist(bool mustbeEnabled = true)
    {
        //StackTrace stack = new StackTrace();
        //Debug.Log("Checking for existing shooter"+ stack.GetFrame(1).GetMethod().Name +" -> "+ stack.GetFrame(2).GetMethod().Name);
        if (mustbeEnabled)
        {
            return allPlayerBalls.Find(b => b.TryGetComponent<BallManagement>(out BallManagement comp) && comp.enabled == true );

        }
        else
        {
            return allPlayerBalls.Find(b => b.TryGetComponent<BallManagement>(out BallManagement comp) );
        }
    }
    
    public bool ShooterStillAlive()
    {
        return FindObjectsOfType<BallManagement>().Length > 0;
    }
}
