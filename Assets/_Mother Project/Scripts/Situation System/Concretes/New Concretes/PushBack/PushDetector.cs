using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PushDetector : MonoBehaviour
{

    public CharacterBaseClasses currentPlayer;

   /* private void OnTriggerEnter(Collider other)
    {

        


        if (other.tag == StringData.PlayerTag || other.tag == StringData.Ally)
        {
            
            if (other.GetComponent<ObjectToBePushed>().IsShoot)
            {

                //currentPlayer = other.GetComponent<PushDetector>().currentPlayer;
                //currentPlayer = other.GetComponent<CharacterBaseClasses>();
                other.GetComponent<ObjectToBePushed>().IsShoot = false;
                Debug.LogError("-----------------who git hit:  " + gameObject.name + "who hit:  " + other.gameObject.name +
                    "-----------");
                Vector2 thisPosition = GridSystem.instance.WorldToGrid(this.transform.position);
                GridMovement.instance.RemoveElementIfExists<Vector2>(other.GetComponent<ObjectToBePushed>().positionTraversed, thisPosition);
                Vector2 newPosition = other.GetComponent<ObjectToBePushed>().positionTraversed.Peek();

                other.GetComponent<ObjectToBePushed>().Target = GridSystem.instance._gridArray[(int)newPosition.x, (int)newPosition.y].transform.position;
                other.GetComponent<ObjectToBePushed>().IsShoot = true;
                CutsceneManager.instance.PlayAnimationForCharacter(this.gameObject, "Hurt");
                other.GetComponent<ObjectToBePushed>().positionTraversed.Clear();
            }

        }
        
    }
*/
    


}
