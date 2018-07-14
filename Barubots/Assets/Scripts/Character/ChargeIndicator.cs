using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeIndicator : MonoBehaviour {

    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
    }

    public void SetValue(float value)
    {
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat("_AlphaValue", 1 - value);
        _renderer.SetPropertyBlock(_propBlock);
    }
}
