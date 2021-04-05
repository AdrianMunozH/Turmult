using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;

public class HGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    HCell[] cells;
    public HCell cellPrefab;
    public Text cellLabelPrefab;
    Canvas gridCanvas;
    
    //test für shortest path
    private HCell[] spath;
    int input;
    HCell start;
    HCell end;

    public EnemySpawn enemySpawn;
    //test für shortest path

    private bool cooldown;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake () {
        
        gridCanvas = GetComponentInChildren<Canvas>();
        cells = new HCell[height * width];

        for (int z = 0, i = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                CreateCell(x, z, i++);
            }
        }
    }
    
    void HexMap()
    {
        
    }

    void CreateCell (int x, int z, int i) {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z *(HexMetrics.outerRadius * 1.5f);

        HCell cell = cells[i] = Instantiate<HCell>(cellPrefab);
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
    void TouchCell (Vector3 position) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        HighlightCell(coordinates, position);
        Debug.Log("touched at " + coordinates.ToString());
    }
    void Update () {
        if (Input.GetMouseButton(0) && !cooldown) {
            HandleInput();
        }
    }

    void HandleInput () {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if (Physics.Raycast(inputRay, out hit)) {
            //HighlightCell2(hit.collider.gameObject.transform.position);

            if (input == 0)
            {
                OnCooldown();
                input++;
                start = hit.collider.gameObject.GetComponent<HCell>();
                Debug.Log("Start" + start.coordinates.ToString());
                spath = Solve(start);
                
                

            } else if (input == 1)
            {
                OnCooldown();
                end = hit.collider.gameObject.GetComponent<HCell>();
                List<HCell> sp = RecPath(start, end, spath);
                sp = ShortestPath(sp);
                Debug.Log("***Ergebnis*** " + ListToString(sp) + " : "+ sp.Count);
                input = 0;
                if(sp.Count > 0)
                    enemySpawn.SpawnEnemy(HCellPositions(sp));
            }
            //Neighb(hit.collider.gameObject.GetComponent<HCell>());
            
            //TouchCell(hit.point);
        }
    }
    // könnte auch zu enemy spawn
    private Vector3[] HCellPositions(List<HCell> sp)
    {
        Vector3[] positions = new Vector3[sp.Count];
        for (int i = 0; i < sp.Count; i++)
        {
            positions[i] =  sp[i].transform.position;
        }

        return positions;
    }
    private void OnCooldown()
    {
        cooldown = true;
        StartCoroutine("ResetCooldown");
    }

    IEnumerator ResetCooldown(){
        yield return new WaitForSeconds(1f);
        cooldown = false;
    }

    public void HighlightCell(HexCoordinates hexCoordinates,Vector3 pos)
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
            if (cell.coordinates.X == h.coordinates.X - 1 && cell.coordinates.Y == h.coordinates.Y)
            {
                neighb[i] = cell;
                i++;
            } else if (cell.coordinates.X == h.coordinates.X  && cell.coordinates.Y == h.coordinates.Y-1)
            {
                neighb[i] = cell;
                i++;
            }else if (cell.coordinates.X == h.coordinates.X+1  && cell.coordinates.Y == h.coordinates.Y)
            {
                neighb[i] = cell;
                i++;
            }else if (cell.coordinates.X == h.coordinates.X  && cell.coordinates.Y == h.coordinates.Y+1)
            {
                neighb[i] = cell;
                i++;
            }else if (cell.coordinates.X == h.coordinates.X-1  && cell.coordinates.Y == h.coordinates.Y+1)
            {
                neighb[i] = cell;
                i++;
            }
            else if (cell.coordinates.X == h.coordinates.X+1  && cell.coordinates.Y == h.coordinates.Y-1)
            {
                neighb[i] = cell;
                i++;
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
    
    // wird nicht mehr gebraucht
    public int CellsIndex(HCell hCell)
    {
        int index = 0;
        for (int i = 0; i < cells.Length; i++)
        {
            if (hCell.coordinates.X.Equals(cells[i].coordinates.X) && hCell.coordinates.Y.Equals(cells[i].coordinates.Y))
            {
                index = i;
            }
        }
        //gibt probleme wenn Hcell nicht teil des arrays ist
        return index;
    }

    public List<HCell> RecPath(HCell start,HCell end, HCell[] list)
    {
        List<HCell> path = new List<HCell>();
        Debug.Log("sp start"+ start.spindex + " sp ende"+ end.spindex );
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

    public string ArrayToString(HCell[] list)
    {
        string s = "";
        foreach (HCell h in list)
        {
            //s += h.coordinates.ToString() + "\n";
            if(h != null)
                s += h.coordinates.ToString()+ "\n";
            else
                s += "null \n";
        }

        return s;
    }
    public string ListToString(List<HCell> list)
    {
        string s = "";
        foreach (HCell h in list)
        {
            //s += h.coordinates.ToString() + "\n";
            s += h.coordinates.ToString()+ "\n";
        }

        return s;
    }
}
