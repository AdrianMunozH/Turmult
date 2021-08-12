using Field;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Ui.Input
{
    public class AcquireState : ModiState
    {
        private int _cellPrice;

        private HexCoordinates prevCell;
        // Start is called before the first frame update
        public override void Start()
        {
            name = "Acquire";
            //HGameManager.instance.CellPrice = _cellPrice;
            _cellPrice = 40;
            Debug.Log(name + " Mode");
        }

        public override void AcquireField(HCell cell)
        {
            // 
            if (_playerStats.startGold < _cellPrice) return;
            if (cell.Celltype != HCell.CellType.CanBeAcquired) return;


            // vllt sowas wie IncomeManager.Instance.MakePurchase() --- rückgabe bool
            
            cell.Celltype = HCell.CellType.Acquired;
            //cell.acquiredField = GameObject.Find("Cylinder");
            
            //cell.AcquiredThisCellServerRpc();
            cell.SetPrefab(cell.type,cell.Ressource.GetRessourceType());
            cell.StartCoroutine(cell.CheckNeighb());
            // vllt kürzester weg bescheid sagen


            // server rpc
            //cell.SetPrefab(cell.Celltype,cell.Ressource);
        }

        public override void OnDestroy()
        {
            HGrid.Instance.GetCellIndex(prevCell.X, prevCell.Y).gridImage.color =  HGrid.Instance.GetCellIndex(prevCell.X, prevCell.Y).SetColor(0f,0f,0f,0f);;
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
                            if (cell.GetCellType() == HCell.CellType.CanBeAcquired && prevCell.CompareCoord(cell.coordinates))
                            {
                                cell.gridImage.color = cell.SetColor(225f,225f,225f,70f/255f);
                            }
                            
                        }
                        else
                        {
                            HCell prevCellColor = HGrid.Instance.GetCellIndex(prevCell.X, prevCell.Y) ;
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
                        AcquireField(hit.transform.GetComponent<HCell>());
                    }
                }
            }
        }
    }
}
