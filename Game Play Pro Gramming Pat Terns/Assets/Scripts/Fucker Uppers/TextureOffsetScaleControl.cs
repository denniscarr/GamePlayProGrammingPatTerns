using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureOffsetScaleControl : MonoBehaviour {

    [SerializeField] FloatRange offsetXRange;
    [SerializeField] FloatRange offsetYRange;

    [SerializeField] FloatRange scaleXRange;
    [SerializeField] FloatRange scaleYRange;

    Material m_Material;


    private void Start() {
        m_Material = GetComponent<MeshRenderer>().material;
    }


    private void Update() {
        m_Material.mainTextureOffset = new Vector2(
                offsetXRange.Random,
                offsetXRange.Random
            );

        m_Material.mainTextureScale = new Vector2(
                scaleXRange.Random,
                scaleYRange.Random
            );
    }
}
