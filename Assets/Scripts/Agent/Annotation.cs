using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;

public class Annotation : MonoBehaviour
{
    Renderer[] renderers;
    public Camera annotator;
    public Canvas canvas;
    public LayerMask Transparent;
    public Dictionary<string, List<Tuple<Vector2, Vector2>>> rendererBounds = new Dictionary<string, List<Tuple<Vector2, Vector2>>>();

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        renderers = System.Array.FindAll(renderers, renderer => !renderer.CompareTag("Room"));
        foreach (Renderer renderer in renderers)
        {
            Debug.Log("Renderer belongs to GameObject: " + renderer.gameObject.name);
        }
        canvas.gameObject.layer = LayerMask.NameToLayer("Annotator");
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = annotator;
        Debug.Log("Screen Bounds: Width = " + Screen.width + ", Height = " + Screen.height);
        TakeScreenshot((byte[] screenshotBytes) => {
            Debug.Log("Screenshot taken");
        });
        DictionaryToJson(rendererBounds);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Renderer[] GetVisibleRenderers()
    {
        if (annotator == null)
        {

            Debug.LogError("Annotator camera is not set");
            return null;
        }
        List<Renderer> visibleRenderers = new List<Renderer>();

        for (int x = 0; x < Screen.width; x += 5)
        {
            for (int y = 0; y < Screen.height; y += 5)
            {
                Ray ray = annotator.ScreenPointToRay(new Vector3(x, y, 0));
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Renderer renderer = hit.collider.GetComponent<Renderer>();
                    if (renderer != null && !renderer.CompareTag("Room"))
                    {
                        if (!visibleRenderers.Contains(renderer))
                        {
                            Debug.Log("Hit: " + renderer.gameObject.name);
                            visibleRenderers.Add(renderer);
                        }

                        if (renderer.CompareTag("Transparent"))
                        {
                            Debug.Log("Hit transparent: " + renderer.gameObject.name + " Parent: " + renderer.transform.parent.gameObject.name);
                            if (Physics.Raycast(ray, out RaycastHit behindHit, 1000, ~Transparent))
                            {
                                Debug.Log("Hit behind: " + behindHit.collider.gameObject.name);
                                Renderer behindRenderer = behindHit.collider.GetComponent<Renderer>();
                                if (behindRenderer != null && !renderer.CompareTag("Room"))
                                {
                                    if (!visibleRenderers.Contains(behindRenderer))
                                    {
                                        visibleRenderers.Add(behindRenderer);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return visibleRenderers.ToArray();
    }
    void Annotate(Renderer[] renderers)
    {
        rendererBounds.Clear();
        
        if (annotator == null)
            return;
        foreach (Renderer renderer in renderers)
        {
            Bounds bounds = renderer.bounds;
            Vector3[] vertices = new Vector3[8];

            vertices[0] = annotator.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.min.z));
            vertices[1] = annotator.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.min.z));
            vertices[2] = annotator.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.min.z));
            vertices[3] = annotator.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.min.z));
            vertices[4] = annotator.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, bounds.max.z));
            vertices[5] = annotator.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, bounds.max.z));
            vertices[6] = annotator.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, bounds.max.z));
            vertices[7] = annotator.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, bounds.max.z));

            Vector3 min = vertices[0];
            Vector3 max = vertices[0];

            foreach (Vector3 vertex in vertices)
            {
                min = Vector3.Min(min, vertex);
                max = Vector3.Max(max, vertex);
            }

            // Store the renderer name and min and max as dictionary in a public variable
            if (!rendererBounds.ContainsKey(renderer.gameObject.name))
            {
                rendererBounds[renderer.gameObject.name] = new List<Tuple<Vector2, Vector2>>();
            }
            Tuple<Vector2, Vector2> boundingBox = new Tuple<Vector2, Vector2>(
                new Vector2(
                    (int)Mathf.Clamp(annotator.ScreenToViewportPoint(new Vector2(min.x, min.y)).x * Screen.width, 0, Screen.width),
                    (int)Mathf.Clamp(annotator.ScreenToViewportPoint(new Vector2(min.x, min.y)).y * Screen.height, 0, Screen.height)
                ),
                new Vector2(
                    (int)Mathf.Clamp(annotator.ScreenToViewportPoint(new Vector2(max.x, max.y)).x * Screen.width, 0, Screen.width),
                    (int)Mathf.Clamp(annotator.ScreenToViewportPoint(new Vector2(max.x, max.y)).y * Screen.height, 0, Screen.height)
                )
            );

            rendererBounds[renderer.gameObject.name].Add(boundingBox);
            Debug.Log(renderer.gameObject.name + " min: " + new Vector2(
                    (int)Mathf.Clamp(annotator.ScreenToViewportPoint(new Vector2(min.x, min.y)).x * Screen.width, 0, Screen.width),
                    (int)Mathf.Clamp(annotator.ScreenToViewportPoint(new Vector2(min.x, min.y)).y * Screen.height, 0, Screen.height)
                ) + " max: " + new Vector2(
                    (int)Mathf.Clamp(annotator.ScreenToViewportPoint(new Vector2(max.x, max.y)).x * Screen.width, 0, Screen.width),
                    (int)Mathf.Clamp(annotator.ScreenToViewportPoint(new Vector2(max.x, max.y)).y * Screen.height, 0, Screen.height)
                ));
        }

    }

    public string DictionaryToJson(Dictionary<string, List<Tuple<Vector2, Vector2>>> dict)
    {
        string json = "{";
        foreach (KeyValuePair<string, List<Tuple<Vector2, Vector2>>> entry in dict)
        {
            json += "\"" + entry.Key + "\": [";
            foreach (var tuple in entry.Value)
            {
                json += "{\"bbox\": [" + tuple.Item1.x + ", " + tuple.Item1.y + ", " + tuple.Item2.x + ", " + tuple.Item2.y + "]},";
            }
            json = json.TrimEnd(',') + "],";
        }
        json = json.TrimEnd(',');
        json += "}";
        System.IO.File.WriteAllText(Application.dataPath + "/Scripts/Test.json", json);
        return json;
    }

    public IEnumerator TakeScreenshotCoroutine(Action<byte[]> callback)
    {
        yield return new WaitForEndOfFrame();

        if (annotator == null)
        {
            Debug.LogError("Annotator camera is not set");
            yield break;
        }

        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        annotator.targetTexture = renderTexture;
        Texture2D screenshotTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        annotator.Render();
        RenderTexture.active = renderTexture;
        screenshotTexture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);

        int thickness = 2; // Thickness of the border
        foreach (var list in rendererBounds.Values)
        {
            foreach (var bounds in list)
            {
                for (int t = 0; t < thickness; t++)
                {
                    for (int x = (int)bounds.Item1.x - t; x <= bounds.Item2.x + t; x++)
                    {
                        screenshotTexture.SetPixel(x, (int)bounds.Item1.y - t, Color.red);
                        screenshotTexture.SetPixel(x, (int)bounds.Item2.y + t, Color.red);
                    }

                    for (int y = (int)bounds.Item1.y - t; y <= (int)bounds.Item2.y + t; y++)
                    {
                        screenshotTexture.SetPixel((int)bounds.Item1.x - t, y, Color.red);
                        screenshotTexture.SetPixel((int)bounds.Item2.x + t, y, Color.red);
                    }
                }
            }
        }

        screenshotTexture.Apply();

        annotator.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        string filePath = Application.dataPath + "/Screenshot.png";
        System.IO.File.WriteAllBytes(filePath, screenshotTexture.EncodeToPNG());
        Debug.Log("Screenshot saved to: " + filePath);

        byte[] screenshotBytes = screenshotTexture.EncodeToPNG();
        callback(screenshotBytes);

        Destroy(screenshotTexture);
    }

    public void TakeScreenshot(Action<byte[]> callback)
    {
        Annotate(GetVisibleRenderers());
        StartCoroutine(TakeScreenshotCoroutine(callback));
    }
}