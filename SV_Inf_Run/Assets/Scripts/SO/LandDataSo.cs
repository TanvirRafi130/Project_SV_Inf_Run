using UnityEngine;

[CreateAssetMenu(fileName = "LandDataSo", menuName = "Scriptable Objects/LandDataSo")]
public class LandDataSo : ScriptableObject
{
    [System.Serializable]
    public struct LandData
    {
        public GameObject landPrefab;
        [Range(1,10)]public int maxSpawnCount;
        
    }

    public LandData[] landDataArray;
}
