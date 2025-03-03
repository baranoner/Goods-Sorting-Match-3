using System.Collections.Generic;
using UnityEngine;

public class Shelf : MonoBehaviour
{
    [SerializeField] private List<Transform> shelfSections;
    [SerializeField] private GameObject disabledItemsSection;
    [SerializeField] private GameObject emptyItemPrefab;
    private Vector3 _itemScales;

    private void Start()
    {
        InitializeShelf();
    }

    private void InitializeShelf()
    {
        foreach (Transform section in shelfSections)
        {

            Item[] items = section.GetComponentsInChildren<Item>(true);

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].StartingPrefab != null)
                {
                    Vector3 position = items[i].transform.position;
                    int sortingLayer = items[i].GetComponent<SpriteRenderer>().sortingLayerID;
                    bool isEnabled = items[i].enabled;
                    _itemScales = items[i].transform.localScale;

                    items[i].gameObject.transform.SetParent(disabledItemsSection.transform);
                    items[i].gameObject.SetActive(false);
                    GameObject newItem = Instantiate(items[i].StartingPrefab, position, Quaternion.identity);
                    newItem.transform.SetParent(section);
                    newItem.transform.localScale = _itemScales;
                    newItem.GetComponent<SpriteRenderer>().sortingLayerID = sortingLayer;
                    newItem.SetActive(isEnabled);

                    if (newItem.transform.parent.CompareTag("Back"))
                    {
                        newItem.GetComponent<CapsuleCollider2D>().enabled = false;
                    }
                    else if (newItem.transform.parent.CompareTag("OtherSections"))
                    {
                        newItem.GetComponent<CapsuleCollider2D>().enabled = false;
                        newItem.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }
        }
    }


    public void CheckMatch()
    {
        Item[] frontItems = shelfSections[0].GetComponentsInChildren<Item>();
        int typeCount = 1;
        Item firstItem = frontItems[0];

        if (firstItem.IsEmpty) return;

        for (int i = 1; i < frontItems.Length; i++)
        {
            if (frontItems[i].Type == firstItem.Type)
            {
                typeCount++;
            }
        }

        if (typeCount == frontItems.Length)
        {
            DestroyMatch();
            GameEvents.OnScoreIncrease?.Invoke();
            GameEvents.OnComboIncrease?.Invoke();
        }
    }

    private void DestroyMatch()
    {
        Transform front = shelfSections[0];
        Item[] frontItems = front.GetComponentsInChildren<Item>();

        // float moveDuration = 0.2f;

        for (int i = 0; i < frontItems.Length; i++)
        {
            GameObject newEmptyItem = Instantiate(emptyItemPrefab, frontItems[i].transform.position, Quaternion.identity);
            newEmptyItem.transform.SetParent(front);
            newEmptyItem.transform.localScale = _itemScales;
            
            frontItems[i].gameObject.transform.SetParent(disabledItemsSection.transform);
            frontItems[i].gameObject.SetActive(false);
        }

        ReplaceItems();
    }

    public void ReplaceItems()
    {
        for (int i = 1; i < shelfSections.Count; i++)
        {
            Transform previousSection = shelfSections[i - 1];
            Transform currentSection = shelfSections[i];

            Item[] previousItems = previousSection.GetComponentsInChildren<Item>();
            Item[] currentItems = currentSection.GetComponentsInChildren<Item>();

            int itemCount = Mathf.Min(previousItems.Length, currentItems.Length);

            Vector3[] previousPositions = new Vector3[itemCount];
            Vector3[] currentPositions = new Vector3[itemCount];

            for (int j = 0; j < itemCount; j++)
            {
                previousPositions[j] = previousItems[j].transform.position;
                currentPositions[j] = currentItems[j].transform.position;
            }

            for (int j = 0; j < itemCount; j++)
            {
                previousItems[j].transform.SetParent(currentSection);
                // previousItems[j].transform.DOMove(currentPositions[j], moveDuration).SetEase(Ease.OutQuad);
                previousItems[j].transform.position = currentPositions[j];
                
                AdjustComponents(previousItems[j]);
            }
            
            // yield return new WaitForSeconds(moveDuration);
            
            for (int j = 0; j < itemCount; j++)
            {
                currentItems[j].transform.SetParent(previousSection);
                // currentItems[j].transform.DOMove(previousPositions[j], moveDuration).SetEase(Ease.OutQuad);
                currentItems[j].transform.position = previousPositions[j];

                AdjustComponents(currentItems[j]);
            }
        }
    }

    private void AdjustComponents(Item currentItem)
    {
        SpriteRenderer spriteRenderer = currentItem.GetComponent<SpriteRenderer>();
        CapsuleCollider2D capsuleCollider = currentItem.GetComponent<CapsuleCollider2D>();
        
        if (currentItem.transform.parent.CompareTag("Front"))
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sortingLayerName = "FrontItem";
            capsuleCollider.enabled = true;
        }
        else if (currentItem.transform.parent.CompareTag("Back"))
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sortingLayerName = "BackItem";
            capsuleCollider.enabled = false;
        }
        else if (currentItem.transform.parent.CompareTag("OtherSections"))
        {
            spriteRenderer.enabled = false;
            capsuleCollider.enabled = false;
        }
    }
}

