using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CreateButtonForTesting : MonoBehaviour
{
    [SerializeField] private GameObject ButtonPrefab;
    [SerializeField] private TMP_InputField TextInput;
    [SerializeField] private GameObject Destination;

    public void CreateButton()
    {
        GameObject button = Instantiate(ButtonPrefab, Destination.transform);
        button.GetComponentInChildren<TMP_Text>().text = TextInput.text;
    }
}
