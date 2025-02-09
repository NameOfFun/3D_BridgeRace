using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public Color Color { get; private set; }
    public void ChangeColor(Color newColor)
    {
        Color = newColor;
        GetComponent<Renderer>().material.color = newColor;
    }
}
