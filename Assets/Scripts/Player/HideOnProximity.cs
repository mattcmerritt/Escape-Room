using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shader modifications from: https://www.youtube.com/watch?v=dIC4wbUgt5M

public class HideOnProximity : MonoBehaviour
{
    // internal fade in/out data
    private List<MeshRenderer> renderers;
    private List<Color> originalColors;
    private float hiddenOpacity = 0.33f;
    private float transitionDuration = 1;
    private float fadeInDelay = 0.5f;
    private bool isHiding;
    [SerializeField] private bool modelHidden;
    private List<Coroutine> activeCoroutines;
    private Coroutine fadeInDelayCoroutine;

    // camera and raycasting
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private LayerMask bothLayer;
    [SerializeField] private float fadeOutDistance;

    private void Start()
    {
        renderers = new List<MeshRenderer>(GetComponentsInChildren<MeshRenderer>());
        originalColors = new List<Color>();
        foreach (MeshRenderer renderer in renderers)
        {
            originalColors.Add(renderer.material.color);
        }
        activeCoroutines = new List<Coroutine>();
    }

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(cameraObject.transform.position, cameraObject.transform.forward, out hit, fadeOutDistance, bothLayer))
        {
            //Debug.Log($"Hit {hit.collider.name}");
            HideOnProximity hider = hit.collider.gameObject.GetComponent<HideOnProximity>();
            hider.MarkAsHidden();
        }

        if (modelHidden)
        {
            gameObject.layer = LayerMask.NameToLayer("Hidden Players");
            foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = LayerMask.NameToLayer("Hidden Players");
            }
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Players");
            foreach (Transform child in gameObject.GetComponentsInChildren<Transform>())
            {
                child.gameObject.layer = LayerMask.NameToLayer("Players");
            }
        }
    }

    public void MarkAsHidden()
    {
        // stop any fade in coroutines
        if (!isHiding)
        {
            while (activeCoroutines.Count > 0)
            {
                if (activeCoroutines[0] != null)
                {
                    StopCoroutine(activeCoroutines[0]);
                    activeCoroutines.RemoveAt(0);
                }
                else
                {
                    activeCoroutines.RemoveAt(0);
                }
            }
        }

        // start fade out coroutines if not active
        if (!isHiding)
        {
            //Debug.Log("Unhiding!");
            for (int i = 0; i < renderers.Count; i++)
            {
                activeCoroutines.Add(StartCoroutine(FadeOut(i)));
            }
            fadeInDelayCoroutine = StartCoroutine(WaitForFadeIn());
            isHiding = true;
        }
        // else reset fade in delay
        else
        {
            //Debug.Log("Continue waiting!");
            if (fadeInDelayCoroutine != null)
            {
                StopCoroutine(fadeInDelayCoroutine);
                fadeInDelayCoroutine = StartCoroutine(WaitForFadeIn());
            }
        }
    }

    public IEnumerator WaitForFadeIn()
    {
        // wait until it has been sufficiently long without player blocking
        yield return new WaitForSeconds(fadeInDelay);

        // stop any fade out coroutines
        while (activeCoroutines.Count > 0)
        {
            if (activeCoroutines[0] != null)
            {
                StopCoroutine(activeCoroutines[0]);
                activeCoroutines.RemoveAt(0);
            }
            else
            {
                activeCoroutines.RemoveAt(0);
            }
        }

        // starting fade in coroutines
        for (int i = 0; i < renderers.Count; i++)
        {
            activeCoroutines.Add(StartCoroutine(FadeIn(i)));
        }
        isHiding = false;
    }

    public IEnumerator FadeOut(int i)
    {
        //Debug.Log("Fading out!");

        renderers[i].material.SetInt("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
        renderers[i].material.SetInt("_DstBlend", (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        renderers[i].material.SetInt("_ZWrite", 0);
        //renderers[i].material.EnableKeyword("_ALPHAPREMULTIPLY_ON"); // enable for fade
        renderers[i].material.EnableKeyword("_ALPHABLEND_ON"); // enable for transparent
        renderers[i].material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;

        if (renderers[i].material.HasProperty("_Color"))
        {
            // transition time depends on how far the previous transition was
            float timeElapsed = (originalColors[i].a - renderers[i].material.color.a) / (originalColors[i].a - hiddenOpacity) * transitionDuration;

            while (timeElapsed < transitionDuration)
            {
                renderers[i].material.color = new Color(
                    renderers[i].material.color.r,
                    renderers[i].material.color.g,
                    renderers[i].material.color.b,
                    Mathf.Lerp(originalColors[i].a, hiddenOpacity, timeElapsed / transitionDuration) 
                );

                timeElapsed += Time.deltaTime;

                yield return null;
            }
        }

        modelHidden = true;
    }

    public IEnumerator FadeIn(int i)
    {
        //Debug.Log("Fading in!");

        if (renderers[i].material.HasProperty("_Color"))
        {
            // transition time depends on how far the previous transition was
            float timeElapsed = (renderers[i].material.color.a - hiddenOpacity) / (originalColors[i].a - hiddenOpacity) * transitionDuration;

            while (timeElapsed < transitionDuration)
            {
                renderers[i].material.color = new Color(
                    renderers[i].material.color.r,
                    renderers[i].material.color.g,
                    renderers[i].material.color.b,
                    Mathf.Lerp(hiddenOpacity, originalColors[i].a, timeElapsed / transitionDuration)
                );

                timeElapsed += Time.deltaTime;

                yield return null;
            }
        }

        renderers[i].material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        renderers[i].material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        renderers[i].material.SetInt("_ZWrite", 1);
        //renderers[i].material.DisableKeyword("_ALPHAPREMULTIPLY_ON"); // enable for fade
        renderers[i].material.DisableKeyword("_ALPHABLEND_ON"); // enable for transparent
        renderers[i].material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

        modelHidden = false;
    }
}
