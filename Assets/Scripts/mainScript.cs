using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainScript : MonoBehaviour
{
    Color color_def = new Color(0.4f, 0.8f, 0.3f);
    Color color_start = new Color(0.8f, 0.3f, 0.4f);
    Color color_end = new Color(0.3f, 0.4f, 0.8f);
    Color color_play = new Color(0.4f, 0.3f, 0.8f);
    public Transform Base;
    /// <summary>
    ///  Vị trí đầu tiên
    /// </summary>
    public Vector2 firstPosition;
    /// <summary>
    /// Kích thước một ô
    /// </summary>
    public Vector2 size;
    /// <summary>
    /// kích thước bàn cờ (cố định)
    /// </summary>
    const int max_w = 8, max_h = 8;
    /// <summary>
    /// hinh anh quan co
    /// </summary>
    public GameObject icon, icon_start, icon_end;

    private Camera cam_main;

    internal void onGo(int x_start, int y_start, int x_end, int y_end)
    {

    }

    private List<singleBox> listBox;
    private List<singleBox> listStep;
    private singleBox[] point;
    public List<List<Node>> listnode;
    bool isfind = false;

    [Obsolete]
    void Start()
    {
        Init();
        DrawMap();
    }

    private void Init()
    {
        cam_main = Camera.main;
        listBox = new List<singleBox>();
        point = new singleBox[2];
        listStep = new List<singleBox>();
    }

    [Obsolete]
    private void DrawMap()
    {
        ClearChild(Base);
        for (int i = 0; i < max_w; i++)
        {
            for (int j = max_h; j > 0; j--)
            {
                Vector2 start = new Vector2(firstPosition.x + size.x * i, firstPosition.y - size.y * (max_h - j));
                Vector2 end = new Vector2(firstPosition.x + size.x * i + size.x, firstPosition.y - size.y * (max_h - j) - size.y);
                var box = new singleBox(start, end);
                listBox.Add(box);
                DrawRectangle(box, color_def);
            }
        }
    }

    private void ClearChild(Transform @base)
    {
        if (@base.childCount == 0) return;
        while (@base.childCount > 0)
        {
            Transform child = @base.GetChild(0);
            child.parent = null;
            Destroy(child.gameObject);
        }
    }

    [Obsolete]
    private void DrawRectangle(singleBox e, Color color, float width = 0.03f)
    {
        e.ob1 = DrawLine(Base, new Vector3(e.start.x, e.start.y), new Vector3(e.start.x, e.end.y), color, width);
        e.ob2 = DrawLine(Base, new Vector3(e.start.x, e.end.y), new Vector3(e.end.x, e.end.y), color, width);
        e.ob3 = DrawLine(Base, new Vector3(e.start.x, e.start.y), new Vector3(e.end.x, e.start.y), color, width);
        e.ob4 = DrawLine(Base, new Vector3(e.end.x, e.start.y), new Vector3(e.end.x, e.end.y), color, width);
    }

    // Update is called once per frame
    [System.Obsolete]
    void Update()
    {
        ClickMap();
    }

    [Obsolete]
    private void ClickMap()
    {
        if (Input.GetMouseButtonDown(0) && !isfind)
        {
            var position = cam_main.ScreenToWorldPoint(Input.mousePosition);
            var box = GetRectangle(position);
            if (box == null) return;
            ChooseBox(box);
        }
        return;
    }

    private singleBox GetRectangle(Vector3 position)
    {
        foreach (var e in listBox)
        {
            if (position.x < e.end.x && position.x >= e.start.x && position.y <= e.start.y && position.y > e.end.y)
            {
                return e;
            }
        }
        return null;
    }

    [Obsolete]
    private void ChooseBox(singleBox box)
    {
        if (point[0] == null || point[0] != null && point[1] != null || point[0].Equals(point[1]))
        {
            point[0] = null;
            point[1] = null;
            ChooseStartBox(box);
        }
        else
        {
            ChooseEndBox(box);
        }
    }

    [Obsolete]
    private void ChooseStartBox(singleBox box)
    {
        DrawMap();
        point[0] = new singleBox(box);
        Debug.Log("start point: " + point[0].start);
        fillIcon(box, icon_start);
    }

    [Obsolete]
    private void ChooseEndBox(singleBox box)
    {
        point[1] = new singleBox(box);
        Debug.Log("end point: " + point[1].start);
        fillIcon(box, icon_end);
        var x = StartCoroutine(FindWays());
        isfind = true;
    }

    [Obsolete]
    private IEnumerator FindWays()
    {
        var wait = new WaitForFixedUpdate();
        listnode = new List<List<Node>>();
        listnode.Add(new List<Node>() { new Node() { Prev = null, data = point[0], index = 0 } });
        int index = 1;
        Node result = null;
        while (result == null)
        {
            List<Node> c_list = new List<Node>();
            foreach (var e in listnode[listnode.Count - 1])
            {
                var list_point = GetListBox(e.data.start, point[1].start);
                int value = list_point.Count;// > 2 ? 2 : list_point.Count;
                for (int i = 0; i < value; i++)
                {
                    Node data = new Node() { data = list_point[i], Prev = e, index = index };
                    c_list.Add(data);
                }
            }
            listnode.Add(c_list);
            RemakeList(listnode[listnode.Count - 1]);
            result = CheckEnd(listnode[listnode.Count - 1]);
            yield return wait;
        }
        List<Node> stack = new List<Node>();
        stack.Add(result);
        var comm = result;
        for (int i = listnode.Count - 2; i >= 0; i--)
        {
            var list = listnode[i];
            var e = list.Find(x => x.data == comm.Prev.data);
            stack.Add(e);
            comm = e;
        }
        for (int i = stack.Count - 1; i >= 0; i--)
        {
            listStep.Add(stack[i].data);
        }
        StartCoroutine(PlayCounter());
    }

    [Obsolete]
    private IEnumerator PlayCounter()
    {

        foreach (var e in listStep)
        {
            fillIcon(e, icon);
            yield return new WaitForSeconds(0.5f);
        }
        isfind = false;
        listStep.Clear();
    }

    private void fillIcon(singleBox e, GameObject icon)
    {
        Instantiate(icon, Base).transform.position = new Vector3(e.start.x + size.x / 2, e.start.y - size.y / 2);
    }

    private List<singleBox> GetListBox(Vector3 @s, Vector3 @e)
    {
        List<singleBox> result = new List<singleBox>();
        const int a = 1, b = 2;
        var data = AddPoint(@s, @e, a, b);
        if (data != null) result.Add(data);
        var data2 = AddPoint(@s, @e, -a, b);
        if (data2 != null) result.Add(data2);
        var data3 = AddPoint(@s, @e, a, -b);
        if (data3 != null) result.Add(data3);
        var data4 = AddPoint(@s, @e, -a, -b);
        if (data4 != null) result.Add(data4);
        var data5 = AddPoint(@s, @e, b, a);
        if (data5 != null) result.Add(data5);
        var data6 = AddPoint(@s, @e, -b, a);
        if (data6 != null) result.Add(data6);
        var data7 = AddPoint(@s, @e, b, -a);
        if (data7 != null) result.Add(data7);
        var data8 = AddPoint(@s, @e, -b, -a);
        if (data8 != null) result.Add(data8);

        Sort(result, @e);
        return result;
    }

    private singleBox AddPoint(Vector3 s, Vector3 e, int a, int b)
    {
        var c1 = new Vector3(@s.x + a, @s.y + b);
        if (c1 != null && checkCondition(c1, @e))
            return GetSingleBox(c1);
        return null;
    }

    private bool checkCondition(Vector3 s, Vector3 e)
    {
        if (s.x < firstPosition.x || s.x > firstPosition.x + size.x * (max_w - 1) || s.y > firstPosition.y || s.y < firstPosition.y - size.y * (max_h - 1))
            return false;
        if (listStep.Find(x => x.start.Equals(s)) != null || s.Equals(point[0]))
        {
            return false;
        }
        return true;
    }

    private void Sort(List<singleBox> result, Vector3 e)
    {
        for (int i = 0; i < result.Count - 1; i++)
        {
            for (int j = i + 1; j < result.Count; j++)
            {
                var c1 = Vector3.Distance(result[i].start, e);
                var c2 = Vector3.Distance(result[j].start, e);
                if (c1 > c2)
                {
                    var cache = result[i];
                    result[i] = result[j];
                    result[j] = cache;
                }
            }
        }
    }

    private singleBox GetSingleBox(Vector3 start)
    {
        foreach (var e in listBox)
        {
            if (e.start.Equals(start))
            {
                return e;
            }
        }
        return null;
    }

    private void RemoveRect(singleBox box)
    {
        Destroy(box.ob1);
        Destroy(box.ob2);
        Destroy(box.ob3);
        Destroy(box.ob4);
    }

    [System.Obsolete]
    GameObject DrawLine(Transform parent, Vector3 start, Vector3 end, Color color, float width)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.transform.parent = parent;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.SetColors(color, color);
        lr.SetWidth(width, width);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        return myLine;


    }

    private void RemakeList(List<Node> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            for (int j = i + 1; j < list.Count; j++)
            {
                if (list[i].data.Equals(list[j].data))
                {
                    list.RemoveAt(j);
                }
            }
        }
    }

    private Node CheckEnd(List<Node> list)
    {
        return list.Find(e => e.data.start.Equals(point[1].start));
    }
}

[System.Serializable]
public class singleBox
{
    public Vector3 start, end;
    public GameObject ob1, ob2, ob3, ob4;

    public singleBox() { }

    public singleBox(singleBox box)
    {
        this.start = box.start;
        this.end = box.end;
    }

    public singleBox(Vector2 @start, Vector2 @end)
    {
        this.start = @start;
        this.end = @end;
    }
}
public class Node
{
    public Node Prev;
    public singleBox data;
    public int index;
}