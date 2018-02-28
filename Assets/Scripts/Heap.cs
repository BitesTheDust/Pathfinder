using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] aItems;
    int iCurrentItemCount;
    
	// Constructor...
    public Heap(int HeapSize)
    {
        // Don't want to resize the array, is expensive!
        aItems = new T[HeapSize];
    }

    public void Add(T item)
    {
        // Want item to know it's index so we create an interface:
        // See below interface definition
        // This will need to be implemented by the Node
        item.HeapIndex = iCurrentItemCount;
        aItems[iCurrentItemCount] = item;
        SortUp(item);
        iCurrentItemCount++;
    }

    void SortUp(T item)
    {
        int iParentIndex = (item.HeapIndex - 1) / 2;

        while(true)
        {
            T parentItem = aItems[iParentIndex];

            if(item.CompareTo(parentItem) > 0)
            {
                // IF > returns 1
                // IF == return 0
                // IF < return -1
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            iParentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    void Swap(T itemA, T itemB)
    {
        aItems[itemA.HeapIndex] = itemB;
        aItems[itemB.HeapIndex] = itemA;

        int iTemp = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = iTemp;
    }

    public T RemoveFirst()
    {
        T first = aItems[0];
        iCurrentItemCount--;
        aItems[0] = aItems[iCurrentItemCount];
        aItems[0].HeapIndex = 0;
        SortDown(aItems[0]);
        return first;
    }

    void SortDown(T item)
    {
        while(true)
        {
            int iLeft = (item.HeapIndex * 2) + 1;
            int iRight = (item.HeapIndex * 2) + 2;

            int iSwapIndex = 0;

            if(iLeft < iCurrentItemCount)
            {
                iSwapIndex = iLeft;

                if(iRight < iCurrentItemCount)
                {
                    if (aItems[iLeft].CompareTo(aItems[iRight]) < 0)
                    {
                        iSwapIndex = iRight;
                    }
                }

                if(item.CompareTo(aItems[iSwapIndex]) < 0)
                {
                    Swap(item, aItems[iSwapIndex]);
                } 
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    public void UpdateItem(T item)
    {
        // Only ever increase priority in our use case. 
        SortUp(item);
    }

    public int Count
    {
        get { return iCurrentItemCount; }
    }

    public bool Contains(T item)
    {
        return Equals(aItems[item.HeapIndex], item);
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }

}
