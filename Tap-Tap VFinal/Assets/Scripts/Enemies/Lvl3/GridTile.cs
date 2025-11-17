using UnityEngine;

public class GridTile : MonoBehaviour
{
    [Header("Referencias")]
    public Color defaultColor = Color.gray;
    public Color patternColor = Color.green; // El color que marca el patrón a seguir
    public MeshRenderer tileRenderer;
    
    // Indica si esta loseta es parte del patrón actual
    [HideInInspector] public bool isPatternTile = false;
    
    private Material tileMaterial;

    void Start()
    {
        if (tileRenderer == null)
            tileRenderer = GetComponent<MeshRenderer>();
            
        if (tileRenderer != null)
        {
            tileMaterial = tileRenderer.material;
            ResetColor();
        }
    }

    public void SetPattern(bool isPattern)
    {
        isPatternTile = isPattern;
        if (isPatternTile)
        {
            // Cambiar a color de patrón (Verde)
            tileMaterial.color = patternColor;
        }
        else
        {
            // Volver a color por defecto
            ResetColor();
        }
    }

    private void ResetColor()
    {
        tileMaterial.color = defaultColor;
        isPatternTile = false;
    }

    // Opcional: Para devolver la penalización si el jugador se mueve al lugar equivocado
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !isPatternTile)
        {
            // Podrías activar una penalización por estar parado en un lugar NO válido.
            // Ejemplo: PlayerMove.Instance.ApplyStandingPenalty();
        }
    }
}