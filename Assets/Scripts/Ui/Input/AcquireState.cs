using Field;
using UnityEngine;

namespace Ui.Input
{
    public class AcquireState : ModiState
    {
        private int _cellPrice;
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
            // vllt sowas wie IncomeManager.Instance.MakePurchase() --- rückgabe bool
            if (_playerStats.startGold < _cellPrice) return;
            if (cell.Celltype != HCell.CellType.CanBeAcquired) return;



            cell.Celltype = HCell.CellType.Acquired;
            // vllt kürzester weg bescheid sagen


            // server rpc
            //cell.SetPrefab(cell.Celltype,cell.Ressource);
        }
        public override void Input()
        {
            //linker mouseclick
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
                
                // debug
                Debug.Log(ray.origin + " " + ray.direction);
                //
                
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
                {
                    Debug.Log("hier1");
                    if (hit.transform.gameObject.tag == "Cell")
                    {
                        AcquireField(hit.transform.GetComponent<HCell>());
                        
                        Debug.Log("hier2");
                        Debug.Log(hit.transform.GetComponent<HCell>().coordinates + " - Cell");
                    }
                }
            }
        }
    }
}
