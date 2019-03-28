using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            StartCoroutine("Fade");
        }
    }

    IEnumerator Fade()
    {
        for (float f = 1f; f >= 0; f -= 0.1f)
        {
            Debug.Log(f);

            MeshRenderer r = GetComponent<MeshRenderer>();
            foreach (Material m in r.materials)
            {
                
                // Get the color
                Color c = m.GetColor("_Color");
                
                // Adjust the alpha value to desired alpha value
                c.a = f;
                // Set the color of the material again
                m.SetColor("_Color", c);
            }
            //Color c = this.GetComponent<MeshRenderer>().
            //c.a = f;
            //GetComponent<MeshRenderer>().material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
