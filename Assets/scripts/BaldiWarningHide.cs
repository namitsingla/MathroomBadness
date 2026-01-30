using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BaldiWarningHide : MonoBehaviour
{
    public collectedisplay collecteddisplay;
    public TextMeshProUGUI messageText;
    public GameObject baldi;
    public GameObject uiiacat;
    public GameObject oggy;
    public Image baldi_frown;
    public Image baldi_talk;
    public Image baldi_rotate;
    
    public void ShowWarning ()
    {
        gameObject.SetActive(true);
        Invoke("HideBaldiWarning", 5f);
    }

    public void HideBaldiWarning()
    {
        baldi_frown.enabled = false;
        baldi_talk.enabled = false;
        baldi_rotate.enabled = false;
        gameObject.SetActive(false);   // hide the canvas
        RectTransform rt = messageText.rectTransform;
        rt.anchoredPosition = new Vector2(170f, rt.anchoredPosition.y);

    }

    public void WarningNumber(GameObject item)
    {
        //Debug.Log(item.name);

        if (item.CompareTag("Homework"))
        {
            if (collecteddisplay.homework == 1)
            {
                messageText.text = "Baldi is fuming - that was his best doodle";
                messageText.fontSize = 50;
                baldi_talk.enabled = true;
            }

            if (collecteddisplay.homework == 2)
            {
                messageText.text = "“Another unfinished assignment.... you'll never finish them all!”";
                messageText.fontSize = 40;
                baldi_talk.enabled = true;
            }

            if (collecteddisplay.homework == 3)
            {
                messageText.text = "*strikes*";
                messageText.fontSize = 120;

                baldi.GetComponent<BaldiEnemy>().lookRadius = 500f;
                baldi.GetComponent<BaldiEnemy>().isEnraged = true;
                baldi_frown.enabled = true;
            }
        }
        else if (item.CompareTag("Chalk"))
        {
            if (collecteddisplay.chalk == 1)
            {
                 messageText.text = "One less chalk stick for Baldi! His slaps grow louder…";
                 messageText.fontSize = 45;
                 baldi_rotate.enabled =true;
            }

            if (collecteddisplay.chalk == 2)
            {
                messageText.text = "Snap! Baldi can’t write now, but he sure can chase.";
                messageText.fontSize = 50; 
                baldi_rotate.enabled =true;          
            }

            if (collecteddisplay.chalk == 3)
            {
                messageText.text = "*meow*";
                messageText.fontSize = 120;
                RectTransform rt = messageText.rectTransform;
                rt.anchoredPosition = new Vector2(0f, rt.anchoredPosition.y);


                oggy.GetComponent<EnemyController>().lookRadius = 500f;          
                oggy.GetComponent<EnemyController>().isEnraged = true; 

                //uiiacat.GetComponent<UIIAController>().lookRadius = 500f;
                //uiiacat.GetComponent<UIIAController>().isEnraged = true;
                //uiiacat.GetComponent<UIIAController>().ActivateAllWalls();
                uiiacat.GetComponent<UnityEngine.AI.NavMeshAgent>().speed *= 1.5f;
                uiiacat.GetComponent<UIIAController>().wallLifetime *= 1.5f;
            }
        }

    }
}
