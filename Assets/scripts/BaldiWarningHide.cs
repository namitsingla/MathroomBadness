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
            if (collecteddisplay.homework == 4 && !baldi.GetComponent<BaldiEnemy>().isEnraged)
            {
                messageText.text = "*strikes*";
                messageText.fontSize = 120;
                baldi_frown.enabled = true;

                if (!baldi.GetComponent<BaldiEnemy>().isEnraged)
                {
                    baldi.GetComponent<BaldiEnemy>().lookRadius = 1000f;
                    baldi.GetComponent<BaldiEnemy>().isEnraged = true;   
                }
            }
            else
            {
                int randoText = Random.Range(0, 9);

                switch (randoText)
                {
                 case 0:
                    messageText.text = "Baldi is fuming - that was his best doodle";
                    messageText.fontSize = 50;
                    baldi_talk.enabled = true;
                    break;

                case 1:
                    messageText.text = "“Another unfinished assignment.... you'll never finish them all!”";
                    messageText.fontSize = 40;
                    baldi_talk.enabled = true;
                    break;

                case 2:
                    messageText.text = "Baldi's slaps grow louder...";
                    messageText.fontSize = 50;
                    baldi_talk.enabled = true;
                    break; 

                case 3:
                    messageText.text = "I don’t like repeating myself… especially mistakes.";
                    messageText.fontSize = 50;
                    baldi_talk.enabled = true;
                    break;

                case 4:
                    messageText.text = "I was having a good day. You changed that.";
                    messageText.fontSize = 60;
                    baldi_talk.enabled = true;
                    break;

                case 5:
                    messageText.text = "I see we’re skipping the “learning” part today.";
                    messageText.fontSize = 60;
                    baldi_talk.enabled = true;
                    break;   

                case 6:
                    messageText.text = "Class dismissed… for you.";
                    messageText.fontSize = 60;
                    baldi_talk.enabled = true;
                    break; 

                case 7:
                    messageText.text = "That assignment had feelings, you know.";
                    messageText.fontSize = 60;
                    baldi_talk.enabled = true;
                    break;

                case 8:
                    messageText.text = "You’ll pay for that doodle.";
                    messageText.fontSize = 60;
                    baldi_talk.enabled = true;
                    break;  
                }
            }
        }
        else if (item.CompareTag("Chalk"))
        {
            if (collecteddisplay.chalk == 4 && !oggy.GetComponent<EnemyController>().isEnraged)
            {
                messageText.text = "*meow*";
                messageText.fontSize = 120;
                RectTransform rt = messageText.rectTransform;
                rt.anchoredPosition = new Vector2(0f, rt.anchoredPosition.y);

                if (!oggy.GetComponent<EnemyController>().isEnraged)
                {
                    oggy.GetComponent<EnemyController>().lookRadius = 500f;          
                    oggy.GetComponent<EnemyController>().isEnraged = true; 
                }
                

                //uiiacat.GetComponent<UIIAController>().lookRadius = 500f;
                //uiiacat.GetComponent<UIIAController>().ActivateAllWalls();

                if (!uiiacat.GetComponent<UIIAController>().isEnraged)
                {
                    uiiacat.GetComponent<UIIAController>().isEnraged = true;
                    uiiacat.GetComponent<UnityEngine.AI.NavMeshAgent>().speed *= 1.5f;
                    uiiacat.GetComponent<UIIAController>().wallLifetime *= 1.5f;   
                }
            }
            else
            {
                int randoText = Random.Range(0, 7);

                switch (randoText)
                {
                    case 0:
                        messageText.text = "One less chalk stick for Baldi! His slaps grow louder…";
                        messageText.fontSize = 45;
                        baldi_rotate.enabled =true;
                        break;

                    case 1:
                        messageText.text = "Snap! Baldi can’t write now, but he sure can chase.";
                        messageText.fontSize = 50; 
                        baldi_rotate.enabled =true; 
                        break;

                    case 2:
                        messageText.text = "The chalk is gone. The lesson continues.";
                        messageText.fontSize = 50; 
                        baldi_rotate.enabled =true;   
                        break;

                    case 3:
                        messageText.text = "No chalk left to teach… guess I’ll demonstrate instead.";
                        messageText.fontSize = 50; 
                        baldi_rotate.enabled =true;   
                        break;

                    case 4:
                        messageText.text = "I expected better. I shouldn’t have.";
                        messageText.fontSize = 45; 
                        baldi_rotate.enabled =true;   
                        break;

                    case 5:
                        messageText.text = "Running won’t improve your grade.";
                        messageText.fontSize = 45; 
                        baldi_rotate.enabled =true;   
                        break;

                    case 6:
                        messageText.text = "Creative work deserves respect. You showed none.";
                        messageText.fontSize = 50; 
                        baldi_rotate.enabled =true;   
                        break;
                }
            }
        }

    }
}
