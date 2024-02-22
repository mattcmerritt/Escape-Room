using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shader modifications from: https://www.youtube.com/watch?v=dIC4wbUgt5M

public class HideOnProximity : MonoBehaviour
{
    private List<MeshRenderer> renderers;
    private List<Color> originalColors;
    private float hiddenOpacity = 0.33f;
    private float transitionDuration = 1;
    private bool hidden;
    private List<Coroutine> activeCoroutines;

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
        if (Input.GetKeyDown(KeyCode.Space))
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
            
            if (!hidden)
            {
                for (int i = 0; i < renderers.Count; i++)
                {
                    activeCoroutines.Add(StartCoroutine(FadeOut(i)));
                    hidden = true;
                }
            }
            else if (hidden)
            {
                for (int i = 0; i < renderers.Count; i++)
                {
                    activeCoroutines.Add(StartCoroutine(FadeIn(i)));
                    hidden = false;
                }
            }
        }
    }

    public IEnumerator FadeOut(int i)
    {
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
    }

    public IEnumerator FadeIn(int i)
    {
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
    }
}
