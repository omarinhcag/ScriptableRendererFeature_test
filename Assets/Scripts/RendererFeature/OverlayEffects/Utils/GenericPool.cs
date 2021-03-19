using System.Collections.Generic;

public abstract class GenericPool<T>
{
    private Stack<T> objects = new Stack<T>();

    private List<T> createdObject = new List<T>();

    public abstract T CreateNew();
    public virtual void OnRelease(T obj)
    {

    }

    public T Get()
    {
        if (objects.Count == 0)
        {
            objects.Push(CreateNew());
            createdObject.Add(objects.Peek());
        }

        return objects.Pop();
    }

    public void ReleaseAll()
    {
        objects.Clear();
        foreach (var obj in createdObject)
        {
            objects.Push(obj);
            OnRelease(obj);
        }
            
    }
}