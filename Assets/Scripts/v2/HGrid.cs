using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HGrid : MonoBehaviour
{
    public int width = 6;
    public int height = 6;
    HCell[] cells;
    public HCell cellPrefab;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Awake () {
        
        cells = new HCell[height * width];

        for (int z = 0, i = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                CreateCell(x, z, i++);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void HexMap()
    {
        
    }

    void CreateCell (int x, int z, int i) {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        position.y = 0f;
        position.z = z * 1.5f;

        HCell cell = cells[i] = Instantiate<HCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        /*cell.color = defaultColor;
        Text label = Instantiate<Text>(cellLabelPrefab);
        label.rectTransform.SetParent(gridCanvas.transform, false);
        label.rectTransform.anchoredPosition =
            new Vector2(position.x, position.z);
        label.text = cell.coordinates.ToStringOnSeparateLines();
        */
    }
}
