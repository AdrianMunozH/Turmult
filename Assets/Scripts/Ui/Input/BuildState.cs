using System;
using Field;
using UnityEngine;

namespace Ui.Input
{
    public class BuildState : ModiState
    {
        private Ressource.RessourceType _ressourceEnum = Ressource.RessourceType.Berg;
        private int currentTurretIndex = 0;
        
        private HexCoordinates prevCell;
        private GameObject previewTurret;

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

        // nur für baschi
        public override void OnDestroy()
        {
            HGrid.Instance.GetCellIndex(prevCell.X,prevCell.Y).DestroyPreviewTurret();
        }

        // wird das hier ein rpc call ?
        public override void BuyTurret(HCell cell, Ressource.RessourceType ressourceEnum, int turret)
        {
            Debug.Log(!cell.hasBuilding + " " + cell.Celltype.ToString() + cell.Ressource.GetRessourceType());
            if(cell.HasBuilding || cell.Celltype != HCell.CellType.Acquired || cell.Ressource.GetRessourceType() != Ressource.RessourceType.Neutral) return;

            Debug.Log("buyturret");
            cell.BuildTurret();
            
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
            

            if (Physics.Raycast(hoverRay, out hoverHit, float.MaxValue))
            {
                // sollte immer true sein weil with nur eine layer benutzen -- nicht mehr sadge
                if (hoverHit.transform.gameObject.tag == "Cell")
                {
                    HCell cell = hoverHit.transform.GetComponent<HCell>();
                    
                    // gleiche zelle -> turret prefab wird nicht neu gesetzt 
                    if (!prevCell.CompareCoord(cell.coordinates))
                    {
                        // neue prefab
                        if (cell.GetCellType() == HCell.CellType.Acquired && !cell.HasBuilding && cell.Ressource.GetRessourceType() != Ressource.RessourceType.Neutral)
                        {

                            GameObject turretToBuild = BuildManager.instance.GetTurretToBuildPreview(); 
                            if (turretToBuild != null)
                            {
                                cell.previewTurret = (GameObject) cell.InstantiateTurretPreview(turretToBuild);
                            } 
                        }
                        
                        // altes prefab
                        HGrid.Instance.GetCellIndex(prevCell.X,prevCell.Y).DestroyPreviewTurret();
                        
                        
                        // machen wir immer
                        prevCell = cell.coordinates;
                    }
                        
                   
                }
            }
            


            //linker mouseclick
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);

                if (Physics.Raycast(ray, out hit, float.MaxValue))
                {
                    if (hit.transform.gameObject.tag == "Cell")
                    {
                        BuyTurret(hit.transform.GetComponent<HCell>(),_ressourceEnum,currentTurretIndex);
                        
                    }
                }
            }
        }
        
        
    }
    
}