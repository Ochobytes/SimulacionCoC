using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CantidadPorNivel {
    public int nivel;
    public int cantidad;
}

public class GameManager : MonoBehaviour {
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Variables públicas
    public int oro;
    public int maxOro;
    public int elixir;
    public int maxElixir;
    public int gems;
    public int numTropas;
    public int nivelAldea;

    public List<EdificiosPorNivel> edificiosPorNivel = new();

    [System.Serializable]
    public class EdificiosPorNivel {
        public BuildingControl.TipoEdificios tipoEdificios;
        public List<CantidadPorNivel> cantidadesPorNiveles = new();
        public List<BuildingControl> edificios = new();

        public void CargarEdificios() {
            edificios = FindObjectsOfType<BuildingControl>().Where(b => b.tipoEdificios == tipoEdificios).ToList();
        }
    }

    // Diccionario para la cantidad máxima de cada tipo de edificio por nivel de aldea
    public Dictionary<string, int> maxBuildingsPerLevel;

    // Constructor para inicializar las variables (puedes agregar más si lo necesitas)
    void Awake() {
        // Configurar el Singleton
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    void Update() {

    }
}
