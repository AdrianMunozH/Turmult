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
            
            if ((cell.GetCellType() == HCell.CellType.Acquired ||cell.GetCellType() == HCell.CellType.Neutral) && !cell.hasBuilding)
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
        GetCellIndex(0, 0, 0).SetCellType(HCell.CellType.Neutral);
        int z = 0;
        int p = 1;
        int m = -1;
        for (int i = 0; i < HGameManager.instance.distanceFromSpawn; i++)
        {
            if (i >= 2 && i < HGameManager.instance.distanceFromSpawn-1)
            {
                GetCellIndex(z, m, p).SetCellType(HCell.CellType.Acquired);
                GetCellIndex(p, z, m).SetCellType(HCell.CellType.Acquired);
                GetCellIndex(m, p, z).SetCellType(HCell.CellType.Acquired);
            }
            else
            {
                GetCellIndex(z, m, p).SetCellType(HCell.CellType.Neutral);
                GetCellIndex(p, z, m).SetCellType(HCell.CellType.Neutral);
                GetCellIndex(m, p, z).SetCellType(HCell.CellType.Neutral);
            }

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