using UnityEngine;

public class Slot : MonoBehaviour
{
    public int x;
    public int y;
    public int Index;
    public Chip chip;
    private string layerName = "Slot";

    private void Start()
    {
        SetLayerName();
    }

    public void Initialize(int posX, int posY,int iIndex)
    {
        x = posX;
        y = posY;
        Index= iIndex;
        chip = null;
    }

    private void SetLayerName()
    {
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }

    public bool HasChip()
    {
        return chip != null;
    }

    public void SetChip(Chip newChip)
    {
        chip = newChip;
        if (newChip != null)
        {
            newChip.SetSlot(this);
        }
    }

    public void ClearChip()
    {
        chip = null;
    }
}
