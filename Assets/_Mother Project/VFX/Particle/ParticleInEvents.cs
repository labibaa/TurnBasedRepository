using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleInEvents : MonoBehaviour
{
    [SerializeField] ParticleSystem particleSystem; 
    [SerializeField] ParticleSystem particleSystemBoom; 
    [SerializeField] GameObject monChara;
    [SerializeField] GameObject DevourGameObject;

    [SerializeField]Shield shield;
    float scaleFactor = 80f;

    private void Start()
    {
        particleSystem.Stop();
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.M))
        {
          // monChara.GetComponent<Animator>().Play("WitchesBolt");
          //  monChara.GetComponent<Animator>().Play("Dodge"); //go
            //monChara.GetComponent<Animator>().Play("Seduce");
           // monChara.GetComponent<Animator>().Play("CaptivatingPerformance");
           //   monChara.GetComponent<Animator>().Play("Devour");   //go
              monChara.GetComponent<Animator>().Play("Block");  //go
            // monChara.GetComponent<Animator>().Play("FearTacticts");
        }
        if(Input.GetKeyDown(KeyCode.P)) 
        {
            particleSystemBoom.Play();
        }*/




    }

    void PlayParticle()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                HitShield(hit.point);
            }
        }*/
       // particleSystem.Play();
        shield.OpenCloseShield();
    }
    void StopParticle()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                HitShield(hit.point);
            }
        }*/
        //particleSystem.Stop();
        shield.OpenCloseShield();
    }

    void PlaySkeleton()
    {

        // Instantiate(Instantiate(DevourGameObject, this.transform.position+ new Vector3(0f,1f,-0.128f), Quaternion.identity));
        DevourGameObject.SetActive(true);
       
    }
    void StopSkeleton()
    {

        // Instantiate(Instantiate(DevourGameObject, this.transform.position+ new Vector3(0f,1f,-0.128f), Quaternion.identity));
        DevourGameObject.SetActive(false);

    }




}