using UnityEngine;
using UnityEngine.Rendering;

public class BuildingPlacement : MonoBehaviour {
    public bool isValid = true;
    public bool modoMover;

    public GameObject buildingToPlace;
    public LayerMask groundLayer;
    public GameObject buildingPrefab;

    void Update() {
        // Detectar clic izquierdo del ratón para colocar el edificio
        if (!modoMover && isValid && Input.GetMouseButtonDown(0) && buildingToPlace != null) {
            PlaceBuilding();
        }

        // Mover el edificio con el ratón si hay uno seleccionado
        if (buildingToPlace != null) {
            MoveBuildingWithMouse();
        }
    }

    // Función para definir el objeto a instanciar y crear la instancia
    public void SetBuildingToPlace(GameObject prefab) {
        if (buildingToPlace != null) {
            Destroy(buildingToPlace);
        }
        buildingPrefab = prefab;
        buildingToPlace = Instantiate(buildingPrefab);

        if (buildingToPlace.GetComponent<SortingGroup>() != null) {
            buildingToPlace.GetComponent<SortingGroup>().sortingOrder = 5;
        }

        modoMover = false;
    }

    // Mover el edificio con el ratón y alinearlo con los tiles
    void MoveBuildingWithMouse() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer)) {
            buildingToPlace.transform.position = hit.collider.transform.position;
        }
    }

    // Función para colocar el edificio
    void PlaceBuilding() {
        if (buildingToPlace.GetComponent<SortingGroup>() != null) {
            buildingToPlace.GetComponent<SortingGroup>().sortingOrder = 0;
        }

        if (!buildingToPlace.GetComponent<BuildingControl>().construido)
            buildingToPlace.GetComponent<BuildingControl>().Construir();

        Debug.Log("PlaceBuilding");

        // Soltar el edificio
        buildingToPlace = null;
    }
}
