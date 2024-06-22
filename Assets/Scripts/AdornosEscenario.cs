using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using TMPro;
using UnityEngine.UI;
using System;

public class AdornosEscenario : MonoBehaviour {
    public GameObject canvas;
    public bool quitando;
    public bool eliminando = false;
    public float tiempo = 0f;
    public float tiempoMax = 10f;
    public BuildingControl.CostoRecurso recurso;
    public int costo = 0;
    public TextMeshProUGUI textTiempo;
    public TextMeshProUGUI textGemas;
    public Slider sliderActu;
    public List<SpriteRenderer> renderers;

    private GameObject botonEliminar;

    // Start is called before the first frame update
    void Start() {
        textGemas.gameObject.SetActive(false);
        renderers = GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    // Update is called once per frame
    void Update() {
        if (eliminando) return;

        textTiempo.gameObject.SetActive(quitando);
        sliderActu.gameObject.SetActive(quitando);

        if (!quitando) {
            tiempo = 0;
            return;
        }

        sliderActu.gameObject.SetActive(true);

        tiempo += Time.deltaTime * GameManager.Instance.Velocidad;

        textTiempo.text = BuildingControl.GetRemainingTimeFormatted(tiempo, tiempoMax);

        sliderActu.value = tiempo / tiempoMax;

        if (tiempo < tiempoMax) return;

        quitando = false;
        eliminando = true;
        StartCoroutine(FadeOut());
    }

    private void OnMouseDown() {
        //quitando = true;
        GameManager.Instance.EliminarObstaculo(this);
    }

    IEnumerator FadeOut() {
        sliderActu.gameObject.SetActive(false);
        int gema = GameManager.Instance.GetRandomInt(-5, 10);
        textGemas.gameObject.SetActive(gema > 0);
        textGemas.text = $"Gano {gema} gema{(gema > 1 ? ("s") : (""))}";
        
        if (gema > 0)
            GameManager.Instance.gems += gema;

        yield return new WaitForSeconds(1f);
        List<Color> colors = new();

        foreach (var c in renderers) {
            colors.Add(c.color);
        }

        for (int i = 10; i >= 0; i--) {
            yield return new WaitForSeconds(0.075f / GameManager.Instance.Velocidad);
            for (int j = 0; j < renderers.Count; j++) {
                Color c = colors[j];
                c.a = i / 10f;
                renderers[j].color = c;
            }
        }
        botonEliminar.SetActive(false);
        yield return new WaitForSeconds(0.5f / GameManager.Instance.Velocidad);
        Destroy(gameObject);
    }

    internal void Eliminar(GameObject boton) {
        quitando = true;
        if (recurso == BuildingControl.CostoRecurso.oro)
            GameManager.Instance.oro -= costo;
        else if (recurso == BuildingControl.CostoRecurso.elixir)
            GameManager.Instance.elixir -= costo;
        else
            GameManager.Instance.gems -= costo;
    }
}