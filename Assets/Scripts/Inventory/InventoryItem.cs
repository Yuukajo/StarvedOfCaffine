using PurrNet;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TMP_Text amoutText;
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Transform _originalParent;
    private Canvas _canvas;
    private Image _itemImage;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _itemImage = GetComponent<Image>();
        _canvas = GetComponentInParent<Canvas>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        _originalParent = transform.parent;
        _canvasGroup.blocksRaycasts = false;
        _rectTransform.SetParent(_canvas.transform);
        _rectTransform.SetAsLastSibling();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    { 
        if (eventData.pointerEnter == null || !eventData.pointerEnter.TryGetComponent(out InventorySlot inventorySlot))
        {
            transform.SetParent(_originalParent);
            SetAvailable();
        }
    }

    public void SetAvailable()
    {
        _canvasGroup.blocksRaycasts = true;
        _rectTransform.anchoredPosition = Vector2.zero;
    }

    public void Init(string itemName, Sprite _itemPicture, int amout)
    {
        _itemImage.sprite = _itemPicture;
        amoutText.text = amout.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Right) return;

        if (!InstanceHandler.TryGetInstance(out InventoryManager InventoryManager))
        {
            Debug.LogError($"Failed to get inventory manager to drop item!");
            return;
        }
        InventoryManager.DropItem(this);
    }
}
