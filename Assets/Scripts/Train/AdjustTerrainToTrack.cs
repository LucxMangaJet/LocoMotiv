using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

public class AdjustTerrainToTrack : MonoBehaviour
{
    [SerializeField] Terrain m_terrain;
    [SerializeField] TrackRoute m_track;
    [SerializeField] float m_fStepSize = 1;

    [SerializeField]
    bool running = false;

    [Button]
    public void updateTerrainData()
    {
        if (running)
            return;

        EditorCoroutineUtility.StartCoroutine(buildTerrain(), this);
    }

    private IEnumerator buildTerrain()
    {
        running = true;
        var data = m_terrain.terrainData;

        var heights = data.GetHeights(0, 0, data.heightmapResolution, data.heightmapResolution);

        for (int x = 0; x < data.heightmapResolution; x++)
        {
            for (int y = 0; y < data.heightmapResolution; y++)
                heights[y, x] = 0;
        }

        Vector3[] m_arTrackpoints = new Vector3[Mathf.RoundToInt(m_track.Length / m_fStepSize)];
        float[] weights = new float[m_arTrackpoints.Length];

        for (int i = 0; i < m_arTrackpoints.Length; i++)
        {
            float distance = m_fStepSize * i;
            m_arTrackpoints[i] = m_track.SampleTrackpoint(distance).Position;
        }

        for (int x = 0; x < data.heightmapResolution; x++)
        {
            for (int y = 0; y < data.heightmapResolution; y++)
            {
                var worldPos = DataPointToWorld(data, x, y);

                float totalWeight = 0;
                for (int i = 0; i < m_arTrackpoints.Length; i++)
                {

                    Vector3 tp = m_arTrackpoints[i];
                    tp.y = 0;
                    float distance = (worldPos - tp).sqrMagnitude * 2;
                    weights[i] = 1 / distance;
                    totalWeight += weights[i];
                }

                float height = 0;

                for (int i = 0; i < m_arTrackpoints.Length; i++)
                {
                    Vector3 tp = m_arTrackpoints[i];
                    height += tp.y * weights[i] / totalWeight;
                }
                heights[y, x] = height / data.size.y;
            }

            data.SetHeights(0, 0, heights);
            yield return null;
        }

        m_terrain.Flush();

        running = false;
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

    Vector3 DataPointToWorld(TerrainData _data, int x, int y)
    {
        float fx = (float)x * _data.size.x / _data.heightmapResolution;
        float fy = (float)y * _data.size.x / _data.heightmapResolution;

        return new Vector3(fx, 0, fy) + transform.position;
    }

    float WorldToTerrainHeightValue(TerrainData _data, float _y)
    {
        _y -= transform.position.y;

        _y = _y / _data.size.y;

        return _y;
    }
}
