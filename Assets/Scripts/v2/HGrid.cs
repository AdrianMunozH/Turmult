using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class HGrid : Singleton<HGrid>
{
    public int width = 6;
    public int height = 6;
    public int radius;
    public HCell[] cells;
    private List<HCell> cellList;
    public HCell cellPrefab;
    public Text cellLabelPrefab;
    Canvas gridCanvas;

    //test für shortest path

    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
        gridCanvas = GetComponentInChildren<Canvas>();

        cellList = new List<HCell>();

        for (int z = -radius, i = 0; z < radius; z++)
        {
            for (int x = -radius; x < radius; x++)
            {
                if (hexCircle(x, z, i))
                {
                    i++;
                }
            }
        }

        cells = new HCell[cellList.Count];
        cells = cellList.ToArray();
        neutralCell();
    }

    bool hexCircle(int x, int z, int i)
    {
        Vector3 position;

        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);
        if (position.magnitude <= radius * HexMetrics.outerRadius)
        {
            CreateCell(x, z, i);
            return true;
        }

        return false;
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;

        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * (HexMetrics.outerRadius * 1.5f);

        HCell cell = Instantiate<HCell>(cellPrefab);
        cellList.Add(cell);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.GetComponent<HCell>().index = i;

        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
    }

    void TouchCell(Vector3 position)
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        HighlightCell(coordinates, position);
        Debug.Log("touched at " + coordinates.ToString());
    }


    // könnte auch zu enemy spawn
    public Vector3[] HCellPositions(HCell[] sp)
    {
        Vector3[] positions = new Vector3[sp.Length];
        for (int i = 0; i < sp.Length; i++)
        {
            positions[i] = sp[i].transform.position;
        }

        return positions;
    }


    public void HighlightCell(HexCoordinates hexCoordinates, Vector3 pos)
    {
        Vector3 position = pos;
        position.y = 1;
        HCell cell = Instantiate<HCell>(cellPrefab);
        cell.transform.localPosition = position;
        cell.coordinates = hexCoordinates;
    }

    //works
    public void HighlightCell2(Vector3 pos)
    {
        Vector3 position = pos;
        position.y = 1;
        HCell cell = Instantiate<HCell>(cellPrefab);
        cell.transform.localPosition = position;
        //cell.coordinates = hexCoordinates;
    }

    public HCell[] Neighb(HCell h)
    {
        //Debug.Log(h.coordinates.ToString() + " ausgewählt");
        HCell[] neighb = new HCell[6];
        int i = 0;
        foreach (HCell cell in cells)
        {
            // TODO: fehlt der check ob es schon ein gebäude hat
            if (cell.isAcquired && !cell.hasBuilding)
            {
                if (cell.coordinates.X == h.coordinates.X - 1 && cell.coordinates.Y == h.coordinates.Y)
                {
                    neighb[i] = cell;
                    i++;
                }
                else if (cell.coordinates.X == h.coordinates.X && cell.coordinates.Y == h.coordinates.Y - 1)
                {
                    neighb[i] = cell;
                    i++;
                }
                else if (cell.coordinates.X == h.coordinates.X + 1 && cell.coordinates.Y == h.coordinates.Y)
                {
                    neighb[i] = cell;
                    i++;
                }
                else if (cell.coordinates.X == h.coordinates.X && cell.coordinates.Y == h.coordinates.Y + 1)
                {
                    neighb[i] = cell;
                    i++;
                }
                else if (cell.coordinates.X == h.coordinates.X - 1 && cell.coordinates.Y == h.coordinates.Y + 1)
                {
                    neighb[i] = cell;
                    i++;
                }
                else if (cell.coordinates.X == h.coordinates.X + 1 && cell.coordinates.Y == h.coordinates.Y - 1)
                {
                    neighb[i] = cell;
                    i++;
                }
            }
        }
        /*
        for (int j = 0; j < i; j++)
        {
            Debug.Log(neighb[j].coordinates.ToString());
        }
        */

        return WithoutNull(neighb);
    }

    private HCell[] WithoutNull(HCell[] arr)
    {
        int i = 0;
        foreach (HCell h in arr)
        {
            if (h != null)
                i++;
        }

        HCell[] end = new HCell[i];
        for (int j = 0; j < i; j++)
        {
            end[j] = arr[j];
        }

        return end;
    }

    /*
    public HCell[] Solve(HCell start)
    {
        Queue<HCell> queue = new Queue<HCell>();
        queue.Enqueue(start);
        //index stellen
        List<int> visited = new List<int>();
        visited.Add(start.index);
        HCell[] prev = new HCell[cells.Length];
        int p = 0;
        while (queue.Count > 0)
        {
            HCell node = queue.Dequeue();
            HCell[] neighb = Neighb(node);
            for (int i = 0;i<neighb.Length;i++)
            {
                //nicht visited
                //if (!visited.Contains(CellsIndex(neighb[i])))
                if (!Visited(visited,neighb[i].index))
                {
                    queue.Enqueue(neighb[i]);
                    //visited.Add(CellsIndex(neighb[i]));
                    visited.Add(neighb[i].index);
                    //prev[i] = node;
                    prev[neighb[i].index] = node;
                    prev[neighb[i].index].spindex = p;
                    p++;
                }
            }
        }
        Debug.Log("solve list: " + ArrayToString(prev));
        return prev;
    }

    public bool Visited(List<int> list, int i)
    {
        foreach (int item in list)
        {
            if (item.Equals(i))
            {
                return true;
            }
        }
        return false;
    }
    */
    // wird nicht mehr gebraucht
    public int CellsIndex(HCell hCell)
    {
        int index = 0;
        for (int i = 0; i < cells.Length; i++)
        {
            if (hCell.coordinates.X.Equals(cells[i].coordinates.X) &&
                hCell.coordinates.Y.Equals(cells[i].coordinates.Y))
            {
                index = i;
            }
        }

        //gibt probleme wenn Hcell nicht teil des arrays ist
        return index;
    }

    /*

    public List<HCell> RecPath(HCell start,HCell end, HCell[] list)
    {
        List<HCell> path = new List<HCell>();
        Debug.Log("sp start"+ start.spindex + " index start"+ start.index );
        Debug.Log("sp end"+ end.spindex + " index start"+ end.index );
        Debug.Log("ende: " + end.coordinates.ToString());


        for (HCell at = end; at != null ; at = list[at.index])
        {
            path.Add(at);
        }
        return path;
        
    }

    public List<HCell> ShortestPath(List<HCell> path)
    {
        
        path.Reverse();
        if (path.Count > 0 && path[0].coordinates.X == start.coordinates.X)
        {
            return path;
        }
        Debug.Log("leer");
        // wenn der weg nicht möglich ist kommt eine leere liste zurück // muss also gecheckt werden
        return new List<HCell>();
    }
    */
    public HCell GetCellIndex(int x, int y)
    {
        foreach (HCell cell in cells)
        {
            if (cell.coordinates.X == x && cell.coordinates.Y == y)
            {
                return cell;
            }
        }

        return null;
    }

    private void neutralCell()
    {
        GetCellIndex(0, 0, 0).isAcquired = true;
        int z = 0;
        int p = 1;
        int m = -1;
        for (int i = 0; i < 5; i++)
        {
            GetCellIndex(z, m, p).isAcquired = true;
            GetCellIndex(p, z, m).isAcquired = true;
            GetCellIndex(m, p, z).isAcquired = true;
            p++;
            m--;
        }
    }

    public HCell GetCellIndex(int x, int y, int z)
    {
        foreach (HCell cell in cells)
        {
            if (cell.coordinates.X == x && cell.coordinates.Y == y && cell.coordinates.Z == z)
            {
                return cell;
            }
        }

        return null;
    }

    public string ArrayToString(HCell[] list)
    {
        string s = "";
        foreach (HCell h in list)
        {
            //s += h.coordinates.ToString() + "\n";
            if (h != null)
                s += h.coordinates.ToString() + "\n";
            else
                s += "(null) \n";
        }

        return s;
    }

    public string ListToString(List<HCell> list)
    {
        string s = "";
        foreach (HCell h in list)
        {
            //s += h.coordinates.ToString() + "\n";
            s += h.coordinates.ToString() + "\n";
        }

        return s;
    }
}