using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryManager 
{
    public bool Full;
    public bool Expandable;

    protected GameObject[] Inventory;
    protected int[] InventoryStack;

    public InventoryManager()
	{
        Inventory = new GameObject[0];
		InventoryStack = new int[0];
    }

	public InventoryManager(int Size,bool Debug = false)
	{
        Inventory = new GameObject[Size];
        InventoryStack = new int[Size];
    }

	public int Length { 
		get { return (Inventory != null) ? Inventory.Length : 0; } 
	}

	public int AddItem(GameObject NewItem)
    {
        if (NewItem && !Full)
		{
			Item ItemComponent = null;	
			for (int index = 0; index < Inventory.Length; index++) 
			{
				if (Inventory [index] == null) 
				{
                    Inventory [index] = NewItem;
					InventoryStack [index] = 1;
                    Debug.Log("New Item Added at ["+index+"]");
                    return index;
				} 
				else
				{
					ItemComponent = Inventory [index].GetComponent<Item> ();
					if (ItemComponent.Equal(Inventory[index]))
					{
                        InventoryStack[index] += 1;
                        Debug.Log("Item Stacked at [" + index + "]");
                    }
				}
			}
            
		}

        if (NewItem && Expandable)
        {
            int OldLength = Length;
            Resize(Length + 1);
            Inventory[OldLength] = NewItem;
            return OldLength;
        }

        Debug.Log("Inventory is null");
        return -1;
	}

	public void AddItemAt(GameObject NewItem,int Index)
	{
        if (Full) { return; }

        if (NewItem == null || (Index < 0 && Index > Inventory.Length))
        {
            Debug.Log("Invalid Inventory Index");
            Debug.Log("Item: "+ NewItem);
        }

		if (Inventory [Index] == null)
		{
			Inventory [Index] = NewItem;
			InventoryStack [Index] = 1;
            Debug.Log("New Item Added To ["+Index+"]");
        } 
		else 
		{
			Item Temp = NewItem.GetComponent<Item> ();
			if (Temp.Equal (Inventory [Index])) 
			{
				InventoryStack [Index] += 1;
                Debug.Log("Item Stacked To [" + Index + "]");
            } 
			else 
			{
				Inventory [Index] = NewItem;
				InventoryStack [Index] = 1;
                Debug.Log("New Item Added To [" + Index + "]");
                Debug.Log("Old Item Lost");
            }
		}
	}

	public GameObject RemoveItem(GameObject Other)
	{
		if (Other == null)
        {
            Debug.Log("Invalid Item to Remove");
            return null;
        }
			

		Item Temp = Other.GetComponent<Item> ();
		for (int i = 0; i < Inventory.Length; i++)
		{
			if(Temp.Equal(Inventory[i]))
			{
				bool StackEmpty = (InventoryStack [i] <= 0);
				if (StackEmpty)
				{
					GameObject ItemTemp = Inventory [i];
					Inventory [i] = null;
                    Debug.Log("Item Removed from [" + i + "]");
                    return ItemTemp;
                } 
				else
				{
                    Debug.Log("Item Copied from [" + i + "]");
                    return Inventory[i];
				}
			}
		}

        Debug.Log("Item not found");
        return null;
	}

	public GameObject RemoveAt(int Index)
	{
		if (Index >= 0 && Index < Inventory.Length)
        {
			bool StackEmpty = InventoryStack [Index] >= 0;
			if (StackEmpty) {
				GameObject Temp = Inventory [Index];
				Inventory [Index] = null;
                Debug.Log("Item Removed from [" + Index + "]");
                return Temp;
			}
            else
            {
                Debug.Log("Item Copied from [" + Index + "]");
                return Inventory[Index];
			}
		}

        Debug.Log("Inventory Index Invalid");
        return null;
	}

	public GameObject GetItemAt(int Index)
	{
		if (Index >= 0 && Index < Inventory.Length) {
            Debug.Log("Item Get from [" + Index + "]");
            return Inventory [Index];
		}
        Debug.Log("Inventory Index Invalid");
        return null;
	}

	public int GetStackAt(int Index)
	{
		if (Index >= 0 && Index < InventoryStack.Length) {
            Debug.Log("Stack Get from [" + Index + "]");
            return InventoryStack [Index];
		}
        Debug.Log("Inventory Index Invalid");
        return -1;
	}

	public void Resize(int NewSize)
	{
		GameObject[] Temp = new GameObject[Inventory.Length];
		int[] TempStack = new int[InventoryStack.Length]; 

		bool Continue = (Temp.Length == TempStack.Length); 
		if (Continue)
		{
			for (int i = 0; i < Inventory.Length; i++) {
				Temp [i] = Inventory [i];
				TempStack [i] = InventoryStack [i];
			}

			Inventory = new GameObject[NewSize];
			InventoryStack = new int[NewSize];
            Debug.Log("Iventory Resize from ["+Temp.Length+" To "+NewSize+"]");

            for (int i = 0; i < Inventory.Length; i++)
            {
                if (i >= 0 && i < Temp.Length && i < Inventory.Length) {
                    Inventory[i] = Temp[i];
                    InventoryStack[i] = TempStack[i];
                }
				
			}

			Temp = new GameObject[0];
			TempStack = new int[0];
		}
	}

	public void Clear()
	{
		Inventory = new GameObject[Length];
		InventoryStack = new int[Length];
        Debug.Log("Ivnetory Cleared");
    }

	public void Empty()
	{
		Inventory = new GameObject[0];
		InventoryStack = new int[0];
        Debug.Log("Inventory Empty");
    }

	public bool Contains(GameObject Other)
	{
		Item Temp = Other.GetComponent<Item> ();
		for (int i = 0; i < Inventory.Length; i++) 
		{
			if (Temp.Equal (Inventory [i]))
				return true;
		}
		return false;
	}

    public bool Contains(int Id)
    {
        Item Temp;
        for (int i = 0; i < Inventory.Length; i++)
        {
            Temp = Inventory[i].GetComponent<Item>();
            if (Temp && Temp.Id == Id)
                return true;
        }

        return false;
    }

    public GameObject Find(int Id)
    {
        Item Temp;
        for (int i = 0; i < Inventory.Length; i++)
        {
            Temp = Inventory[i].GetComponent<Item>();
            if (Temp && Temp.Id == Id)
                return Inventory[i];
        }

        return null;
    }

    public GameObject Find(string Name)
    {
        Item Temp;
        for (int i = 0; i < Inventory.Length; i++)
        {
            Temp = Inventory[i].GetComponent<Item>();
            if (Temp && Temp.Name == Name)
                return Inventory[i];
        }

        return null;
    }

    public int FindIndex(int Id)
    {
        Item Temp;
        for (int i = 0; i < Inventory.Length; i++)
        {
            Temp = Inventory[i].GetComponent<Item>();
            if (Temp && Temp.Id == Id) return i;
        }

        return -1;
    }

    public int FindIndex(string Name)
    {
        Item Temp;
        for (int i = 0; i < Inventory.Length; i++)
        {
            Temp = Inventory[i].GetComponent<Item>();
            if (Temp && Temp.Name == Name) return i;
        }

        return -1;
    }

    public bool IsValidIndex(int Index) {
        return (Index >= 0 && Index < Inventory.Length);
    }
}
