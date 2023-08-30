using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NoiseWeightItem", menuName = "New Noise Weight")]
public class NoiseWeightItem : ScriptableObject
{
    public float MaxNoiseWeight;
    public float MinNoiseWeight;
    public List<GameObject> Prefabs = new List<GameObject>();
}
