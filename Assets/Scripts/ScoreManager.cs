using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : GameBall
{
    public static ScoreManager instance;

    public Text scoreText;
    public Text highScoreText;
    public Text negativeScoreText;
    public Text finalLevelScoreText;

    int score = 0;
    int highscore = 0;
    int negativescore = 0;
    int finalLevelScore = 0;
    // Start is called before the first frame update
    void Start()
    {
        finalLevelScore= negativescore= score= 0;
        highscore = PlayerPrefs.GetInt("highscore", 0);
        highScoreText.text = "HIGHSCORE: " + highscore.ToString();
        scoreText.text = score.ToString() + " POINTS";
        negativeScoreText.text = negativescore.ToString()+ " NEGATIVE POINTS";
        finalLevelScoreText.text = "Total Points: " + finalLevelScore.ToString();

        //Debug.Log("MAT?" + (GameObject.Find("Blockade BackWall Left").GetComponent<Renderer>().sharedMaterial == GameObject.Find("Pointer").GetComponent<Renderer>().sharedMaterial));
    }

    private void UpdateFinalScore()
    {
        if (negativeScoreText != null && scoreText!= null)
        {
            finalLevelScore = score - negativescore;
            if (finalLevelScore > 0)
            {
                  finalLevelScoreText.color = Color.green;
            }
            else if (finalLevelScore < 0)
            {
                finalLevelScoreText.color = Color.red;
            }
            else
            {
                finalLevelScoreText.color = Color.grey; // 0 is grey
            }
            finalLevelScoreText.text = "Total Points: "+finalLevelScore.ToString();
            scoreText.text = score.ToString() + " POINTS";

            //Debug.Log("Upodate score, final" + finalLevelScore);
        }
    }

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    public void AddPoint()
    {
        if (scoreText!= null )
        {
            score++;
            scoreText.text = score.ToString() + " POINTS";

            if (highscore < score)
            {
                PlayerPrefs.SetInt("highscore", score);

            }
            //Debug.Log("I added 1");
            UpdateFinalScore();
        }

    }

    public void AddShooter(bool increaseCount = true)
    {
        if (negativeScoreText != null && increaseCount)
        {
            negativescore++;
            negativeScoreText.text = negativescore.ToString() + " NEGATIVE POINTS";
            UpdateFinalScore();
        }
    }

    public void ResetScoreToZero()
    {
        Start();
    }

    public void ResetScoreToZeroExceptFinalScore()
    {
        int temp = finalLevelScore;
        Start(); 
        score =  temp; // because finalLevelScore is dependednt on the difference between score and negative points
        UpdateFinalScore();
        //Debug.Log("After reseting: " + finalLevelScore);
    }

    public int GetCurrentScore(){

        return finalLevelScore;
    }

   

}
