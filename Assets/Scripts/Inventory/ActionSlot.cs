
using PurrNet;
using UnityEngine;
using UnityEngine.UI;

public class ActionSlot : MonoBehaviour
{
    [SerializeField] private Image slotImage;
    [SerializeField] private KeyCode actionkey = KeyCode.Alpha1;
    [SerializeField] private Color activeColor;

    private Color _originalColor;

    void Awake()
    {
        _originalColor = slotImage.color;
    }

    void Update()
    {
        if(!Input.GetKeyDown(actionkey))
            return;

        InstanceHandler.GetInstance<InventoryManager>().SetActionSlotActive(this);
        
    }

    public void ToggleActive (bool toggle)
    {
        slotImage.color = toggle ? activeColor : _originalColor;
    }
}
