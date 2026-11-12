using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// code heavily based on https://github.com/UnityNinja/drawable-Raw-Image-for-Unity
// small adjustments were done to DrawLineTo() and DrawPixel() to erase gaps in lines

public class DrawOnRawImage : MonoBehaviour
{
    [SerializeField] RawImage drawImage;
    private int scaleFactor;

    private Texture2D canvasTexture;
    private Color32[] canvasColors;
    private Vector2 previousPosition;
    private Texture2D originalTexture;

    [SerializeField] private float brushSize = 10f;
    [SerializeField] private Color brushColor = Color.black;

    public event Action<DrawOnRawImage, Vector2[]> StrokeCompleted;
    private readonly List<Vector2> currentStroke = new List<Vector2>(); // list of points
    private bool isDrawing;

    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        // Create a Texture2D that matches the RawImage size (568x568)
        int width = (int)drawImage.GetComponent<RectTransform>().sizeDelta.x;
        int height = (int)drawImage.GetComponent<RectTransform>().sizeDelta.y;
        originalTexture = new Texture2D(width, height);

        // fill the texture with a certain  background
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
        {
            if(i < width*10 || i > width*width-width*10 || i % width > 0 && i % width < 10 || (i - (width-1)) % width <= 10 || (i - (width-1)) % width >= width - 10)
            {
                pixels[i] = Color.gray; // set the pixel color to gray
            }
            else if(i % (width/2) > 0 && i % (width/2) < 5 || i > width*(width/2-2) && i < width*(width/2+3))
            {
                pixels[i] = new Color(0.8f, 0.8f, 0.8f); // set the pixel color to light gray
            }
            else
            {
                pixels[i] = Color.white; // set the pixel color to white
            }
        }

        originalTexture.SetPixels(pixels);
        originalTexture.Apply(); // apply the background color

        // Assign the texture to the RawImage
        drawImage.texture = originalTexture;

        // calculate scaleFactor automatically based on the width of the RawImage and the original texture
        float widthRatio = drawImage.rectTransform.rect.width / (float)originalTexture.width;
        float heightRatio = drawImage.rectTransform.rect.height / (float)originalTexture.height;
        scaleFactor = Mathf.RoundToInt(Mathf.Max(widthRatio, heightRatio));

        canvasTexture = new Texture2D(originalTexture.width * scaleFactor, originalTexture.height * scaleFactor, TextureFormat.RGBA32, false);

        canvasTexture.filterMode = FilterMode.Point;
        canvasTexture.wrapMode = TextureWrapMode.Clamp;

        // Create a new array of size equal to canvasTexture.width multiplied by canvasTexture.height
        canvasColors = new Color32[canvasTexture.width * canvasTexture.height];

        // Copy the pixels from the original texture to the new array
        for (int y = 0; y < originalTexture.height; y++)
        {
            for (int x = 0; x < originalTexture.width; x++)
            {
                Color32 pixel = originalTexture.GetPixel(x, y);
                for (int i = 0; i < scaleFactor; i++)
                {
                    for (int j = 0; j < scaleFactor; j++)
                    {
                        int index = ((y * scaleFactor) + j) * canvasTexture.width + ((x * scaleFactor) + i);
                        canvasColors[index] = pixel;
                    }
                }
            }
        }

        canvasTexture.SetPixels32(canvasColors);
        canvasTexture.Apply();

        drawImage.texture = canvasTexture;
    }

    // stroke detection
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsCursorInside())
        {
            BeginStroke(GetLocalCursor());
        }

        if (!isDrawing)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            AddStrokePoint(GetLocalCursor());
        }

        if (Input.GetMouseButtonUp(0))
        {
            AddStrokePoint(GetLocalCursor());
            EndStroke();
        }
    }

    private void BeginStroke(Vector2 position)
    {
        isDrawing = true;
        currentStroke.Clear();

        currentStroke.Add(position);
        previousPosition = position;

        // Ein kurzer Klick soll ebenfalls einen sichtbaren Punkt erzeugen.
        DrawPixel(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.y),
            brushColor,
            brushSize
        );

        ApplyCanvas();
    }

    private void AddStrokePoint(Vector2 position)
    {
        // Identische aufeinanderfolgende Punkte nicht mehrfach speichern.
        if ((position - previousPosition).sqrMagnitude < 0.0001f)
        {
            return;
        }

        currentStroke.Add(position);
        DrawLineTo(position);
    }

    private void EndStroke()
    {
        isDrawing = false;

        // Eigenständige Kopie: Der nächste Strich darf diese Daten nicht verändern.
        Vector2[] completedStroke = currentStroke.ToArray();
        currentStroke.Clear();

        StrokeCompleted?.Invoke(this, completedStroke);
    }

    private Vector2 GetLocalCursor()
    {
        RectTransform rectTransform = drawImage.rectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            Input.mousePosition,
            GetCanvasCamera(),
            out Vector2 localCursor
        );

        Rect rect = rectTransform.rect;

        // map local UI coordinates to texture coordinates
        // works independently of the pivot and the texture resolution
        float x = (localCursor.x - rect.xMin) / rect.width;
        float y = (localCursor.y - rect.yMin) / rect.height;

        return new Vector2(
            x * (canvasTexture.width - 1),
            y * (canvasTexture.height - 1)
        );
    }

    // set camera (to calculate stroke points correctly)
    private Camera GetCanvasCamera()
    {
        Canvas canvas = drawImage.canvas;

        if (canvas == null ||
            canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            return null;
        }

        return canvas.worldCamera;
    }

    private bool IsCursorInside()
    {
        return RectTransformUtility.RectangleContainsScreenPoint(
            drawImage.rectTransform,
            Input.mousePosition,
            GetCanvasCamera()
        );
    }

    // draw on image
    private void DrawLineTo(Vector2 currentPosition)
    {
        float distance = Vector2.Distance(
            previousPosition,
            currentPosition
        );

        int steps = Mathf.Max(
            1,
            Mathf.CeilToInt(distance / 0.5f) // smaller step size for smoother lines
        );

        // draw multiple points between the previous and current positions
        for (int i = 0; i <= steps; i++)
        {
            Vector2 position = Vector2.Lerp(
                previousPosition,
                currentPosition,
                i / (float)steps
            );

            DrawPixel(
                Mathf.RoundToInt(position.x),
                Mathf.RoundToInt(position.y),
                brushColor,
                brushSize
            );
        }

        // update the canvas and apply the changes
        ApplyCanvas();
        previousPosition = currentPosition;
    }

    private void DrawPixel(int x, int y, Color color, float size)
    {
        int width = canvasTexture.width;
        int height = canvasTexture.height;
        int radius = Mathf.CeilToInt(size / 2); // radius depends on brush size

        // Draw a circle around the pixel based on the brush size (ensures thick lines)
        for (int i = x - radius; i <= x + radius; i++)
        {
            if (i < 0 || i >= width) continue;

            for (int j = y - radius; j <= y + radius; j++)
            {
                if (j < 0 || j >= height) continue;

                // Only set pixels within the circle area to avoid messy lines
                if (Vector2.Distance(new Vector2(i, j), new Vector2(x, y)) <= radius)
                {
                    int index = j * width + i;
                    // Ensure the color has an alpha value (preserving transparency)
                    Color32 c = color;
                    c.a = canvasColors[index].a;
                    canvasColors[index] = c;
                }
            }
        }
    }

    private void ApplyCanvas()
    {
        canvasTexture.SetPixels32(canvasColors);
        canvasTexture.Apply();
    }

    public void Repaint()
    {
        isDrawing = false;
        currentStroke.Clear();

        int width = (int)drawImage.GetComponent<RectTransform>().sizeDelta.x;

        // Reset the canvasColors array
        for (int i = 0; i < canvasColors.Length; i++)
        {
            if(i < width*10 || i > width*width-width*10 || i % width > 0 && i % width < 10 || (i - (width-1)) % width <= 10 || (i - (width-1)) % width >= width - 10)
            {
                canvasColors[i] = Color.gray; // Set the background color to gray
            }
            else if(i % (width/2) > 0 && i % (width/2) < 5 || i > width*(width/2-2) && i < width*(width/2+3))
            {
                canvasColors[i] = new Color(0.8f, 0.8f, 0.8f); // Set the background color to light gray
            }
            else
            {
                canvasColors[i] = Color.white; // Set the background color to white
            }
        }
        // Apply the cleared colors to the canvas texture
        canvasTexture.SetPixels32(canvasColors);
        canvasTexture.Apply(); // Refresh the texture
    }

    private void OnDisable()
    {
        isDrawing = false;
        currentStroke.Clear();
    }

    public void DisplayFeaturePoints(Vector2[] featurePoints)
    {
        foreach (Vector2 point in featurePoints)
        {
            DrawPixel(
                Mathf.RoundToInt(point.x),
                Mathf.RoundToInt(point.y),
                Color.red,
                14f
            );
        }

        canvasTexture.SetPixels32(canvasColors);
        canvasTexture.Apply();
    }

}