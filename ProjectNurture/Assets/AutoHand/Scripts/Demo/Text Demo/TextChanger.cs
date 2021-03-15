using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

namespace Autohand.Demo{
    public class TextChanger : MonoBehaviour{
        public TMPro.TextMeshPro text;
        Coroutine changing;
        Coroutine hide;
        GameObject lastCommandFrom;
        

        public void HideText(float changeTime, float fadeTime) {
            if(hide != null)
                StopCoroutine(hide);
            hide = StartCoroutine(HideTextRoutine(changeTime, fadeTime));
        }

        public void UpdateText(GameObject from, string newText, float changeTime) {
            if(lastCommandFrom != from || !newText.Equals(text.text)){ 
                lastCommandFrom = from;
                if(hide != null)
                    StopCoroutine(hide);
                if(changing != null)
                    StopCoroutine(changing);
                changing = StartCoroutine(ChangeText(changeTime, newText));
            }
        }

        IEnumerator ChangeText(float seconds, string newText) {
            seconds /= 2f;
            float totalTime = seconds;
            while(totalTime > 0) {
                text.alpha = Mathf.Sqrt(totalTime/seconds);
                totalTime -= Time.deltaTime;
                if(totalTime <= 0)
                    text.alpha = 0;
                yield return Time.deltaTime;
            }

            text.text = newText;

            totalTime = 0;
            while(totalTime < seconds) {
                text.alpha = Mathf.Sqrt(totalTime/seconds);
                totalTime += Time.deltaTime;
                if(totalTime >= seconds)
                    text.alpha = 1;
                yield return Time.deltaTime;
            }

        }

        IEnumerator HideTextRoutine(float seconds, float delay = 0) {
            yield return new WaitForSeconds(delay);
            float totalTime = seconds;
            while(totalTime > 0) {
                text.alpha = Mathf.Sqrt(totalTime/seconds);
                totalTime -= Time.deltaTime;
                if(totalTime <= 0)
                    text.alpha = 0;
                yield return Time.deltaTime;
            }

            text.text = "";
        }

        private void OnDestroy() {
            text.text = "";
        }
    }
}
