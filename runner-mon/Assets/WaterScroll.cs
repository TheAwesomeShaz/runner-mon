using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterScroll : MonoBehaviour
{
    public float scrollX = 0f;
    public float scrollY = 01f;

    // Update is called once per frame
    void Update()
    {
        float OffsetX = Time.time * scrollX;
        float OffsetY = Time.time * scrollY;
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(OffsetX, OffsetY);
    }
}
