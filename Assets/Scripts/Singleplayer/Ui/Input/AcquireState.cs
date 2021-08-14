using System;
using Singleplayer.Field;
using UnityEngine;
using UnityEngine.EventSystems;
using Singleplayer.Player;

namespace Singleplayer.Ui.Input
{
    public class AcquireState : ModiState
    {
        private int _cellPrice;
        private HGrid _hexGrid;

        private HexCoordinates prevCell;
        // Start is called before the first frame update
        public override void Start()
        {
            name = "Acquire";
            //HGameManager.instance.CellPrice = _cellPrice;
            _cellPrice = 20;
            _hexGrid = GameObject.Find("HexGrid").GetComponent<HGrid>();
            if (_hexGrid == null) throw new Exception("Kein Objekt HexGrid in der Szene gefunden oder es keine Komponente HGrid an diese! ");
        }

        public override void AcquireField(HCell cell)
        {
            // 
            if (_playerStats.startGold < _cellPrice) return;
            if (cell.Celltype != HCell.CellType.CanBeAcquired) return;

            if(IncomeManager.Instance == null) Debug.Log("Incomemanager darf nicht null sein!");
            if(!IncomeManager.Instance.GoldPurchase(_cellPrice)) return;
            IncomeManager.Instance.GetResource(1,cell.resource.GetResource());
            // vllt sowas wie IncomeManager.Instance.MakePurchase() --- rückgabe bool
            
            cell.Celltype = HCell.CellType.Acquired;
            //cell.acquiredField = GameObject.Find("Cylinder");
            
            //cell.AcquiredThisCellServerRpc();
            cell.SetPrefab(cell.Celltype, cell.resource.GetResource());
            cell.StartCoroutine(cell.CheckNeighb());
            // vllt kürzester weg bescheid sagen


            // server rpc
            //cell.SetPrefab(cell.Celltype,cell.Ressource);
        }

        public override void OnDestroy()
        {
            _hexGrid.GetHCellByXyCoordinates(prevCell.X, prevCell.Y).gridImage.color =  _hexGrid.GetHCellByXyCoordinates(prevCell.X, prevCell.Y).SetColor(0f,0f,0f,0f);;
        }

        public override void Input()
        {
            
            // hover
            RaycastHit hoverHit;
            Ray hoverRay = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);

            if (Physics.Raycast(hoverRay, out hoverHit, float.MaxValue))
            {
                // sollte immer true sein weil with nur eine layer benutzen -- nicht mehr sadge
                if (hoverHit.transform.gameObject.tag == "Cell"  && !EventSystem.current.IsPointerOverGameObject())
                {
                    HCell cell = hoverHit.transform.GetComponent<HCell>();
                    
                        if (prevCell.CompareCoord(cell.coordinates))
                        {
                            if (cell.Celltype == HCell.CellType.CanBeAcquired && prevCell.CompareCoord(cell.coordinates))
                            {
                                cell.gridImage.color = cell.SetColor(225f,225f,225f,70f/255f);
                            }
                            
                        }
                        else
                        {
                            HCell prevCellColor = _hexGrid.GetHCellByXyCoordinates(prevCell.X, prevCell.Y) ;
                            prevCellColor.gridImage.color = prevCellColor.SetColor(0f,0f,0f,0f);
                            prevCell = cell.coordinates;
                        }
                        
                   
                }
            }
            
            //linker mouseclick
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
                {

                    if (hit.transform.gameObject.tag == "Cell"  && !EventSystem.current.IsPointerOverGameObject())
                    {
                        //Benötigt wird aktivieren des TweeningEffekts => Hcell
                        hit.transform.GetComponent<HCell>().recentlyBuild = true;
                        AcquireField(hit.transform.GetComponent<HCell>());
                    }
                }
            }
        }
    }
}
