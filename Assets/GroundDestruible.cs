using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class DestructibleTilemap : MonoBehaviour
{
    [Header("Configuración")]
    public float destructionDelay = 0.2f;
    
    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    private Tilemap tilemap;

    private void Awake()
    {
        tilemap = GetComponent<Tilemap>();
        if (debugMode) Debug.Log($"Tilemap asignado: {tilemap.name}");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (debugMode) Debug.Log($"Colisión detectada con: {collision.gameObject.name}");

        if (collision.gameObject.CompareTag("Player"))
        {
            if (debugMode) Debug.Log("Es un jugador");

            ContactPoint2D contact = collision.GetContact(0);
            Vector3 hitPosition = contact.point;
            Vector3Int tilePosition = tilemap.WorldToCell(hitPosition);

            if (debugMode) Debug.Log($"Posición del tile: {tilePosition}");

            if (tilemap.GetTile(tilePosition) != null)
            {
                if (debugMode) Debug.Log($"Tile encontrado en posición: {tilePosition}");
                StartCoroutine(DestroyTile(tilePosition));
            }
            else
            {
                if (debugMode) Debug.LogWarning($"No hay tile en {tilePosition}");
            }
        }
    }

    private System.Collections.IEnumerator DestroyTile(Vector3Int tilePos)
    {
        if (debugMode) Debug.Log($"Destruyendo tile en {tilePos} (esperando {destructionDelay}s)");
        
        yield return new WaitForSeconds(destructionDelay);
        
        tilemap.SetTile(tilePos, null);
        
        if (debugMode) Debug.Log($"Tile en {tilePos} ha sido destruido");
    }

    // Método para visualizar los tiles en el Editor
    private void OnDrawGizmosSelected()
    {
        if (!debugMode || tilemap == null) return;
        
        Gizmos.color = Color.red;
        BoundsInt bounds = tilemap.cellBounds;
        
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(pos) != null)
                {
                    Gizmos.DrawWireCube(tilemap.GetCellCenterWorld(pos), tilemap.cellSize);
                }
            }
        }
    }
}