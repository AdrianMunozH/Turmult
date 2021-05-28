﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Field
{
    public class HGrid : Singleton<HGrid>
    {
        public int radius;
        public HCell[] cells;
        private List<HCell> cellList;
        public HCell cellPrefab;
        public Text cellLabelPrefab;
        Canvas gridCanvas;

        public Image hexImage;
        
        //test delete later
        public List<HCell> pathTest = new List<HCell>();
        public bool go;
        public bool once = true;

        private void Update()
        {
            if (go && once)
            {
                ShortestPathPrefabs(pathTest.ToArray());
                once = false;
            }
        }

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
            /*
            setze neutral 7 felder  x 
            setze weg
            setze base
            setze array mit den vorher mit index
            setze ressource

             
            foreach (var cell in cells)
            {
                if (cell.GetCellType() != HCell.CellType.Acquired || cell.GetCellType() != HCell.CellType.Neutral)
                {
                    cell.gameObject.SetActive(false);
                }
                
            }
            */
        }

        bool hexCircle(int x, int z, int i)
        {
            Vector3 position;

            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f);
            if (position.magnitude <= radius * HexMetrics.innerRadius)
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

            Image gridImage = Instantiate(hexImage);
            gridImage.rectTransform.SetParent(gridCanvas.transform,false);
            gridImage.rectTransform.anchoredPosition = new Vector2(position.x,position.z);
            Vector3 rot = gridImage.rectTransform.rotation.eulerAngles;
            rot = new Vector3(0,0,45);
            gridImage.rectTransform.Rotate(rot);
            
            
            HCell cell = Instantiate<HCell>(cellPrefab);
            cellList.Add(cell);
            cell.gridImage = gridImage;
            cell.transform.SetParent(transform, false);
            cell.transform.localPosition = position;
            cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.GetComponent<HCell>().index = i;


            //hoverImage.enabled = false;

            /*
            Text label = Instantiate<Text>(cellLabelPrefab);
            label.rectTransform.SetParent(gridCanvas.transform, false);
            label.rectTransform.anchoredPosition =
                new Vector2(position.x, position.z);
            label.text = cell.coordinates.ToStringOnSeparateLines();
            */
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
                if ((cell.GetCellType() == HCell.CellType.Acquired ||cell.GetCellType() == HCell.CellType.Neutral||cell.GetCellType() == HCell.CellType.Base) && !cell.hasBuilding)
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
        public HCell[] NeighbAttack(HCell h)
        {
            //Debug.Log(h.coordinates.ToString() + " ausgewählt");
            HCell[] neighb = new HCell[6];
            int i = 0;
            foreach (HCell cell in cells)
            {
                // Checkt mit absicht nicht nach towern
                if (cell.GetCellType() == HCell.CellType.Acquired ||cell.GetCellType() == HCell.CellType.Neutral)
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
            GetCellIndex(0, 0, 0).SetCellType(HCell.CellType.Base); // portal
            GetCellIndex(0,0,0).SetPrefab(19,new Vector3(0,60,0));
            
            GetCellIndex(0, 1,-1).SetCellType(HCell.CellType.Base);// arch1
            GetCellIndex(0, 1,-1).SetPrefab(20,new Vector3(0,-120,0)); //-30
            
            GetCellIndex(1, -1,0).SetCellType(HCell.CellType.Base); // arch2
            GetCellIndex(1, -1,0).SetPrefab(20,new Vector3(0,-240,0)); //-150
            
            GetCellIndex(-1, 0,1).SetCellType(HCell.CellType.Base); // arch3
            GetCellIndex(-1, 0,1).SetPrefab(20);
            
            int z = 0;
            int p = 1;
            int m = -1;
            for (int i = 0; i < HGameManager.instance.distanceFromSpawn; i++)
            {
                if (i >= 1 && i < HGameManager.instance.distanceFromSpawn-1)
                {
                    pathTest.Add(GetCellIndex(z, m, p));
                    GetCellIndex(z, m, p).SetCellType(HCell.CellType.Acquired);
                    GetCellIndex(p, z, m).SetCellType(HCell.CellType.Acquired);
                    GetCellIndex(m, p, z).SetCellType(HCell.CellType.Acquired);
                }
                else
                {
                    if (i == HGameManager.instance.distanceFromSpawn - 1)
                    {
                        GetCellIndex(z, m,p).SetPrefab(22); // base
                        GetCellIndex(p, z,m).SetPrefab(22);
                        GetCellIndex(m, p,z).SetPrefab(22);
                    }
                    else
                    {
                        GetCellIndex(z, m,p).SetPrefab(21,new Vector3(0,-300,0));
                        GetCellIndex(p, z,m).SetPrefab(21,new Vector3(0,180,0));
                        GetCellIndex(m, p,z).SetPrefab(21,new Vector3(0,-60,0));
                    }
                        
                    GetCellIndex(z, m, p).SetCellType(HCell.CellType.Base); // base ?
                    
                    GetCellIndex(p, z, m).SetCellType(HCell.CellType.Base);
                    
                    GetCellIndex(m, p, z).SetCellType(HCell.CellType.Base);
                    
                }

                p++;
                m--;
            }
            
        }

        public void ShortestPathPrefabs(HCell[] path)
        {
            int x;
            int y;
            int z;
            if(path == null)
                return;
            for (int i = 2; i < path.Length - 1; i++)
            {

                x = Math.Abs(path[i - 1].coordinates.X) - Math.Abs(path[i + 1].coordinates.X);
                y = Math.Abs(path[i - 1].coordinates.Y) - Math.Abs(path[i + 1].coordinates.Y);
                z = Math.Abs(path[i - 1].coordinates.Z) - Math.Abs(path[i + 1].coordinates.Z);

                // wenn x+z+y = 4  dann ist es gerade, wenn x+z+y = 3  ecke
                Debug.Log(x + " : " +y + " : " +z);
                if (Math.Abs(x + z + y) == 4)
                {
                    Straight(path[i], x, y, z);
                }
                else
                {
                    Corner(path[i], x, y, z);
                }
            }



        }

        public void Corner(HCell path,int x, int y, int z)
        {
            int str;
            // neutral hat weniger felder als ressourcen
            if (path.Ressource.GetRessourceType() == Ressource.RessourceType.Neutral)
                str = 2;
            else
                str = 3;
            int absx = Math.Abs(x);
            int absy = Math.Abs(y);
            int absz = Math.Abs(z);

            if (absx == 1 && absy == 1 && absz == 2)
            {
                path.SetPrefab(path.GetCellType(),path.Ressource.GetRessourceType(),new Vector3(0,0,0),str);
            } else if (absx == 2 && absy == 1 && absz == 1)
            {
                path.SetPrefab(path.GetCellType(),path.Ressource.GetRessourceType(),new Vector3(0,210,0),str);
            }
            
        }

        public void Straight(HCell path,int x, int y,int z)
        {
            int str;
            // neutral hat weniger felder als ressourcen
            if (path.Ressource.GetRessourceType() == Ressource.RessourceType.Neutral)
                str = 1;
            else
                str = 2;
            
            //das ist nur für straight
            if (x != 0 && y != 0 && z == 0)
            {
                path.SetPrefab(path.GetCellType(),path.Ressource.GetRessourceType(),new Vector3(0,120,0),str);
            } else if (y != 0 && z != 0 && x == 0)
            {
                path.SetPrefab(path.GetCellType(),path.Ressource.GetRessourceType(),new Vector3(0,-120,0),str);
            }
            else
            {
                path.SetPrefab(path.GetCellType(),path.Ressource.GetRessourceType(),Vector3.zero,str);
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
    
}