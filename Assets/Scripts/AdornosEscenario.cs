using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class AdornosEscenario : MonoBehaviour {
    public GameObject canvas;
    public List<SpriteRenderer> renderers;

    // Start is called before the first frame update
    void Start() {
        renderers = GetComponentsInChildren<SpriteRenderer>().ToList();
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnMouseDown() {

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut() {
        canvas.SetActive(false);
        yield return new WaitForSeconds(1f);
        List<Color> colors = new();

        foreach (var c in renderers) {
            colors.Add(c.color);
        }

        for (int i = 10; i >= 0; i--) {
            yield return new WaitForSeconds(0.075f);
            for (int j = 0; j < renderers.Count; j++) {
                Color c = colors[j];
                c.a = i / 10f;
                renderers[j].color = c;
            }
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}