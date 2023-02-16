using Csg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSGObjectPool : MonoBehaviour
{
    public Stack<PolygonTreeNodeList> polygonTreeNodeLists = new Stack<PolygonTreeNodeList>();

    public Stack<PolygonTreeNodeList> recycledata = new Stack<PolygonTreeNodeList>();

    public Stack<PolygonTreeNode> NodeLists = new Stack<PolygonTreeNode>();

    public Stack<PolygonTreeNode> Noderecycledata = new Stack<PolygonTreeNode>();

 
    public static CSGObjectPool Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Init();
    }
    public void Init()
    {
        for (int i = 0; i < 1000000; i++)
        {
            polygonTreeNodeLists.Push(new PolygonTreeNodeList());
        }

        for (int i = 0; i < 200000; i++)
        {
            NodeLists.Push(new PolygonTreeNode());
        }
    }

    public PolygonTreeNodeList Pop()
    {
        var result = polygonTreeNodeLists.Pop();
        if (result != null)
        {
            recycledata.Push(result);
            return result;
        }
        polygonTreeNodeLists.Push(new PolygonTreeNodeList());
         return  polygonTreeNodeLists.Pop();
    }

    public void Recycle()
    {
        int count = recycledata.Count;
        for (int i = 0; i < count; i++)
        {
            var item=  recycledata.Pop();
            item.Clean();
            polygonTreeNodeLists.Push(item);
         }
     }





    public PolygonTreeNode PopNode()
    {
        if (NodeLists.Count == 0)
        {
            NodeLists.Push(new PolygonTreeNode());
            return NodeLists.Pop();
        }
        var result = NodeLists.Pop();
        Noderecycledata.Push(result);
        return result;
    }

    public void RecycleNode()
    {
        int count = Noderecycledata.Count;
        for (int i = 0; i < count; i++)
        {
            var item = Noderecycledata.Pop();
            item.Clean();
            NodeLists.Push(item);
        }
    }
}
