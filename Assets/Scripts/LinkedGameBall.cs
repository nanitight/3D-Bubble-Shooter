using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LinkedGameBall : GameBall
{
    public List< LinkedGameBall> neighbours = new();
    public bool visited = false;
    public bool shooter = false;
    private bool notCleanUp = false;
  
    public void AddNodeToNeighbour(LinkedGameBall nodeToAdd, bool checkForAlignment = true){
        int i = neighbours.FindIndex(x => x == nodeToAdd) ;

        if (i == -1)
        {
            if (nodeToAdd != null)
            {
                neighbours.Add(nodeToAdd);
            }

            if (checkForAlignment)
            {
                //Debug.Log("Checking for aligment");
                CheckForAlignemt();
            }

            if (nodeToAdd != null && nodeToAdd.gameObject.name.Contains("Clone"))
            {
                CheckForAlignemt();
            }
        }
    }

    public void RemoveNodeFromNeighbours(LinkedGameBall nodeToRemove)
    {
        int i = neighbours.FindIndex(x => x == nodeToRemove);
        if (i > -1) { 
            neighbours.RemoveAt(i);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        //    Debug.Log("Collision B,"+collision.gameObject.tag);

        if (collision.gameObject.CompareTag("PlayingObject"))
        {
            //Debug.Log("Collision B, trying to add" );

           
            if (gameObject.TryGetComponent(out BallManagement ballManager) ) //&& ballManager.enabled == true)
            {
                //the true version of add node called in the ball manager, since it is a precondition, and on the collision function of ball manager. 
                shooter = false;
                AddNodeToNeighbour(collision.gameObject.GetComponent<LinkedGameBall>());//check for alignment 
            }
            else
            {
                AddNodeToNeighbour(collision.gameObject.GetComponent<LinkedGameBall>(), false);
                // we're avoiding checking 
            }
        }

    }

    private void OnDestroy()
    {
        if (notCleanUp)
        {
            ScoreManager.instance.AddPoint();
        }
        if (manager != null)
        {
            manager.RemoveDestroyedGameBall(gameObject);
        }
        foreach(LinkedGameBall b in neighbours)
        {
            if (b != null)
            b.RemoveNodeFromNeighbours(this);
        }
    }

    public void DestroyMe()
    {
        notCleanUp = true;
        Destroy(gameObject);
    }

    private void CheckForAlignemt()
    {
        List<LinkedGameBall> sameAsMe = new()
        {  
            this
        };
        
        LinkedGameBall ptrNext;
        List<LinkedGameBall> queue = new()
        {
            this
        }; 
        List<LinkedGameBall> visitedNodes = new()
        {
            this
        };
        visited = true;
        while(queue.Count > 0)
        {
            ptrNext = queue[0];
            queue.RemoveAt(0);
            Debug.Log("---Traversal, Curr Node" + ptrNext.ToString());
            foreach (LinkedGameBall ball in ptrNext.neighbours)
            {
                //check if the color is equal
                if (ball != null && MaterialTheSame(ball.gameObject.GetComponent<Renderer>()) && ball.visited ==false)
                {
                    sameAsMe.Add(ball);
                    queue.Add(ball);
                    visitedNodes.Add(ball);
                    ball.visited = true;
                    //Debug.Log("---Traversal, neighbor Node same as me" + ball.ToString());

                }
            }

        }
        
        if (sameAsMe.Count > 2 )//&& MaterialTheSame(sameAsMe[1].gameObject.GetComponent<MeshRenderer>())  && MaterialTheSame(sameAsMe[2].gameObject.GetComponent<MeshRenderer>()))
        {
            foreach(LinkedGameBall ball in sameAsMe)
            {
                //Debug.Log("Same as me to destroy "+ ball.name + "tot: "+sameAsMe.Count);
                ball.DestroyMe();
            }
        }

        sameAsMe.Clear();

        foreach(LinkedGameBall ball in visitedNodes)
        {
            if (ball != null)
            {
                ball.visited = false;
            }
        }
        
    }

    private bool MaterialTheSame(Renderer toCheckAgainst)
    {
        //Debug.Log("Checking materials? "+ (GetComponent<MeshRenderer>().materials[0].name == toCheckAgainst.materials[0].name));
        //Debug.Log("Checking materials? "+toCheckAgainst.gameObject.name+"shared?"+ gameObject.name+"Res: " + (toCheckAgainst.sharedMaterial == GetComponent<Renderer>().sharedMaterial));

        return toCheckAgainst.materials[0].name.Contains(GetComponent<MeshRenderer>().materials[0].name);
        //return toCheckAgainst.sharedMaterial == GetComponent<Renderer>().sharedMaterial;

    }

    //public override string ToString()
    //{

    //    string words = "" + gameObject.name + "list->["; 
    //     foreach(LinkedGameBall ball in neighbours)
    //    {
    //        words+= (ball.ToString()+", ");
    //    } 
    //    words += "] ; " ;
    //    return words; 
    //}
}
