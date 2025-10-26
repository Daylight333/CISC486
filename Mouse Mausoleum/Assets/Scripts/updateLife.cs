using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class updateLife : MonoBehaviour {
    
    public TMP_Text livesText;
    public GameObject mouseObj;

    // Update is called once per frame
    void Update() {

        if (mouseObj != null){
            livesText.text = "Lives: " + mouseObj.GetComponent<Health>().getHealth();
        }
        else{
            livesText.text = "Lives: 0";
        }
        
        
    }
}
