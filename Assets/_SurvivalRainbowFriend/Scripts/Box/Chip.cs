using UnityEngine;

public class Chip : MonoBehaviour
{
    public Slot slot;
    public SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    public void Initialize(Slot targetSlot, Sprite sprite)
    {
        slot = targetSlot;
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
        spriteRenderer.sprite = sprite;
        transform.position = slot.transform.position;
    }

    public void SetSlot(Slot newSlot)
    {
        slot = newSlot;
        if (slot != null)
        {
            transform.position = slot.transform.position;
        }
    }

    public Slot GetSlot()
    {
        return slot;
    }
}
