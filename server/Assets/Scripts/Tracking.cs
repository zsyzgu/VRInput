using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tracking : MonoBehaviour {
    bool isDrawing = false;

    private LineRenderer lineRenderer;
    private int rendererCnt = 0;
    
    void Start () {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.SetVertexCount(rendererCnt);
    }
	
	void Update () {
        RectTransform rect = GetComponentInParent<RectTransform>();
        lineRenderer.SetWidth(0.1f * rect.localScale.x, 0.1f * rect.localScale.y);
    }

    void OnDestroy() {
        clearCanvas();
    }

    public void clearCanvas() {
        rendererCnt = 0;
        if (lineRenderer != null) {
            lineRenderer.SetVertexCount(rendererCnt);
        }
    }


    public void addPos(float x, float y) {
        RectTransform rect = GetComponentInParent<RectTransform>();
        float width = rect.rect.width * rect.localScale.x;
        float height = rect.rect.height * rect.localScale.y;
        if (lineRenderer != null) {
            lineRenderer.SetVertexCount(++rendererCnt);
            lineRenderer.SetPosition(rendererCnt - 1, new Vector3((x - 0.5f) * width, (y - 0.5f) * height, rect.transform.position.z - 0.01f));
        }
    }

    public void keepDrawing() {
        isDrawing = true;
    }

    public bool stopDrawing() {
        if (isDrawing) {
            isDrawing = false;
            clearCanvas();
            return true;
        }
        return false;
    }
}
