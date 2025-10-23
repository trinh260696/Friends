using UnityEngine;
using System.Collections.Generic;
using VKSdk.Support;
using VKSdk.Notify;
public class FieldAssistant : MonoBehaviour
{
    public static FieldAssistant main;
    public Field field;
    public GameObject SlotFolder;
    public float offsetX = 1f;
    public float offsetY = 1f;
    public Player player;
    
    private Dictionary<string, Slot> slots = new Dictionary<string, Slot>();

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        if (field != null)
        {
            GenerateSlots();
        }
    }

    public void CreateField(Field field)
    {
        this.field = field;
        GenerateSlots();
    }

    private void GenerateSlots()
    {
        if (field == null || SlotFolder == null) return;

        slots.Clear();
        
        Vector3 slotFolderPos = SlotFolder.transform.position;
        float startX = slotFolderPos.x - (field.width - 1) * offsetX * 0.5f;
        float startY = slotFolderPos.y + (field.height - 1) * offsetY * 0.5f;

        for (int row = 0; row < field.height; row++)
        {
            for (int col = 0; col < field.width; col++)
            {
                Vector3 slotPosition = new Vector3(
                    startX + col * offsetX,
                    startY - row * offsetY,
                    0
                );

                GameObject slotObj = ContentAssistant.Instance.GetItem("Slot", slotPosition);
                slotObj.transform.SetParent(SlotFolder.transform, false);
                
                Slot slot = slotObj.GetComponent<Slot>();
                if (slot == null)
                {
                    slot = slotObj.AddComponent<Slot>();
                }
                
                slot.Initialize(col, row);
                string key = $"{row}_{col}";
                slots[key] = slot;
            }
        }
    }

    public Slot GetSlot(int x, int y)
    {
        string key = $"{y}_{x}";
        if (slots.ContainsKey(key))
        {
            return slots[key];
        }
        return null;
    }

    public Dictionary<string, Slot> GetAllSlots()
    {
        return slots;
    }

    public bool AreAllSlotsOccupied()
    {
        foreach (var slot in slots.Values)
        {
            if (!slot.HasChip())
            {
                return false;
            }
        }
        return true;
    }

    public void PlaceChip(int x, int y, Chip chip, int expectedID)
    {
        Slot slot = GetSlot(x, y);
        if (slot != null)
        {
            if (slot.x == chip.GetSlot().x && slot.y == chip.GetSlot().y)
            {
                slot.SetChip(chip);
                NotificationCenter.DefaultCenter().PostNotification(this, "ChipPlaced", chip);
            }
            else
            {
                VKNotifyController.Instance.AddNotify( "Lắp sai vật phẩm!",VKNotifyController.TypeNotify.Normal );
            }
        }
    }

    public void HandleSlotClick(Slot clickedSlot)
    {       
        if (clickedSlot == null) return;
        if(player.playerNPC.State!= StateFriend.FRIEND_SORTING_FOOD) return;
        if (player != null && player.playerNPC.bodyPart != null && player.playerNPC.State == StateFriend.FRIEND_GO_MAIN)
        {
            if (player.playerNPC.bodyPart.ID == clickedSlot.x)
            {
                Chip chip = ContentAssistant.Instance.GetItem<Chip>("Chip");
                
                Sprite avatarSprite = Resources.Load<Sprite>($"Avatar/Avatar_{player.playerNPC.bodyPart.ID}");
                chip.Initialize(clickedSlot, avatarSprite);
                
                clickedSlot.SetChip(chip);
                
                Destroy(player.playerNPC.bodyPart.gameObject);
                player.playerNPC.bodyPart = null;
                
                NotificationCenter.DefaultCenter().PostNotification(this, "ChipPlaced");
                GameManager.Instance.OnSortComplete();
                if (AreAllSlotsOccupied())
                {
                    GameManager.Instance.OnVictory();
                }
            }
            else
            {
                VKNotifyController.Instance.AddNotify("Lắp sai vật phẩm!", VKNotifyController.TypeNotify.Normal);
            }
        }
    }
}

public class Field
{
    public int width;
    public int height;
    public int[,] chips;
}
