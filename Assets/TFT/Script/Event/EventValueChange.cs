using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventValueChange
{
    private float value;

    public delegate void NumManipulationHandler();

    public event NumManipulationHandler valueChange;

    protected virtual void OnNumChanged()
    {
        valueChange?.Invoke(); /* 事件被触发 */
    }

}
