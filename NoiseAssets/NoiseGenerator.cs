using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField] private int Width = 256;
    [SerializeField] private int Height = 256;

    [SerializeField] private float Scale = 20f;

    [SerializeField] private float OffsetX;
    [SerializeField] private float OffsetY;

    [SerializeField] private int MinIslandSize = 10;

    [Header("Object Spawner")] [SerializeField]
    private List<NoiseWeightItem> NoiseWeights = new List<NoiseWeightItem>();

    private Dictionary<Vector3, Transform> _islandChunks = new Dictionary<Vector3, Transform>();

    [SerializeField] private GameObject IslandParent;

    private List<Transform> _islands = new List<Transform>();

    private Queue<Transform> _tilesToProcess = new Queue<Transform>();

    private Transform _lastIsland;
    private Transform _lastTile;

    private GameObject _prefab;

    private void Start()
    {
        _islandChunks.Clear();
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                foreach (NoiseWeightItem weight in NoiseWeights)
                {
                    float compareValue = GetNoiseSample(x, y);
                    
                    if(weight.MaxNoiseWeight < compareValue || weight.MinNoiseWeight > compareValue)
                        continue;
                    
                    Vector3 spawnPos = new Vector3(x, 0, y);

                    float rand = Random.Range(0.00f, 1.00f);
                    rand = Mathf.Round(rand * 100f) / 100f;

                    if (rand <= 0.95f)
                    {
                        _prefab = weight.Prefabs[0];
                    }

                    else
                    {
                        int r = Random.Range(0, 100);

                        _prefab = r > 50 ? weight.Prefabs[1] : weight.Prefabs[2];
                    }
                    
                    _lastTile = Instantiate(_prefab, spawnPos, Quaternion.identity).transform;
                    _lastTile.name = x + "_" + y;
                    _lastTile.SetParent(transform);
                    _lastTile.GetComponent<Tile>().SetBlendShapes(compareValue);
                    _islandChunks.Add(spawnPos, _lastTile);
                    
                }
            }
        }

        //ProcessIslands();
        ProcessMap();
    }

    private void ProcessMap()
    {
        do
        {
            Transform firstTile = transform.GetChild(0);
            Transform island = Instantiate(IslandParent, firstTile.position, Quaternion.identity).transform;
            _islands.Add(island);
            
            _tilesToProcess.Enqueue(firstTile);

            while (_tilesToProcess.Count > 0)
            {
                Transform tile = _tilesToProcess.Dequeue();
                tile.SetParent(island);
                Vector3 tilePos = tile.position;

                _islandChunks.Remove(tilePos);
                
                //Get neighbors that have not been evaluated yet!
                Vector3 leftNeighbor = new Vector3(tilePos.x - 1, tilePos.y, tilePos.z);
                if (_islandChunks.ContainsKey(leftNeighbor))
                {
                    if(!_tilesToProcess.Contains(_islandChunks[leftNeighbor]))
                        _tilesToProcess.Enqueue(_islandChunks[leftNeighbor]);
                    
                }
                
                Vector3 rightNeighbor = new Vector3(tilePos.x + 1, tilePos.y, tilePos.z);
                if (_islandChunks.ContainsKey(rightNeighbor))
                {
                    if(!_tilesToProcess.Contains(_islandChunks[rightNeighbor]))
                        _tilesToProcess.Enqueue(_islandChunks[rightNeighbor]);
                }
                    
                Vector3 topNeighbor = new Vector3(tilePos.x, tilePos.y, tilePos.z + 1);
                if (_islandChunks.ContainsKey(topNeighbor))
                {
                    if(!_tilesToProcess.Contains(_islandChunks[topNeighbor]))
                        _tilesToProcess.Enqueue(_islandChunks[topNeighbor]);
                }

                Vector3 bottomNeighbor = new Vector3(tilePos.x, tilePos.y, tilePos.z - 1);
                if (_islandChunks.ContainsKey(bottomNeighbor))
                {
                    if(!_tilesToProcess.Contains(_islandChunks[bottomNeighbor]))
                        _tilesToProcess.Enqueue(_islandChunks[bottomNeighbor]);
                }
            }

        } while (transform.childCount > 0);
        
        ProcessIslands();
    }
    
    private void ProcessTile(Transform tile)
    {
        //Does the spawned tile have a neighbor? Yes? => Add to that neighbors island No? => Spawn new island!
        if (HasNeighbors(tile))
        {
            tile.SetParent(_islandChunks[GetTileNeighbor(tile)].parent);
            return;
        }
        
        _lastIsland = Instantiate(IslandParent, tile.position, Quaternion.identity).transform;
        tile.SetParent(_lastIsland);
        _islands.Add(_lastIsland);
    }

    private Vector3 GetTileNeighbor(Transform tile)
    {
        Vector3 tilePos = tile.position;
        
        Vector3 leftNeighbor = new Vector3(tilePos.x - 1, tilePos.y, tilePos.z);
        if (_islandChunks.ContainsKey(leftNeighbor))
            return leftNeighbor;
        
        Vector3 topLeftNeighbor = new Vector3(tilePos.x - 1, tilePos.y, tilePos.z + 1);
        if (_islandChunks.ContainsKey(topLeftNeighbor))
            return topLeftNeighbor;
        
        Vector3 rightNeighbor = new Vector3(tilePos.x + 1, tilePos.y, tilePos.z);
        if (_islandChunks.ContainsKey(rightNeighbor))
            return rightNeighbor;
        
        Vector3 topRightNeighbor = new Vector3(tilePos.x + 1, tilePos.y, tilePos.z + 1);
        if (_islandChunks.ContainsKey(topRightNeighbor))
            return topRightNeighbor;
        
        Vector3 topNeighbor = new Vector3(tilePos.x, tilePos.y, tilePos.z + 1);
        if (_islandChunks.ContainsKey(topNeighbor))
            return topNeighbor;
        
        Vector3 bottomRightNeighbor = new Vector3(tilePos.x + 1, tilePos.y, tilePos.z - 1);
        if (_islandChunks.ContainsKey(bottomRightNeighbor))
            return bottomRightNeighbor;
        
        Vector3 bottomNeighbor = new Vector3(tilePos.x, tilePos.y, tilePos.z - 1);
        if (_islandChunks.ContainsKey(bottomNeighbor))
            return bottomNeighbor;
        
        Vector3 bottomLeftNeighbor = new Vector3(tilePos.x - 1, tilePos.y, tilePos.z - 1);
        if (_islandChunks.ContainsKey(bottomLeftNeighbor))
            return bottomLeftNeighbor;
        
        return Vector3.zero;
    }
    
    private bool HasNeighbors(Transform tile)
    {
        Vector3 tilePos = tile.position;
        
        Vector3 leftNeighbor = new Vector3(tilePos.x - 1, tilePos.y, tilePos.z);
        Vector3 topLeftNeighbor = new Vector3(tilePos.x - 1, tilePos.y, tilePos.z + 1);
        Vector3 rightNeighbor = new Vector3(tilePos.x + 1, tilePos.y, tilePos.z);
        Vector3 topRightNeighbor = new Vector3(tilePos.x + 1, tilePos.y, tilePos.z + 1);
        Vector3 topNeighbor = new Vector3(tilePos.x, tilePos.y, tilePos.z + 1);
        Vector3 bottomRightNeighbor = new Vector3(tilePos.x + 1, tilePos.y, tilePos.z - 1);
        Vector3 bottomNeighbor = new Vector3(tilePos.x, tilePos.y, tilePos.z - 1);
        Vector3 bottomLeftNeighbor = new Vector3(tilePos.x - 1, tilePos.y, tilePos.z - 1);

        return _islandChunks.ContainsKey(leftNeighbor) || 
                _islandChunks.ContainsKey(rightNeighbor) || 
                _islandChunks.ContainsKey(topNeighbor) || 
                _islandChunks.ContainsKey(bottomNeighbor) ||
                _islandChunks.ContainsKey(topLeftNeighbor) ||
                _islandChunks.ContainsKey(topRightNeighbor) ||
                _islandChunks.ContainsKey(bottomRightNeighbor) ||
                _islandChunks.ContainsKey(bottomLeftNeighbor);
    }

    private void ProcessIslands()
    {
        //Loop through each island and randomize position + rotation!
        foreach (Transform island in _islands)
        {
            if (island.childCount < MinIslandSize)
            {
                Destroy(island.gameObject);
                continue;
            }

            Vector3 randomPos = new Vector3(island.position.x, island.position.y + Random.Range(-50, 50),
                island.position.z);
            island.position = randomPos;
        }
    }

    private float GetNoiseSample(int x, int y)
    {
        float xCoord = (float) x / Width * Scale + OffsetX;
        float yCoord = (float) y / Height * Scale + OffsetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);

        return sample;
    }
}
