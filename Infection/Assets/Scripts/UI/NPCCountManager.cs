using NPCs;
using TMPro;
using UnityEngine;

namespace UI
{
    public class NPCCountManager : MonoBehaviour
    {
        [Header("Text Field References")]
        [SerializeField] private TextMeshProUGUI infectedText;
        [SerializeField] private TextMeshProUGUI maskText;
        [SerializeField] private TextMeshProUGUI noMaskText;
        [SerializeField] private TextMeshProUGUI susceptibleText;
        [SerializeField] private TextMeshProUGUI vaccinatedText;
        
        [Header("Other References")]
        [SerializeField] private NPCSpawner spawner;

        private void Update()
        {
            infectedText.text = spawner.infectedCount.ToString();
            maskText.text = spawner.maskCount.ToString();
            noMaskText.text = spawner.noMaskCount.ToString();
            susceptibleText.text = spawner.susceptibleCount.ToString();
            vaccinatedText.text = spawner.vaccinatedCount.ToString();
        }
    }
}