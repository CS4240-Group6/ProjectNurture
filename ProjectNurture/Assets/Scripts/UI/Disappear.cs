using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappear : MonoBehaviour
{
    public float disappearTimer = 3f; // gameObject is inactive after 3 seconds

    void OnEnable()
    {
        StartCoroutine(HideAfterTimeout(disappearTimer));
    }

    private IEnumerator HideAfterTimeout(float time)
    {
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }
}
