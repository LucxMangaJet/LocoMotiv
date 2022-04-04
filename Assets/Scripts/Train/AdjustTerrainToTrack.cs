using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustTerrainToTrack : MonoBehaviour
{
    [SerializeField] Terrain m_terrain;
    [SerializeField] TrackRoute m_track;
    [SerializeField] float m_fStepSize = 1;

    [Button]
    public void updateTerrainData()
    {
        var data = m_terrain.terrainData;
        Bounds bounds = new Bounds(transform.position + data.size * 0.5f, data.size);

        var texture = data.heightmapTexture;

        var heights = data.GetHeights(0, 0, data.heightmapResolution, data.heightmapResolution);

        for (int x = 0; x < data.heightmapResolution; x++)
        {
            for (int y = 0; y < data.heightmapResolution; y++)
                heights[y, x] = -1;
        }

        Queue<Vector2Int> collection = new Queue<Vector2Int>();

        float trackLength = m_track.Length;
        for (float distance = 0; distance < trackLength; distance += m_fStepSize)
        {
            var sample = m_track.SampleTrackpoint(distance);

            if (!bounds.Contains(sample.Position))
                continue;

            var point = WorldToDataPoint(data, sample.Position);

            heights[point.y, point.x] = WorldToTerrainHeightValue(data, sample.Position.y);

            collection.Enqueue(point);
        }

        while (collection.Count > 0)
        {
            var el = collection.Dequeue();

            var neighbours = getNeighbours(el);

            for (int i = 0; i < neighbours.Length; i++)
            {
                Vector2Int n = neighbours[i];
                if (IsInRange(data, n))
                {
                    if (heights[n.y, n.x] == -1)
                    {
                        heights[n.y, n.x] = heights[el.y, el.x];
                        collection.Enqueue(n);
                    }
                }
            }
        }

        data.SetHeights(0, 0, heights);
        m_terrain.Flush();
    }

    private bool IsInRange(TerrainData data, Vector2Int item)
    {
        if (item.x < 0 || item.y < 0 || item.x >= data.heightmapResolution || item.y >= data.heightmapResolution)
            return false;

        return true;
    }

    private Vector2Int[] getNeighbours(Vector2Int el)
    {
        return new Vector2Int[]
        {
            el + new Vector2Int(0,1),
            el + new Vector2Int(1,0),
            el + new Vector2Int(0,-1),
            el + new Vector2Int(-1,0)
        };
    }

    Vector2Int WorldToDataPoint(TerrainData _data, Vector3 _position)
    {
        _position -= transform.position;
        var x = _position.x / _data.size.x;
        var z = _position.z / _data.size.z;

        var ix = Mathf.FloorToInt(x * _data.heightmapResolution);
        var iz = Mathf.FloorToInt(z * _data.heightmapResolution);

        return new Vector2Int(ix, iz);
    }


    float WorldToTerrainHeightValue(TerrainData _data, float _y)
    {
        _y -= transform.position.y;

        _y = _y / _data.size.y;

        return _y;
    }
}
