using System;
using UnityEngine;
using UnityEngine.UI;

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

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 currentPosition = GetLocalCursor();
            DrawLineTo(currentPosition);
        }
        else
        {
            previousPosition = Vector2.zero;
        }
    }

    private void DrawLineTo(Vector2 currentPosition)
    {
        if (previousPosition == Vector2.zero)
        {
            previousPosition = currentPosition;
            return;
        }

        float distance = Vector2.Distance(previousPosition, currentPosition);
        int steps = Mathf.CeilToInt(distance / 0.5f); // Smaller step size for smoother lines

        // Draw multiple points between the previous and current positions
        for (int i = 0; i <= steps; i++)
        {
            Vector2 lerpedPosition = Vector2.Lerp(previousPosition, currentPosition, i / (float)steps);
            DrawPixel((int)lerpedPosition.x, (int)lerpedPosition.y, brushColor, brushSize);
        }

        // Update the canvas and apply the changes
        canvasTexture.SetPixels32(canvasColors);
        canvasTexture.Apply();
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

    private Vector2 GetLocalCursor()
    {
        Vector2 cursor = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(drawImage.rectTransform, cursor, null, out Vector2 localCursor);
        localCursor += new Vector2(canvasTexture.width / 2f, canvasTexture.height / 2f);
        return localCursor;
    }

    public void Repaint()
    {
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

}