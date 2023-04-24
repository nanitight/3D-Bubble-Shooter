using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManagement : GameBall
{
    private Rigidbody rb;
    public GameManager gameManager;
    public float force = 10000;
    public GameObject aim;
    private float maxRightRotation = 40f;
    private float maxleftRotation =360f - 40f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        gameManager = FindAnyObjectByType<GameManager>();
        manager.AdjustAimObject(this.transform,false);
        transform.rotation = Quaternion.Euler(manager.GetAimRotation());

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (manager.allPlayerBalls.Count > 1 )
            {
                manager.SetAimRotation(transform.rotation.eulerAngles);
                manager.AdjustAimObject(manager.transform);
                rb.AddForce(transform.forward *force*Time.deltaTime, ForceMode.VelocityChange);
                GetComponent<BallManagement>().enabled = false;
            }

        }

        bool leftKeyPressed = Input.GetKey(KeyCode.LeftArrow);
        bool rightKeyPressed = Input.GetKey(KeyCode.RightArrow);
        if (leftKeyPressed || rightKeyPressed)
        {
            Vector3 currRotation = transform.rotation.eulerAngles;
            float increaseFactor = 10 * Time.deltaTime;
            if (leftKeyPressed)
            {
                if (currRotation.y > maxleftRotation || currRotation.y <maxRightRotation)
                {
                    currRotation.y -= increaseFactor;
                }
                else
                {
                    currRotation.y = maxleftRotation+0.5f;
                }
            }
            else
            {
                if (currRotation.y > maxleftRotation || currRotation.y < maxRightRotation)
                {
                    currRotation.y += increaseFactor;
                }
                else
                {
                    currRotation.y = maxRightRotation-0.5f;
                }
            }
            transform.eulerAngles = currRotation;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayingObject") || collision.gameObject.CompareTag("BlockBall"))
        {
            rb.constraints = RigidbodyConstraints.FreezePosition;
            //rb.constraints = RigidbodyConstraints.FreezeRotation;
            LinkedGameBall otherObj = collision.gameObject.GetComponent<LinkedGameBall>();
            if(TryGetComponent<LinkedGameBall>(out LinkedGameBall linked ))
            {
                linked.AddNodeToNeighbour(otherObj);

            }//the else version is handled back in the linkedgameball script
            else
            {
                //otherObj.AddNodeToNeighbour(linked);

            }
            Destroy(this.gameObject.GetComponent<BallManagement>()) ;
        }
        else
        {
            //Debug.Log("Collision But not object");
        }
    }

    //IEnumerator AddAnother()
    //{
    //    Debug.Log("Waiting shooter" + gameManager.ShooterStillAlive() + " enabled " + gameManager.ShooterAlreadyExist());
    //    //yield return new WaitUntil(() => gameManager.ShooterStillAlive() == false );
    //    yield return null;
    //    gameManager.IdLikeToAddAnotherShooter();

    //}
    void AddAnother()
    {
        //Debug.Log("Waiting shooter" + gameManager.ShooterStillAlive() + " enabled ");
        //yield return new WaitUntil(() => gameManager.ShooterStillAlive() == false );
       
        manager.IdLikeToAddAnotherShooter();

    }

    private void OnDestroy()
    {
        if (manager != null && aim != null)
        {
            if (aim.transform.parent == gameObject.transform)
            {
                manager.AdjustAimObject(null);
            }
            else
            {
                manager.AdjustAimObject(manager.transform);
            }
        }
        if (gameObject.TryGetComponent<BallManagement>(out BallManagement man) && man.enabled == false)
        {
            AddAnother();
        }
        else
        {
            ScoreManager.instance.AddShooter(false);
        }
    }
}
