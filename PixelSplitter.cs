using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PixelSplitter : MonoBehaviour
{
    public PrefabSet[] OverrideSets;

    public Texture2D PixelArt;

    public PrefabSet DefaultSet;
 
    public int SpawnOffset = 75;

    public float MinTime;
    public float MaxTime;

	private void Start ()
    {
        StartCoroutine(LoadMap());
	}
	
    private void EmptyMap()
    {
        while(transform.childCount > 0)
        {
            Transform trans = transform.GetChild(0);
            trans.BecomeBatman();
            Destroy(trans.gameObject); //Become The Joker
        }
    }

    private IEnumerator LoadMap()
    {
        yield return new WaitForSeconds(2.5f);
        EmptyMap();

        int width = PixelArt.width;
        int heigth = PixelArt.height;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < heigth; y++)
            {
                SpawnPrefabAt(x, y);
            }
        }
    }

    private void SpawnPrefabAt(int x, int y)
    {
        Color32 pixelColor = PixelArt.GetPixel(x, y);

        //Skip transparent pixels
        if(pixelColor.a <= 0)
        {
            return;
        }

        //Add offset to values to spawn prefabs at relative location to parent
        x += (int)transform.position.x;
        y += (int)transform.position.y;

        Vector3 targetPosition = new Vector3(x, y, transform.position.z);

        //Add random offset to spawn position to achieve scatter effect
        Vector3 offsetPosition = targetPosition + new Vector3(Random.Range(-SpawnOffset, SpawnOffset), Random.Range(0, SpawnOffset), Random.Range(-SpawnOffset, SpawnOffset));

        foreach (PrefabSet set in OverrideSets)
        {
            if(pixelColor.Equals(set.Color))
            {
                Transform trans = Instantiate(set.OverridePrefab, offsetPosition, Quaternion.identity).transform;
                StartMovement(trans, targetPosition);
                return;
            }
        }

        //Pixel color is not present in the override set array => Spawn default prefab with pixel color
        MeshRenderer defaultObj = Instantiate(DefaultSet.OverridePrefab, offsetPosition, Quaternion.identity).GetComponent<MeshRenderer>();
        defaultObj.material.color = pixelColor;
        StartMovement(defaultObj.transform, targetPosition);
    }

    private void StartMovement(Transform spawnedPrefab, Vector3 targetPosition)
    {
        spawnedPrefab.SetParent(transform);
        spawnedPrefab.DOMove(targetPosition, Random.Range(MinTime, MaxTime));
    }
}
