using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private GameObject border;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetInvisible(border);
    }

    private void SetInvisible(GameObject gameObject)
    {
        // Tắt Renderer
        var render = gameObject.GetComponent<Renderer>();
        if (render != null)
        {
            render.enabled = false;
        }

        // Đảm bảo Collider hoạt động
        var collider = gameObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }
    }

    void SetTransparent(GameObject obj)
    {
        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            foreach (var material in renderer.materials)
            {
                material.SetFloat("_Mode", 2); // Đặt chế độ Fade
                material.color = new Color(material.color.r, material.color.g, material.color.b, 0f); // Alpha = 0 (trong suốt)
            }
        }
    }

}
