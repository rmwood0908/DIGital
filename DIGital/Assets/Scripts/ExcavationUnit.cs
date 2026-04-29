using UnityEngine;
using System.Collections.Generic;

public class ExcavationUnit
{
    private readonly HashSet<Vector2Int> cells;
    private readonly List<GameObject> stakes;
    private readonly GameObject ropeObject;

    // maxDepth[cell] = the lowest digLayer index that exists for that cell (0 = deepest).
    private readonly Dictionary<Vector2Int, int> maxDepthPerCell;

    // depthReached[cell] = the lowest digLayer index dug so far in that cell.
    private readonly Dictionary<Vector2Int, int> depthReachedPerCell;

    public ExcavationUnit(
        List<Vector2Int> cells,
        List<GameObject> stakes,
        GameObject ropeObject,
        Dictionary<Vector2Int, int> maxDepthPerCell)
    {
        this.cells = new HashSet<Vector2Int>(cells);
        this.stakes = stakes;
        this.ropeObject = ropeObject;
        this.maxDepthPerCell = maxDepthPerCell;
        this.depthReachedPerCell = new Dictionary<Vector2Int, int>();
    }

    public bool ContainsCell(Vector2Int cell) => cells.Contains(cell);

    public IEnumerable<Vector2Int> Cells => cells;

    public void RecordCellDepthReached(Vector2Int cell, int layerIndex)
    {
        if (!cells.Contains(cell)) return;
        if (!depthReachedPerCell.ContainsKey(cell) || layerIndex < depthReachedPerCell[cell])
            depthReachedPerCell[cell] = layerIndex;
    }

    public bool IsComplete()
    {
        int shallowLimit = int.MinValue;
        foreach (var kvp in maxDepthPerCell)
            if (kvp.Value > shallowLimit) shallowLimit = kvp.Value;

        foreach (var cell in cells)
        {
            if (!depthReachedPerCell.ContainsKey(cell)) return false;
            if (depthReachedPerCell[cell] > shallowLimit) return false;
        }
        return true;
    }

    public void Despawn()
    {
        foreach (var stake in stakes)
            if (stake != null) Object.Destroy(stake);
        if (ropeObject != null) Object.Destroy(ropeObject);
    }

    public int GetDeepestLayerIndex()
    {
        int deepest = int.MaxValue;
        foreach (var kvp in maxDepthPerCell)
            if (kvp.Value < deepest) deepest = kvp.Value;
        return deepest == int.MaxValue ? 0 : deepest;
    }   
}