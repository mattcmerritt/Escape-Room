using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBetween : MonoBehaviour
{
    [SerializeField] private Vector3 positionA, positionB;
    private float transitionDuration = 1;
    private Coroutine activeCoroutine;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            //Debug.Log("Forward");
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }
            activeCoroutine = StartCoroutine(MoveToPoint(positionA, positionB));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            //Debug.Log("Backwards");
            if (activeCoroutine != null)
            {
                StopCoroutine(activeCoroutine);
            }
            activeCoroutine = StartCoroutine(MoveToPoint(positionB, positionA));
        }
    }

    private IEnumerator MoveToPoint(Vector3 start, Vector3 target)
    {
        // transition time depends on how far the previous transition was
        float timeElapsed = (transform.position - start).magnitude / (target - start).magnitude * transitionDuration;

        while (timeElapsed < transitionDuration)
        {
            transform.position = new Vector3(
                Mathf.Lerp(start.x, target.x, timeElapsed / transitionDuration),
                Mathf.Lerp(start.y, target.y, timeElapsed / transitionDuration),
                Mathf.Lerp(start.z, target.z, timeElapsed / transitionDuration)
            );

            timeElapsed += Time.deltaTime;

            yield return null;
        }
    }
}
