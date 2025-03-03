using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private int type;
    [SerializeField] private bool isEmpty;
    [SerializeField] private GameObject startingPrefab;
    public GameObject StartingPrefab => startingPrefab;
    public int Type => type;
    public bool IsEmpty => isEmpty;
}
