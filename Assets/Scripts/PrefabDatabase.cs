using UnityEngine;

[CreateAssetMenu(fileName = "PrefabDB", menuName = "ScriptableObject/PrefabDB", order = 1)]
public class PrefabDatabase : ScriptableObject
{
    public GameObject[] prefabList;
}
