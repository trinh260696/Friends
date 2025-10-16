using UnityEngine;
using System.Collections.Generic;
using VKSdk.Support;
public class FieldAssistant : MonoBehaviour
{
    public static FieldAssistant main;
    public Field field;
    public GameObject SlotFolder;
    private Dictionary<string,Slot> slots = new Dictionary<string, Slot>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        main = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CreateField( Field field)
    {
        this.field = field;

    }
    void GenerateSlots()
    {

    }
}
public class Field
{
    public int width;
    public int height;
    public int[,] chips;
}
