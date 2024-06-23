using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenerarRecurso : MonoBehaviour {
    public BuildingControl.CostoRecurso recursoGenerar;
    public Animator animator;
    public float tiempo = 0;
    public List<float> recursoPorHora = new();

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        float tMax = (3600f / recursoPorHora[GameManager.Instance.nivelAldea]);

        if (animator)
            animator.SetFloat("cap", tiempo / tMax);

        if (recursoGenerar == BuildingControl.CostoRecurso.oro && GameManager.Instance.oro >= GameManager.Instance.maxOro) return;

        if (recursoGenerar == BuildingControl.CostoRecurso.elixir && GameManager.Instance.elixir >= GameManager.Instance.maxElixir) return;

        tiempo += Time.deltaTime * GameManager.Instance.Velocidad;

        if (tiempo <= tMax) return;

        if (recursoGenerar == BuildingControl.CostoRecurso.oro) {
            if (GameManager.Instance.oro + (int)GameManager.Instance.Velocidad > GameManager.Instance.maxOro)
                GameManager.Instance.oro = GameManager.Instance.maxOro;
            else
                GameManager.Instance.oro += (int)GameManager.Instance.Velocidad;
        }

        if (recursoGenerar == BuildingControl.CostoRecurso.elixir)
            if (GameManager.Instance.elixir + (int)GameManager.Instance.Velocidad > GameManager.Instance.maxElixir)
                GameManager.Instance.elixir = GameManager.Instance.maxElixir;
            else
                GameManager.Instance.elixir += (int)GameManager.Instance.Velocidad;

        tiempo = 0f;
    }
}