using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class CantidadPorNivel {
    public int nivel;
    public int cantidad;
}

public class GameManager : MonoBehaviour {
    // Singleton instance
    public static GameManager Instance { get; private set; }

    public BuildingPlacement placement;

    public TMP_InputField inputSemilla;
    public TextMeshProUGUI textVelocidad;

    // Variables públicas
    public float _velocidad = 1f;
    public float Velocidad { get { return _velocidad; } set { textVelocidad.text = $"Velocidad x{value}"; _velocidad = value; } }
    public int semilla { get; set; } = 123;
    public int oro;
    public int maxOro;
    public int elixir;
    public int maxElixir;
    public int gems;
    public int numTropas;
    public int nivelAldea;

    public List<EdificiosPorNivel> edificiosPorNivel = new();

    public System.Random random;

    [System.Serializable]
    public class EdificiosPorNivel {
        public BuildingControl.TipoEdificios tipoEdificios;
        public BuildingControl prefab;

        public BuildingPlacement placement;

        public Button botonConstruccion;

        public List<CantidadPorNivel> cantidadesPorNiveles = new();
        public List<BuildingControl> edificios = new();

        public void CargarEdificios() {
            edificios.Clear();
            foreach (var item in FindObjectsOfType<BuildingControl>()) {
                if (item.tipoEdificios != tipoEdificios) continue;
                BuildingControl b = item;
                edificios.Add(b);
                Debug.Log($"{b.name} {b.tipoEdificios}");
            }
        }

        public void CargarBoton(int nivel) {
            if (!botonConstruccion) return;

            //ActualizarBotonConstruccion(nivel);
            int i = nivel;
            botonConstruccion.onClick.AddListener(delegate {
                placement.SetBuildingToPlace(prefab.gameObject);
                CargarEdificios();
                RestarRecurso(i);
                //ActualizarBotonConstruccion(i);
            });
        }

        public void RestarRecurso(int nivel) {
            if (prefab.costoRecursoActualizacion == BuildingControl.CostoRecurso.oro)
                GameManager.Instance.oro -= prefab.datosUnidad.nivelList[nivel].costoConstruccion;
            else if (prefab.costoRecursoActualizacion == BuildingControl.CostoRecurso.elixir)
                GameManager.Instance.elixir -= prefab.datosUnidad.nivelList[nivel].costoConstruccion;
            else
                GameManager.Instance.gems -= prefab.datosUnidad.nivelList[nivel].costoConstruccion;
        }

        public void ActualizarBotonConstruccion(int nivel) {
            if (!botonConstruccion) return;

            CantidadPorNivel c = null;
            try {
                c = cantidadesPorNiveles.Where(n => n.nivel == nivel).First();
            } catch {
                return;
            }

            if (c == null) return;

            bool recursoNecesario = false;

            if (prefab.costoRecursoActualizacion == BuildingControl.CostoRecurso.oro)
                recursoNecesario = GameManager.Instance.oro >= prefab.datosUnidad.nivelList[nivel].costoConstruccion;
            else if (prefab.costoRecursoActualizacion == BuildingControl.CostoRecurso.elixir)
                recursoNecesario = GameManager.Instance.elixir >= prefab.datosUnidad.nivelList[nivel].costoConstruccion;
            else
                recursoNecesario = GameManager.Instance.gems >= prefab.datosUnidad.nivelList[nivel].costoConstruccion;

            botonConstruccion.gameObject.SetActive(c.cantidad > 0);
            botonConstruccion.interactable = edificios.Count < c.cantidad && recursoNecesario;

            BotonEdificios b = botonConstruccion.GetComponent<BotonEdificios>();
            b.ChangeIcono(prefab.costoRecursoActualizacion);
            b.cantidad.text = $"{edificios.Count}/{c.cantidad}";
            b.costo.text = prefab.datosUnidad.nivelList[nivel].costoConstruccion.ToString();
        }
    }

    // Constructor para inicializar las variables (puedes agregar más si lo necesitas)
    void Awake() {
        if (PlayerPrefs.HasKey("semilla"))
            semilla = PlayerPrefs.GetInt("semilla");

        SetSemilla("" + semilla);
        inputSemilla.text = "" + semilla;

        // Configurar el Singleton
        if (Instance == null) {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        ActualizarListaEdificios();
    }

    void Update() {
        foreach (var item in edificiosPorNivel) {
            item.ActualizarBotonConstruccion(nivelAldea);
        }   
    }

    public void ActualizarListaEdificios() {
        foreach (EdificiosPorNivel item in edificiosPorNivel) {
            item.placement = placement;
            item.CargarEdificios();
            item.CargarBoton(1);
        }
    }

    public void SetSemilla(string value) {
        semilla = int.Parse(value);
        random = new System.Random(semilla);
    }

    public void Reiniciar() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public int GetRandomInt(int min, int max) {
        return random.Next(min, max);
    }

    // Método para obtener un número flotante aleatorio
    public float GetRandomFloat(float min, float max) {
        return (float)(random.NextDouble() * (max - min) + min);
    }
}