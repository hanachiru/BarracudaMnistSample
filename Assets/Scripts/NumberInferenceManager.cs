using Unity.Barracuda;
using UnityEngine;
using UnityEngine.UI;

public class NumberInferenceManager : MonoBehaviour
{
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
         var pixels = new float[28, 28];

         for (var i = 0; i < 28 * 28; i++)
         {
             pixels[i % 28, i / 28] = colors[i].r * 0.299f + colors[i].g * 0.587f + colors[i].b * 0.114f;
         }

         var result = _mnistInferencer.Execute(pixels);
         numberText.text = result.Number.ToString();
         scoreText.text = result.Score.ToString();
    }
}
