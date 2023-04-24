using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{

    public Text pointsTextAtEnd;

    public void BringUpMenu(int score)
    {
        gameObject.SetActive(true);
        pointsTextAtEnd.text = score.ToString() + " POINTS";
    }
}
