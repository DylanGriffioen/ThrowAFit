using UnityEngine;

/**
 *  (DD/MM/YY - 06.10.2022) 
 *  @Janek Tuisk
 *  
 *  ColorHSV - https://docs.unity3d.com/ScriptReference/Random.ColorHSV.html
 */

/** TODO
 * 
 */

public class PlayerColorHandler : MonoBehaviour
{
    [SerializeField] bool useRandomColor = true;
    [SerializeField] Color color;
    [SerializeField] Material material;

    // Start is called before the first frame update
    void Start()
    {
        ApplyRandomColorMaterialToPlayer();
    }

    private void ApplyRandomColorMaterialToPlayer()
    {
        if (useRandomColor)
            color = new Color(Random.Range(0, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
            //color = new ColorSHV();

        if (material == null)
            material = new Material(Shader.Find("Standard"));

        material.SetColor("_Color", color);
        gameObject.GetComponent<MeshRenderer>().material = material;
    }
}
