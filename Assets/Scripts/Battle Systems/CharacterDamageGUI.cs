using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// visual numbers appearing above characters when they take damage
// instantiated by BattleManager at position of attacked character
public class CharacterDamageGUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI damageText;
    [SerializeField] float lifeTime = 1f, moveSpeed = 1f, textVibration = 0.5f;

    void Update()
    {
        Destroy(gameObject, lifeTime); // destroy after life time
        transform.position += new Vector3(0f, moveSpeed * Time.deltaTime); // slowly move text up across y-axis
    }

    public void SetDamage(int damageAmount)
    {
        // set damage text and jitter amount
        damageText.text = damageAmount.ToString();
        float jitterAmount = Random.Range(-textVibration, +textVibration);
        transform.position += new Vector3(jitterAmount, jitterAmount, 0f); // add jitter to text position
    }
}
