using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterUI : MonoBehaviour
{
    [SerializeField] Image _image;
    
    public void SetSprite(Sprite sprite)
    {
        _image.sprite = sprite;
    }
}
