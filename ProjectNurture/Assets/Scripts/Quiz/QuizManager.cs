using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    /* Questions:
    Q1. What is the official name of this plant hovering in front of you?
    Q2. How much water does this species of tomato prefer?
    Q3. In which soil does this tomato grow best in?
    Q4. How much sun does this tomato prefer?
    Q5. What propagation methods does this tomato use?
    Q6. What is the preferred climate of this tomato?
    */

    public List<QuestionAndAnswers> QnA;
    public GameObject[] options;
    public GameObject[] optionButtons;
    public int currentQuestion;

    //public GameObject Quizpanel;
    //public GameObject GoPanel;

    public Text QuestionTxt;
    public Text TotalScore;

    private int totalQuestions = 0;
    private int totalScore = 0;

    private void Start()
    {
        totalQuestions = QnA.Count; // QnA.Count will decrease everytime a question is answered
        TotalScore.text = "0/" + totalQuestions;
        //GoPanel.SetActive(false);
        generateQuestion();
    }

    public void retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GameOver()
    {
        //Quizpanel.SetActive(false);
        //GoPanel.SetActive(true);
    }

    public void correct()
    {
        //When the player is right
        totalScore += 1;
        QnA.RemoveAt(currentQuestion);
        TotalScore.text = totalScore + "/" + totalQuestions;
        StartCoroutine(waitForNext());
    }

    public void wrong()
    {
        //When the player is wrong
        QnA.RemoveAt(currentQuestion);
        StartCoroutine(waitForNext());
    }

    IEnumerator waitForNext()
    {
        yield return new WaitForSeconds(1);
        generateQuestion();
    }

    void SetAnswers()
    {
        for (int i = 0; i < options.Length; i++)
        {
            int index = i + 1; //for display purposes
            options[i].GetComponent<Text>().text = index + ") " + QnA[currentQuestion].Answers[i];
            optionButtons[i].GetComponent<AnswerScript>().isCorrect = false;  //set every option's correct value to be false first

            if (QnA[currentQuestion].CorrectAnswer == i) //if the correct option is correct, set the isCorrect boolean to true
            {
                optionButtons[i].GetComponent<AnswerScript>().isCorrect = true;
            }
        }
    }

    void generateQuestion()
    {
        if (QnA.Count > 0)
        {
            // currentQuestion = Random.Range(0, QnA.Count); for random ordering of questions

            for (int i = QnA.Count-1; i >= 0; i--) //display questions from ascending order
            {
                currentQuestion = i;
                QuestionTxt.text = QnA[currentQuestion].Question;
                SetAnswers();
            }
        }
        else
        {
            Debug.Log("Out of Questions");
            GameOver();
        }


    }
}
