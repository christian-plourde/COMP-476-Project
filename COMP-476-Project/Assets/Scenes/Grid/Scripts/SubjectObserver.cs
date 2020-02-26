using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Subject : MonoBehaviour
{
    private LinkedList<Observer> observers;

    public Subject()
    {
        observers = new LinkedList<Observer>();
    }

    public void AttachObserver(Observer o)
    {
        observers.AddLast(o);
    }

    public void DetachObserver(Observer o)
    {
        observers.Remove(o);
    }

    public void Notify()
    {
        foreach(Observer o in observers)
        {
            o.ObserverUpdate();
        }
    }
}

public abstract class Observer : MonoBehaviour
{
    public Observer()
    {

    }

    public abstract void ObserverUpdate();
}
