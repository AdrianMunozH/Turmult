using Field;
using UnityEngine;

namespace Ui.Input
{
    public class BuildState : ModiState
    {
        private Ressource.RessourceType _ressourceEnum = Ressource.RessourceType.Berg;
        private int currentTurretIndex = 0;

        public Ressource.RessourceType RessourceEnum
        {
            get => _ressourceEnum;
            set => _ressourceEnum = value;
        }

        public int CurrentTurretIndex
        {
            get => currentTurretIndex;
            set => currentTurretIndex = value;
        }
        // Start is called before the first frame update

        

        public override void Start()
        {
            name = "Build";
            Debug.Log(name + " Mode");
            // 
        }

        // wird das hier ein rpc call ?
        public override void BuyTurret(HCell cell, Ressource.RessourceType ressourceEnum, int turret)
        {
            if(cell.HasBuilding || cell.Celltype == HCell.CellType.CanBeAcquired) return;

            
            
            // instiantiate turret 
            // cell den turret geben
            // cell hasbuilding true setzten
        }

        
        // update methode quasi
        public override void Input()
        {
            // hover
            RaycastHit hoverHit;
            Ray hoverRay = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            
            GameObject turretToBuild = BuildManager.instance.GetTurretToBuildPreview(); // welcher tower wird gezeigt ?? muss geändert werden
            GameObject turretPreview;
            
            if (turretToBuild != null)
            {
                //turretPreview = Instantiate(turretToBuild, UnityEngine.Input.mousePosition, new Vector3(0,0,0)); // Utility class braucht eine methode dafür
            }

            if (Physics.Raycast(hoverRay, out hoverHit, float.MaxValue,_layerMask))
            {
                Debug.Log("hit");
                // sollte immer true sein weil with nur eine layer benutzen
                if (hoverHit.transform.gameObject.tag == "Cell")
                {
                    ///turretPreview.transform.DOMove(hover.transform.position,50);
                }
            }







            //linker mouseclick
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 100.0f,_layerMask))
                {
                    if (hit.transform.gameObject.tag == "Cell")
                    {
                        BuyTurret(hit.transform.GetComponent<HCell>(),_ressourceEnum,currentTurretIndex);
                        
                        Debug.Log(hit.transform.gameObject);
                    }
                }
            }
        }
        
        
    }
}