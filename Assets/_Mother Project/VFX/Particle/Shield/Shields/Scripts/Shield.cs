using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    Renderer renderer;
    [SerializeField] AnimationCurve DisplacementCurve;
    [SerializeField] float DisplacementMagnitude;
    [SerializeField] float LerpSpeed;
    [SerializeField] float DisolveSpeed;
    bool shieldOn;
    Coroutine disolveCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        shieldOn = false;
        OpenCloseShield();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                HitShield(hit.point);
            }
        }
      /*  if (Input.GetKeyDown(KeyCode.F))
        {
            OpenCloseShield();
        }*/
    }

    public void HitShield(Vector3 hitPos)
    {
        renderer.material.SetVector("_HitPos", hitPos);
        StopAllCoroutines();
        StartCoroutine(Coroutine_HitDisplacement());
    }

    public void OpenCloseShield()
    {
        float target = 1;
        if (shieldOn)
        {
            target = 0;
        }
        shieldOn = !shieldOn;
        if (disolveCoroutine != null)
        {
            StopCoroutine(disolveCoroutine);
        }
        disolveCoroutine = StartCoroutine(Coroutine_DisolveShield(target));
    }

    IEnumerator Coroutine_HitDisplacement()
    {
        float lerp = 0;
        while (lerp < 1)
        {
            renderer.material.SetFloat("_DisplacementStrength", DisplacementCurve.Evaluate(lerp) * DisplacementMagnitude);
            lerp += Time.deltaTime*LerpSpeed;
            yield return null;
        }
    }

    IEnumerator Coroutine_DisolveShield(float target)
    {
        float start = renderer.material.GetFloat("_Disolve");
        float lerp = 0;
        while (lerp < 1)
        {
            renderer.material.SetFloat("_Disolve", Mathf.Lerp(start,target,lerp));
            lerp += Time.deltaTime * DisolveSpeed;
            yield return null;
        }
    }
}
