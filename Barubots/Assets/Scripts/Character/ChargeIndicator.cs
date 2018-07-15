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

    public void SetColors(Vector4[] colors)
    {
        _renderer.GetPropertyBlock(_propBlock);
        SetCycle(0);
        _propBlock.SetVectorArray("colorIndicators", colors);
        _renderer.SetPropertyBlock(_propBlock);
    }

    // Will update the animation of the charge
    public void SetValue(float value)
    {
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat("_AlphaValue", 0.95f - value);
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void GoTeNextCycle(int cycle)
    {
        _renderer.GetPropertyBlock(_propBlock);
        SetCycle(cycle);
        _renderer.SetPropertyBlock(_propBlock);
    }

    public void Reset()
    {
        SetValue(0);
        _renderer.GetPropertyBlock(_propBlock);
        SetCycle(0);
        _renderer.SetPropertyBlock(_propBlock);
    }

    private void SetCycle(int cycle)
    {
        _propBlock.SetInt("currentCharge", cycle);
    }
}
