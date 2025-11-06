using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VKSdk.Support;
using VKSdk.Notify;
using UnityEngine.Rendering.Universal;
public class FieldAssistant : MonoBehaviour
{
    public static FieldAssistant main;
    public Field field;
    public GameObject SlotFolder;
    public float offsetX = 1f;
    public float offsetY = 1f;
    public Player player;
    public Light2D light2D;
    
    private Dictionary<string, Slot> slots = new Dictionary<string, Slot>();
    private float originalLightRange;
    private float originalLightIntensity;
    private Color originalLightColor;
    private bool isProcessingSlotClick = false;

    private void Awake()
    {
        main = this;
    }

    private void Start()
    {
        if (light2D != null)
        {
            originalLightRange = light2D.pointLightOuterRadius;
            originalLightIntensity = light2D.intensity;
            originalLightColor = light2D.color;
        }
        
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
                slotObj.transform.SetParent(SlotFolder.transform, true);
                
                Slot slot = slotObj.GetComponent<Slot>();
                if (slot == null)
                {
                    slot = slotObj.AddComponent<Slot>();
                }
                
                slot.Initialize(col, row,row*field.height+col);
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
        if (isProcessingSlotClick) return;
        
        if (player != null && player.playerNPC.bodyPart != null )
        {
            isProcessingSlotClick = true;
            if (player.playerNPC.bodyPart.ID == clickedSlot.Index)
            {
                FlashSlotColor(clickedSlot, Color.green, 3f);
                
                Chip chip = ContentAssistant.Instance.GetItem<Chip>("Chip");
                
                Sprite avatarSprite = GameManager.Instance.allSprites[player.playerNPC.bodyPart.ID];
                chip.Initialize(clickedSlot, avatarSprite);
                
                clickedSlot.SetChip(chip);

                player.playerNPC.bodyPart.DestroyNow();
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
                FlashSlotColor(clickedSlot, Color.red, 3f);
                Debug.LogWarning("Lắp sai vật phẩm!");
                VKNotifyController.Instance.AddNotify("Lắp sai vật phẩm!", VKNotifyController.TypeNotify.Normal);
            }
        }
    }

    public void FlashSlotsYellow(float duration = 3f)
    {
        Color originalColor = Color.white;
        Color yellowColor = Color.yellow;

        foreach (var slot in slots.Values)
        {
            SpriteRenderer spriteRenderer = slot.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                // Nháy về màu vàng
                LeanTween.color(slot.gameObject, yellowColor, duration);
                
                // Sau duration giây, trả về màu trắng
                LeanTween.color(slot.gameObject, originalColor, 0.1f).setDelay(duration);
            }
        }
    }

    public void FlashSlotColor(Slot slot, Color flashColor, float duration = 3f)
    {
        if (slot == null) return;
        
        StartCoroutine(FlashSlotColorCoroutine(slot, flashColor, duration));

        // Thay đổi Light2D
        if (light2D != null)
        {
            StartCoroutine(FlashLight2DCoroutine(flashColor, duration));
        }
    }

    private IEnumerator FlashSlotColorCoroutine(Slot slot, Color flashColor, float duration)
    {
        SpriteRenderer spriteRenderer = slot.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) yield break;

        Color originalColor = Color.white;
        float flashSpeed = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // Nháy từ trắng sang màu chỉ định
            LeanTween.color(slot.gameObject, flashColor, flashSpeed * 0.5f);
            yield return new WaitForSeconds(flashSpeed * 0.5f);

            // Nháy từ màu chỉ định về trắng
            LeanTween.color(slot.gameObject, originalColor, flashSpeed * 0.5f);
            yield return new WaitForSeconds(flashSpeed * 0.5f);

            elapsedTime += flashSpeed;
        }

        // Trả về màu trắng cuối cùng
        spriteRenderer.color = originalColor;
        
        // Reset cờ để cho phép click tiếp theo
        isProcessingSlotClick = false;
    }

    private IEnumerator FlashLight2DCoroutine(Color flashColor, float duration)
    {
        float flashSpeed = 0.3f;
        float elapsedTime = 0f;
        float newRange = originalLightRange * 1.5f;
        float newIntensity = originalLightIntensity * 1.5f;

        while (elapsedTime < duration)
        {
            // Nháy sáng - tăng range và intensity
            LeanTween.color(light2D.gameObject, flashColor, flashSpeed * 0.5f);
            LeanTween.value(light2D.gameObject, originalLightRange, newRange, flashSpeed * 0.5f)
                .setOnUpdate((float val) => light2D.pointLightOuterRadius = val);
            LeanTween.value(light2D.gameObject, originalLightIntensity, newIntensity, flashSpeed * 0.5f)
                .setOnUpdate((float val) => light2D.intensity = val);
            yield return new WaitForSeconds(flashSpeed * 0.5f);

            // Nháy tối - giảm range và intensity
            LeanTween.color(light2D.gameObject, originalLightColor, flashSpeed * 0.5f);
            LeanTween.value(light2D.gameObject, newRange, originalLightRange, flashSpeed * 0.5f)
                .setOnUpdate((float val) => light2D.pointLightOuterRadius = val);
            LeanTween.value(light2D.gameObject, newIntensity, originalLightIntensity, flashSpeed * 0.5f)
                .setOnUpdate((float val) => light2D.intensity = val);
            yield return new WaitForSeconds(flashSpeed * 0.5f);

            elapsedTime += flashSpeed;
        }

        // Trả về trạng thái ban đầu
        light2D.color = originalLightColor;
        light2D.pointLightOuterRadius = originalLightRange;
        light2D.intensity = originalLightIntensity;
    }
}

public class Field
{
    public int width;
    public int height;
    public int[,] chips;
}
