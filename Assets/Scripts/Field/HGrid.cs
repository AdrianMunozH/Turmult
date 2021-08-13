using System;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
using UnityEngine.UI;

namespace Field
{
    public class HGrid : NetworkBehaviour
    {
        public int radius;
        public HCell[] cells;
        private List<HCell> cellList;
        public HCell cellPrefab;
        Canvas gridCanvas;
        [SerializeField] private Material[] fieldMaterial;

        public Material[] FieldMaterial => fieldMaterial;

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
            if (position.magnitude <= radius * HexMetrics.innerRadius)
            {
                CreateCell(x, z, i);
                return true;
            }

            return false;
        }

        [ClientRpc]
        public void SetResourceClientRpc(int x, int y)
        {
            GetHCellByXyCoordinates(x, y).ressource.SetRandomType();
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

            //Celltypen ändern wenn Server am Start ist, über ClienRPC die clients damit aktualisieren
            



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


        public HCell GetHCellByXyCoordinates(int x, int y)
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
        
        public HCell GetHCellByIndex(int index)
        {
            return cells.FirstOrDefault(cell => cell.index == index);
        }
        
        private void neutralCell()
        {
            GetHCellByXyzCoordinates(0, 0, 0).SetCellType(HCell.CellType.Base); // portal
            GetHCellByXyzCoordinates(0,0,0).SetPrefab(19,new Vector3(0,60,0));
            
            GetHCellByXyzCoordinates(0, 1,-1).SetCellType(HCell.CellType.Base);// arch1
            GetHCellByXyzCoordinates(0, 1,-1).SetPrefab(20,new Vector3(0,-120,0)); //-30
            
            GetHCellByXyzCoordinates(1, -1,0).SetCellType(HCell.CellType.Base); // arch2
            GetHCellByXyzCoordinates(1, -1,0).SetPrefab(20,new Vector3(0,-240,0)); //-150
            
            GetHCellByXyzCoordinates(-1, 0,1).SetCellType(HCell.CellType.Base); // arch3
            GetHCellByXyzCoordinates(-1, 0,1).SetPrefab(20);
            
            int z = 0;
            int p = 1;
            int m = -1;
            for (int i = 0; i < HGameManager.instance.distanceFromSpawn; i++)
            {
                if (i >= 1 && i < HGameManager.instance.distanceFromSpawn-1)
                {
                    pathTest.Add(GetHCellByXyzCoordinates(z, m, p));
                    GetHCellByXyzCoordinates(z, m, p).SetCellType(HCell.CellType.Acquired);
                    GetHCellByXyzCoordinates(p, z, m).SetCellType(HCell.CellType.Acquired);
                    GetHCellByXyzCoordinates(m, p, z).SetCellType(HCell.CellType.Acquired);
                }
                else
                {
                    if (i == HGameManager.instance.distanceFromSpawn - 1)
                    {
                        GetHCellByXyzCoordinates(z, m,p).SetPrefab(22); // base
                        GetHCellByXyzCoordinates(p, z,m).SetPrefab(22);
                        GetHCellByXyzCoordinates(m, p,z).SetPrefab(22);
                    }
                    else
                    {
                        GetHCellByXyzCoordinates(z, m,p).SetPrefab(21,new Vector3(0,-300,0));
                        GetHCellByXyzCoordinates(p, z,m).SetPrefab(21,new Vector3(0,180,0));
                        GetHCellByXyzCoordinates(m, p,z).SetPrefab(21,new Vector3(0,-60,0));
                    }
                        
                    GetHCellByXyzCoordinates(z, m, p).SetCellType(HCell.CellType.Base); // base ?
                    
                    GetHCellByXyzCoordinates(p, z, m).SetCellType(HCell.CellType.Base);
                    
                    GetHCellByXyzCoordinates(m, p, z).SetCellType(HCell.CellType.Base);
                    
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


                Vector3 magnVector = path[i+1].coordinates.GetVector() - path[i-1].coordinates.GetVector();
                
                if (magnVector.x != 0 && magnVector.y != 0 && magnVector.z != 0)
                {
                    Corner(path[i-1],path[i],path[i+1]);
                    
                }
                else
                {
                    Straight(path[i], x, y, z);
                    
                }
            }



        }

        private Vector3 prevVec;
        private Vector3 nextVec;
        private Vector3 ac;
        private Vector3 control;
        
        private void OnDrawGizmos()
        {
            
            //Gizmos.DrawLine(prevVec,nextVec);
            // Gizmos.DrawRay(prevVec,ac);
            Gizmos.DrawRay(new Vector3(),control);
        }

        public void Corner(HCell prev,HCell curr,HCell next)
        {
            int str;
            // neutral hat weniger felder als ressourcen
            if (curr.ressource.GetRessourceType() == Ressource.RessourceType.Neutral)
                str = 2;
            else
                str = 3;

            prevVec = prev.transform.position;
            nextVec = next.transform.position;

            // a = prev, b = curr , c=next
            Vector3 ac = next.transform.position - prev.transform.position;
            Vector3 ab = curr.transform.position - prev.transform.position;
            this.control = GetHCellByXyzCoordinates(prev.coordinates.X - 1, prev.coordinates.Y - 1, prev.coordinates.Z + 2).transform.position - prev.transform.position;
            
            this.ac = ac;
            
            var angle = Vector3.SignedAngle(ac, ab,new Vector3(0,1,0));


            // Hilfvektor senkr. nach oben für die winkelberechnung++
            Vector3 control = GetHCellByXyzCoordinates(prev.coordinates.X - 1, prev.coordinates.Y - 1, prev.coordinates.Z + 2).transform.position - prev.transform.position;
           /*
            prev.lineRenderer.enabled = true;
            prev.lineRenderer.SetPosition(0,prev.transform.position);
            prev.lineRenderer.SetPosition(1,prev.transform.position+ac.normalized);
            */



            //Vector3 control = new Vector3(prev.coordinates.X -1, prev.coordinates.Y -1,prev.coordinates.Z+2);
            //var rotationOfPrefab = (float) (Math.Acos(Vector3.Dot(ac, control) / (ac.magnitude * control.magnitude)) * 180/Math.PI);
            var rotationOfPrefab = Vector3.Angle(ac, control);

            if (angle > 0)
            {
                
                curr.SetPrefab(curr.GetCellType(),curr.ressource.GetRessourceType(),new Vector3(0,rotationOfPrefab,0),str);
            }
            else
            {
                curr.SetPrefab(curr.GetCellType(),curr.ressource.GetRessourceType(),new Vector3(0,rotationOfPrefab+180f,0),str);
            }
            
           
            
            
            /*
            if (absx == 1 && absy == 1 && absz == 2)
            {
                path.SetPrefab(path.GetCellType(),path.Ressource.GetRessourceType(),new Vector3(0,0,0),str);
            } else if (absx == 2 && absy == 1 && absz == 1)
            {
                path.SetPrefab(path.GetCellType(),path.Ressource.GetRessourceType(),new Vector3(0,120,0),str);
            }
            */

        }

        public void Straight(HCell path,int x, int y,int z)
        {
            int str;
            
            if (path.ressource.GetRessourceType() == Ressource.RessourceType.Neutral)
                str = 1;
            else
                str = 2;
            
            //das ist nur für straight
            if (x != 0 && y != 0 && z == 0)
            {
                path.SetPrefab(path.GetCellType(),path.ressource.GetRessourceType(),new Vector3(0,120,0),str);
            } else if (y != 0 && z != 0 && x == 0)
            {
                path.SetPrefab(path.GetCellType(),path.ressource.GetRessourceType(),new Vector3(0,-120,0),str);
            }
            else
            {
                path.SetPrefab(path.GetCellType(),path.ressource.GetRessourceType(),Vector3.zero,str);
            }
                
        
        }

        public HCell GetHCellByXyzCoordinates(int x, int y, int z)
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