using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> shelves;
    [SerializeField] private float movingDuration = 0.2f;
    [SerializeField] private LevelManager levelManager;
    private bool _isDragging;
    private Transform _selectedObject;
    private Vector3 _offset;
    private Tween _moveTween;
    private Vector3 _selectedObjectStartingPosition;
    private Vector3 _touchWorldPosition;

    private void Update()
    {
        HandleTouch();
    }

    private void HandleTouch()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            _touchWorldPosition = Camera.main!.ScreenToWorldPoint(
                new Vector3(touch.position.x, touch.position.y, Mathf.Abs(Camera.main.transform.position.z))
            );

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touch.position), Vector2.zero);
                    if (hit.collider != null && hit.collider.CompareTag("Item") && !hit.collider.GetComponent<Item>().IsEmpty)
                    {
                        _selectedObjectStartingPosition = hit.collider.transform.position;
                        _selectedObject = hit.transform;
                        _selectedObject.GetComponent<SpriteRenderer>().sortingOrder++;
                        _offset = _selectedObject.position - _touchWorldPosition;
                        _isDragging = true;
                    }
                    break;

                case TouchPhase.Moved:
                    if (_isDragging && _selectedObject != null)
                    {
                        Vector3 targetPosition = _touchWorldPosition + _offset;
                        _moveTween?.Kill();
                        _moveTween = _selectedObject.DOMove(targetPosition, 0.05f).SetEase(Ease.OutQuad);
                    }
                    break;

                case TouchPhase.Ended:
                    if (_selectedObject != null)
                    {
                        DropToNearestEmpty();
                        _isDragging = false;
                        _selectedObject.GetComponent<SpriteRenderer>().sortingOrder--;
                        _selectedObject = null;
                    }
                    break;
            }
        }
    }

    private void DropToNearestEmpty()
    {
        GameObject nearestShelf = shelves
            .OrderBy(shelf => Vector3.Distance(shelf.transform.position, _touchWorldPosition))
            .FirstOrDefault(shelf => shelf.GetComponentsInChildren<Item>().Any(item => item.IsEmpty));

        if (nearestShelf == null)
        {
            _selectedObject.DOMove(_selectedObjectStartingPosition, movingDuration).SetEase(Ease.OutQuad);
            return;
        }

        Item nearestEmptyItem = nearestShelf
            .GetComponentsInChildren<Item>()
            .Where(item => item.IsEmpty && item.transform.parent.CompareTag("Front"))
            .OrderBy(item => Vector3.Distance(item.transform.position, _touchWorldPosition))
            .FirstOrDefault();

        if (nearestEmptyItem == null || Vector3.Distance(_selectedObject.transform.position, _selectedObjectStartingPosition) < 0.1f)
        {
            _selectedObject.DOMove(_selectedObjectStartingPosition, movingDuration).SetEase(Ease.OutQuad);
            return;
        }

        Vector3 emptyItemPosition = nearestEmptyItem.transform.position;
        
        Transform selectedObjectParent = _selectedObject.parent;
        Transform emptyItemParent = nearestEmptyItem.transform.parent;
        
        Sequence dropSequence = DOTween.Sequence();
        dropSequence.Append(_selectedObject.DOMove(emptyItemPosition, movingDuration).SetEase(Ease.OutQuad));
        dropSequence.Append(_selectedObject.DOPunchScale(Vector3.one * 0.2f, 0.3f, 10, 0.3f));
        _selectedObject.SetParent(emptyItemParent);

        nearestEmptyItem.transform.position = _selectedObjectStartingPosition;
        nearestEmptyItem.transform.SetParent(selectedObjectParent);

        dropSequence.OnComplete(() =>
        {
            nearestShelf.GetComponent<Shelf>().CheckMatch();

            ReplaceEmptySelectedShelf(selectedObjectParent);
            
            CheckWinCondition();
        });
        

    }

    private void CheckWinCondition()
    {
        bool isThereAnyItem = false;

        foreach (GameObject shelf in shelves)
        {
            Item[] items = shelf.GetComponentsInChildren<Item>();

            foreach (Item item in items) 
            {
                if (!item.IsEmpty)
                {
                    isThereAnyItem = true;
                    break;
                }
            }

            if (isThereAnyItem) break;
        }

        if (!isThereAnyItem)
        {
            levelManager.Win();   
        }
    }


    private void ReplaceEmptySelectedShelf(Transform selectedObjectParent)
    {
        Item[] selectedObjectSectionItems = selectedObjectParent.GetComponentsInChildren<Item>();
        bool isShelfFilled = false;
        
        foreach (Item shelfItem in selectedObjectSectionItems)
        {
            if (!shelfItem.IsEmpty)
            {
                isShelfFilled = true;
                break;
            }
        }
        
        if (!isShelfFilled)
        {
            Transform selectedObjectShelf = selectedObjectParent.parent;
            selectedObjectShelf.GetComponent<Shelf>().ReplaceItems();
        }
    }
}



