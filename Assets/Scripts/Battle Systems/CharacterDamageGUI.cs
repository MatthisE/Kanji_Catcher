using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterDamageGUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] float lifeTime = 1f, moveSpeed = 1f, textVibration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, lifeTime); // destroy after life time
        transform.position += new Vector3(0f, moveSpeed * Time.deltaTime); // slowly move text up across y-axis
    }

    public void SetDamage(int damageAmount)
    {
        damageText.text = damageAmount.ToString();
        float jitterAmount = Random.Range(-textVibration, +textVibration);
        transform.position += new Vector3(jitterAmount, jitterAmount, 0f); // add jitter to text position
    }
}
