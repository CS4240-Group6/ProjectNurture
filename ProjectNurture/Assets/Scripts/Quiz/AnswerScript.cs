using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnswerScript : MonoBehaviour
{
    public bool isCorrect = false;
    public QuizManager quizManager;
    public AudioSource correctAnswerAudio;
    public AudioSource wrongAnswerAudio;

    public void Answer()
    {
        if (isCorrect)
        {
            Debug.Log("Correct Answer!");
            quizManager.correct();
            correctAnswerAudio.Play();
        }
        else
        {
            Debug.Log("Wrong Answer!!!");
            quizManager.wrong();
            wrongAnswerAudio.Play();
        }
    }

}
