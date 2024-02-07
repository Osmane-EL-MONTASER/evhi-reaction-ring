using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WindowGraph : MonoBehaviour
{
    public RectTransform graphContainer;
    public Sprite pointSprite;
    public RectTransform labelTemplateX;
    public RectTransform labelTemplateY;

    public int additionalpoints = 5;
    public Slider perfAlphaValue;
    public int maxX = 3;

    private List<GameObject> gameObjectList;
    private void Awake()
    {
        graphContainer = transform.Find("Graph Container").GetComponent<RectTransform>();
    }

    private GameObject CreatePoint(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("point", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = pointSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(10, 10);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    public void ShowGraph(List<float> valueList)
    {
        float graphWidth = graphContainer.sizeDelta.x;
        float graphHeight = graphContainer.sizeDelta.y;

        GameObject lastPoint = null;
        for (int i = 0; i < valueList.Count; i++)
        {
            float xPosition = i * (graphWidth/valueList.Count);
            float yPosition = valueList[i] * graphHeight;
            GameObject point = CreatePoint(new Vector2(xPosition, yPosition));
            gameObjectList.Add(point);
            if (lastPoint != null)
            {
                CreateDotConnection(lastPoint.GetComponent<RectTransform>().anchoredPosition, point.GetComponent<RectTransform>().anchoredPosition);
            }
            lastPoint = point;

            RectTransform labelX = Instantiate(labelTemplateX);
            gameObjectList.Add(labelX.gameObject);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -7f);
            labelX.anchorMin = new Vector2(0, 0);
            labelX.anchorMax = new Vector2(0, 0);
            labelX.GetComponent<TMP_Text>().text = (i*(perfAlphaValue.value/(valueList.Count-1-additionalpoints))).ToString("F2");
        }
        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++)
        {
            RectTransform labelY = Instantiate(labelTemplateY);
            gameObjectList.Add(labelY.gameObject);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-15f, normalizedValue * graphHeight);
            labelY.anchorMin = new Vector2(0, 0);
            labelY.anchorMax = new Vector2(0, 0);
            labelY.GetComponent<TMP_Text>().text = normalizedValue.ToString();
        }
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObjectList.Add(gameObject);
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    public void ClearGraph()
    {
        if (gameObjectList == null)
        {
            gameObjectList = new List<GameObject>();
        }
        else
        {
            foreach (GameObject gameObject in gameObjectList)
            {
                Destroy(gameObject);
            }
        }
    }
}
