using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Painter : MonoBehaviour
{
    private const int TexturePixelSize = 28;

    [SerializeField] private RawImage image;
    [SerializeField] private Button deleteButton;
    
    private Texture2D _texture;
    private PointerEventData _pointer;

    private void Start()
    {
        _texture = new Texture2D(TexturePixelSize, TexturePixelSize, TextureFormat.RGBA32, false)
        {
            filterMode = FilterMode.Point
        };
        image.texture = _texture;

        _pointer = new PointerEventData(EventSystem.current);

        ResetColors();
        deleteButton.onClick.AddListener(ResetColors);
    }

    private void OnDestroy()
    {
        if (_texture == null) return;
        Destroy(_texture);
        _texture = null;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0)) 
        {
            var results = new List<RaycastResult>();
            _pointer.position = Input.mousePosition;
            EventSystem.current.RaycastAll(_pointer, results);

            foreach (var target in results.Where(target => target.gameObject == image.gameObject))
            {
                Draw(target);
            }
        }
    }

    private void Draw(RaycastResult target)
    {
        var corners = new Vector3[4];
        image.rectTransform.GetWorldCorners(corners);
        corners[0] =  RectTransformUtility.WorldToScreenPoint(Camera.main, corners[0]);
        corners[2] = RectTransformUtility.WorldToScreenPoint(Camera.main, corners[2]);

        var width = corners[2].x - corners[0].x;
        var height = corners[2].y - corners[0].y;

        var x = (int)((target.screenPosition.x - corners[0].x) / width * TexturePixelSize);
        var y = (int)((target.screenPosition.y - corners[0].y) / height * TexturePixelSize);

        //_texture.SetPixel(x, y, Color.white);
        for (var i = -1; i <= 1; i++)
        {
            for (var j = -1; j <= 1; j++)
            {
                if(x + i < 0 || x + i >= TexturePixelSize || y + j < 0 || y + j >= TexturePixelSize) continue;
                _texture.SetPixel(x + i, y + j, Color.white);
            }
        }
        
        _texture.Apply();
    }

    private void ResetColors()
    {
        var colors = new Color[TexturePixelSize * TexturePixelSize];
        colors = colors.Select(color => Color.black).ToArray();
        
        _texture.SetPixels(colors);
        _texture.Apply();
    }
}
