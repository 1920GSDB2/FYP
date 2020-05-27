using System;
using TFT;
using UnityEngine;

public interface ISelectable
{
    void PutDown();
    void DragUp();
    Transform transform { get; }
} 
