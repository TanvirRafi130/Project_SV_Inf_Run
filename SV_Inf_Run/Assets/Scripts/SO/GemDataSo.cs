using UnityEngine;

[CreateAssetMenu(fileName = "GemDataSo", menuName = "Scriptable Objects/GemDataSo")]
public class GemDataSo : ScriptableObject
{
    [System.Serializable]
    public struct GemDataSoEntry
    {
        public string gemName;
        public int gemPointValue;
        public GameObject gemPrefab;
        public int spawnAmountInPool;
    }

    public GemDataSoEntry[] gemEntries;

}
