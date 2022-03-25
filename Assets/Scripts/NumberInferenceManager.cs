using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class NumberInferenceManager : MonoBehaviour
{
    private const int TexturePixelSize = 28;
    
    [SerializeField] private NNModel model;
    [SerializeField] private RawImage image;
    [SerializeField] private Text numberText;
    [SerializeField] private Text scoreText;
    
    private MnistInferencer _mnistInferencer;

    private void Start()
    {
        _mnistInferencer = new MnistInferencer(model);
    }

    private void OnDestroy()
    {
        _mnistInferencer.Dispose();
    }

    private void Update()
    {
         var colors = (image.texture as Texture2D).GetPixels();
         var pixels = new float[TexturePixelSize, TexturePixelSize];

         for (var i = 0; i < TexturePixelSize * TexturePixelSize; i++)
         {
             pixels[i % TexturePixelSize, i / TexturePixelSize] = colors[i].grayscale;
         }

         var result = _mnistInferencer.Execute(pixels);
         numberText.text = result.Number.ToString();
         scoreText.text = result.Score.ToString();
    }
}
