using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class DatosNivel {
    public int nivelNecesario;
    public float capacidadMax;
    public Sprite sprite;
    public float tiempoActualizacion;
}

[System.Serializable]
public  class DatosUnidad {
    public int nivel;
    public float capacidadActual;
    public float capacidadMax;
    public List<DatosNivel> nivelList;
}

public class BuildingControl : MonoBehaviour {
    public BuildingPlacement buildingPlacement;

    public bool construido = false;
    public bool actualizando = false;
    public bool moviendo = false;

    public float tiempo;
    public int nivelASubir;

    public Color ok;
    public Color fail;

    public SortingGroup sortingGroup;
    public Collider colliderModelo;
    public Animator animator;
    public GameObject cuerdas;
    public GameObject destruido;

    public Sprite normal;
    public Sprite tierra;

    public SpriteRenderer suelo;
    public SpriteRenderer main;
    public Color color;

    public DatosUnidad datosUnidad = new();

    private bool isValid = true;

    private void Awake() {
        if (!buildingPlacement)
            buildingPlacement = FindObjectOfType<BuildingPlacement>();

        if (suelo && normal) {
            suelo.sprite = normal;
            color = suelo.color;
            suelo.color = ok;
        }

        if (colliderModelo)
            colliderModelo.isTrigger = true;

        if (animator) {
            animator.enabled = false;
            animator.SetInteger("nivel", 0);
            animator.SetFloat("cap", 0);
        }

        if (cuerdas)
            cuerdas.SetActive(false);

        if (destruido)
            destruido.SetActive(false);

    }

    // Start is called before the first frame update
    void Start() {
        if (construido)
            Construir(datosUnidad.nivel);
        else
            buildingPlacement.modoMover = moviendo = true;
    }

    // Update is called once per frame
    void Update() {
        cuerdas.SetActive(actualizando);
        colliderModelo.isTrigger = moviendo;

        if (!actualizando) {
            tiempo = 0;
            return;
        }

        tiempo += Time.deltaTime * 1f;

        if (tiempo < datosUnidad.nivelList[nivelASubir].tiempoActualizacion) return;

        actualizando = false;

        SubirNivel(nivelASubir);
    }

    public void OnTriggerStay(Collider other) {
        if (moviendo && other.tag == "edificio") {
            suelo.color = fail;
            buildingPlacement.isValid = isValid = false;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (moviendo && other.tag == "edificio") {
            suelo.color = ok;
            buildingPlacement.isValid = isValid = true;
        }
    }

    internal void Construir(int nivelUp = 0) {
        suelo.sprite = normal;
        suelo.color = color;

        if (animator)
            animator.enabled = true;

        SubirNivel(nivelUp);

        if (!construido) {
            construido = true;
            Actualizar(1);
        }
    }

    public void Actualizar(int nivelUp) {
        actualizando = true;
        nivelASubir = nivelUp;
    }

    public void SubirNivel(int nivel) {
        if (nivel >= datosUnidad.nivelList.Count) return;

        DatosNivel dn = datosUnidad.nivelList[nivel];

        if(dn.sprite != null)
            main.sprite = dn.sprite;
    
        if(animator) {
            animator.enabled = true;
            animator.SetInteger("nivel", nivel);
        }

        datosUnidad.nivel = nivel;
        datosUnidad.capacidadMax = dn.capacidadMax;
    }

    private void OnMouseDown() {
        if (!buildingPlacement || !isValid) return;

        buildingPlacement.modoMover = moviendo = !moviendo;
        buildingPlacement.buildingToPlace = gameObject;

        if (!moviendo) {
            suelo.sprite = normal;
            suelo.color = color;
            sortingGroup.sortingOrder = 0;
        } else {
            sortingGroup.sortingOrder = 5;
            GameObject g = Instantiate(suelo.gameObject, suelo.gameObject.transform.position, suelo.gameObject.transform.rotation);
            g.transform.localScale = suelo.transform.lossyScale;
            SpriteRenderer s = g.GetComponent<SpriteRenderer>();
            s.sprite = tierra;
            s.sortingOrder = 0;
            g.SetActive(true);
            StartCoroutine(TierraFade(s));
        }
    }

    IEnumerator TierraFade(SpriteRenderer s) {
        yield return new WaitForSeconds(1.5f);
        Color c = s.color;
        for (int i = 10; i >= 0; i--) {
            yield return new WaitForSeconds(0.075f);
            c.a = i / 10f;
            s.color = c;
        }
        Destroy(s.gameObject);
    }
}