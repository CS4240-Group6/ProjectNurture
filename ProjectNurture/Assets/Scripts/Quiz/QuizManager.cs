using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    public List<QuestionAndAnswers> QnA;
    public GameObject[] options;
    public GameObject[] optionButtons;
    public int currentQuestion;

    //public GameObject Quizpanel;
    //public GameObject GoPanel;

    public Text QuestionTxt;
    public Text ScoreTxt;

    int totalQuestions = 0;
    public int score;

    private void Start()
    {
        totalQuestions = QnA.Count;
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
        //ScoreTxt.text = score + "/" + totalQuestions;
    }

    public void correct()
    {
        //When the player is right
        score += 1;
        QnA.RemoveAt(currentQuestion);
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
            currentQuestion = Random.Range(0, QnA.Count);

            QuestionTxt.text = QnA[currentQuestion].Question;
            SetAnswers();
        }
        else
        {
            Debug.Log("Out of Questions");
            GameOver();
        }


    }
}
