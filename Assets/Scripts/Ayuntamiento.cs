using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Recursos {
    public int oro;
}

public class Ayuntamiento : MonoBehaviour {
    public GameObject objeto;
    public Transform objetoTransform;
    public Recursos recursos;

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        Random.InitState(3);
        float valor = Random.value;
    }
}