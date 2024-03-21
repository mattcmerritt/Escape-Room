using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillBoxSlot : MonoBehaviour
{
    [SerializeField] private bool Opened;
    [SerializeField] private float MaxAngleZ = -110f;
    [SerializeField] private float OpenTimeDuration = 1f;
    [SerializeField] private float TimeSpentOpening = 0f;
    
    public void Start()
    {
        if(PillBox.SlotTable != null && PillBox.SlotTable.ContainsKey(gameObject.name)) 
        {
            Opened = PillBox.SlotTable[gameObject.name];
        }
        else
        {
            // Debug.LogWarning("Slot table could not be found by slot!");
            Opened = false;
        }
        
        if(!Opened)
        {
            OpenTimeDuration = 1f;
            TimeSpentOpening = 0f; 
        }
        else
        {
            OpenTimeDuration = 1f;
            TimeSpentOpening = 1f;
        }
    }

    public void Update()
    {
        // check if opened
        if(PillBox.SlotTable != null && PillBox.SlotTable.ContainsKey(gameObject.name)) 
        {
            Opened = PillBox.SlotTable[gameObject.name];
        }

        if(Opened && TimeSpentOpening <= OpenTimeDuration)
        {
            TimeSpentOpening += Time.deltaTime;
        }
    }

    public void FixedUpdate()
    {
        if(Opened)
        {
            transform.eulerAngles = Vector3.Lerp(new Vector3(0, -90f, 0), new Vector3(0, -90f, MaxAngleZ), Mathf.Min(1f, TimeSpentOpening / OpenTimeDuration));
        }
    }
}
