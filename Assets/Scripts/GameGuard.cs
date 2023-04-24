using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGuard : GameBall
{
    public Transform triggerObject;
    public float minDistance = 1f;
    private void OnTriggerEnter(Collider other)
    {
        GameObject collidingPlayer = other.gameObject;
        if (collidingPlayer.CompareTag("PlayingObject") && collidingPlayer.TryGetComponent<LinkedGameBall>(out LinkedGameBall ball) )
        {
            if ( !ball.shooter)
            {
                //Debug.Log("Restarting Level - ? BM " + ball.shooter + " ? abled? ");// +ballManger == null?"": ballManger.enabled);
                RestartTheLevel();
            }
            else
            {
                //wait for it not to be a shooter and check if the distance is not close. 
                //Debug.Log("wait for it not to be a shooter and");
                
                StartCoroutine(WaitAndCheckForShooter(ball));
            }
        }
    }

    private void RestartTheLevel()
    {
        //Debug.Log("Restarting Level - ? BM " + manager.numberRows);
        manager.GameOverRestartLevel();
        //gameOverScreen.BringUpMenu(ScoreManager.instance.GetCurrentScore());
    }

    private IEnumerator WaitAndCheckForShooter(LinkedGameBall linkedBall)
    {
        if (linkedBall)
        {
            yield return new WaitUntil(() => linkedBall.shooter == false);
            if (linkedBall && DistanceTooClose(linkedBall))
            {
                RestartTheLevel();
            }
        }
    }

    private bool DistanceTooClose(LinkedGameBall nonshooterGameBall)
    {
        if(nonshooterGameBall)
        {
            //Debug.Log("Distance to curr" + Vector3.Distance(transform.position, nonshooterGameBall.gameObject.transform.position));
            if (Vector3.Distance(transform.position, nonshooterGameBall.gameObject.transform.position)< minDistance)
            {
                return true;
            }
        }
        return false;
    }
}
