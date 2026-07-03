using UnityEngine;
using UnityEngine.Serialization;

public class ScreenDetector : MonoBehaviour
{
    private Camera _camera;
    private int _laneCount = 4;
    [SerializeField] private Transform[] _lineTransforms;
    [SerializeField] private Transform[] _spawnPoints;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _camera = Camera.main;
        float cameraHeight = _camera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * _camera.aspect;
        float offset = cameraWidth * 0.5f;
        float sizePerRow = cameraWidth / _laneCount;
        SafeLineArea(sizePerRow, offset);
        SafeSpawnPointArea(sizePerRow, offset);
    }

    private void SafeLineArea(float cameraSize, float offset)
    {

        for (int i = 0; i < _lineTransforms.Length; i++)
        {
            Transform spawnPoint = _lineTransforms[i];
            Vector3 position = spawnPoint.position;
            position.x = i * cameraSize - offset;
            spawnPoint.position = position;
        }
    }

    private void SafeSpawnPointArea(float cameraSize, float offset)
    {
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            Transform spawnPoint = _spawnPoints[i];
            Vector3 position = spawnPoint.position;
            position.x = i * cameraSize - offset + (offset * 0.25f);
            spawnPoint.position = position;
        }
    }
    
}
