using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{

    [SerializeField] private AnimationCurve BlendCurve;

    [SerializeField] private GameObject DecoPrefab;

    private void Start()
    {
        if (!DecoPrefab)
            return;
        
        SetRandomRotation();
    }

    private void SetRandomRotation()
    {
        int yRot = Random.Range(-360, 360);
        DecoPrefab.transform.eulerAngles = new Vector3(0, yRot, 0);
    }

    public void SetBlendShapes(float noiseValue)
    {
        float blendValue = (1.0f - noiseValue) * 100;

        SkinnedMeshRenderer renderer = GetComponent<SkinnedMeshRenderer>();
        renderer.SetBlendShapeWeight(0, BlendCurve.Evaluate(blendValue));
        //renderer.SetBlendShapeWeight(1, BlendCurve.Evaluate(blendValue));
    }
}
