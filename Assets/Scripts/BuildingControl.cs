using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class DatosNivel {
    public int nivelNecesario;
    public float capacidadMax;
    public Sprite sprite;
    public float tiempoActualizacion;
    public int costoConstruccion;
    public int vida;
    public int capacidadOro;
    public int capacidadElixir;
}

[System.Serializable]
public  class DatosUnidad {
    public int nivel;
    public float capacidadActual;
    public float capacidadMax;
    public List<DatosNivel> nivelList;
}

public class BuildingControl : MonoBehaviour {
    public enum TipoEdificios {
        Ayuntamiento,
        Cuartel,
        MinaDeOro,
        AlmacenDeOro,
        RecolectorDeElixir,
        AlmacenDeElixir,
        Campamento,
        TorreDeArqueras,
        Cañón,
        //Mortero,
        //TorreTesla,
        CastilloDelClan,
        Laboratorio,
        Choza
    }

    public enum CostoRecurso { oro, elixir, gema }

    public BuildingPlacement buildingPlacement;

    public TipoEdificios tipoEdificios;
    public CostoRecurso costoRecursoActualizacion;

    public bool construido = false;
    public bool actualizando = false;
    public bool moviendo = false;

    public float tiempo;
    public int nivelASubir;

    public Color ok;
    public Color fail;
    [Space(20)]
    public TextMeshProUGUI textNivel;
    public TextMeshProUGUI textTiempo;
    public Slider sliderActu;
    public Slider sliderVida;
    [Space(15)]
    public GameObject modelo3D;
    public GameObject cuerdas;
    public GameObject destruido;
    public SpriteRenderer suelo;
    public SpriteRenderer main;
    [Space(20)]

    public SortingGroup sortingGroup;
    public Collider colliderModelo;
    public Animator animator;

    public Sprite normal;
    public Sprite tierra;

    public Color color;

    public DatosUnidad datosUnidad = new();

    private bool isValid = true;

    public static string GetRemainingTimeFormatted(float tiempo, float tiempoActualizacion) {
        float remainingTime = tiempoActualizacion - tiempo;

        if (remainingTime <= 0) {
            return "0s";
        }

        TimeSpan timeSpan = TimeSpan.FromSeconds(remainingTime);

        int days = timeSpan.Days;
        int hours = timeSpan.Hours;
        int minutes = timeSpan.Minutes;
        int seconds = timeSpan.Seconds;

        if (days > 0)
            return $"{days}D {hours}h";

        if (hours > 0)
            return $"{hours}h {minutes}M";

        if (minutes > 0)
            return $"{minutes}M {seconds}s";

        return $"{seconds}s";
    }

    private void Awake() {
        if (!buildingPlacement)
            buildingPlacement = FindObjectOfType<BuildingPlacement>();

        if (!sortingGroup)
            sortingGroup = GetComponent<SortingGroup>();

        if (!colliderModelo)
            colliderModelo = GetComponent<Collider>();

        if (!animator)
            animator = GetComponent<Animator>();

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
        textNivel.gameObject.SetActive(false);
        sliderVida.gameObject.SetActive(false);

        if (construido)
            Construir(datosUnidad.nivel);
        else
            buildingPlacement.modoMover = moviendo = true;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(1))
            OnMouseRight();

            cuerdas.SetActive(actualizando);
        colliderModelo.isTrigger = moviendo;
        textNivel.text = $"Nivel {datosUnidad.nivel}";

        textTiempo.gameObject.SetActive(actualizando);
        sliderActu.gameObject.SetActive(actualizando);

        if (!actualizando) {
            tiempo = 0;
            return;
        }

        sliderActu.gameObject.SetActive(true);

        tiempo += Time.deltaTime * 1f;

        textTiempo.text = GetRemainingTimeFormatted(tiempo, datosUnidad.nivelList[nivelASubir].tiempoActualizacion);

        sliderActu.value = tiempo / datosUnidad.nivelList[nivelASubir].tiempoActualizacion;

        if (tiempo < datosUnidad.nivelList[nivelASubir].tiempoActualizacion) return;

        actualizando = false;

        SubirNivel(nivelASubir);

        sliderActu.gameObject.SetActive(false);
    }

    public void OnMouseRight() {
        // Crear un rayo desde la posición del ratón en la cámara
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Verificar si el rayo impacta en el objeto
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.gameObject == gameObject) {
                OnRightMouseDown();
            }
        }
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

        if (dn.sprite != null)
            main.sprite = dn.sprite;

        if (animator) {
            animator.enabled = true;
            animator.SetInteger("nivel", nivel);
        }

        datosUnidad.nivel = nivel;
        datosUnidad.capacidadMax = dn.capacidadMax;
    }

    private void OnRightMouseDown() {

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

    private void OnMouseEnter() {
        textNivel.gameObject.SetActive(datosUnidad.nivel > 0);
    }

    private void OnMouseExit() {
        textNivel.gameObject.SetActive(false);
    }
}