using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BuildingControl;

public class BotonEdificios : MonoBehaviour {
    public Sprite oro;
    public Sprite elixir;
    public Sprite gema;
    public Image icono;
    public TextMeshProUGUI costo;
    public TextMeshProUGUI cantidad;
    
   public void ChangeIcono(CostoRecurso recurso) {
        icono.sprite = recurso == CostoRecurso.oro ? oro : (recurso == CostoRecurso.elixir ? elixir : gema);
    }
}