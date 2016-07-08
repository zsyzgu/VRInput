using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LineGraphManager : MonoBehaviour {
    public GameObject linerenderer;
	public GameObject pointer;

	public GameObject pointerRed;
	public GameObject pointerBlue;

	public GameObject HolderPrefb;

	public GameObject holder;
	public GameObject xLineNumber;

	public Material bluemat;
	public Material greenmat;

	public Text topValue;

	public List<GraphData> graphDataPlayer1 = new List<GraphData>();
	public List<GraphData> graphDataPlayer2 = new List<GraphData>();

	private GraphData gd;
	private float highestValue = 28;

	public Transform origin;

	public TextMesh player1name;
	public TextMesh player2name;

    public TextMesh player1Rate;
    public TextMesh player2Rate;

	private float lrWidth = 0.1f;
	private int dataGap = 0;
    private bool viewing = false;
    public GameObject mainCamera;
    public GameObject congratulation;

    public void setLine(List<float> values) {
        graphDataPlayer1.Clear();
        for (int i = 0; i < values.Count; i++) {
            GraphData gd = new GraphData();
            gd.marbles = values[i];
            graphDataPlayer1.Add(gd);
        }
    }

    public void show() {
        viewing = true;
        mainCamera.transform.position = transform.position + new Vector3(5, 3, -10f);
        if (graphDataPlayer2.Count == 0) {
            getRecordRate();
        }
        ShowGraph();
        showRate();
    }

    void showRate() {
        float aveRate1 = 0f, maxRate1 = 0f, currRate1 = 0f;
        float aveRate2 = 0f, maxRate2 = 0f;

        for (int i = 0; i < graphDataPlayer1.Count; i++) {
            float value = graphDataPlayer1[i].marbles;
            aveRate1 += value;
            maxRate1 = Mathf.Max(maxRate1, value);
        }
        if (graphDataPlayer1.Count > 0) {
            aveRate1 /= graphDataPlayer1.Count;
        }
        currRate1 = graphDataPlayer1[graphDataPlayer1.Count - 1].marbles;

        for (int i = 0; i < graphDataPlayer2.Count; i++) {
            float value = graphDataPlayer2[i].marbles;
            aveRate2 += value;
            maxRate2 = Mathf.Max(maxRate2, value);
        }
        if (graphDataPlayer2.Count > 0) {
            aveRate2 /= graphDataPlayer2.Count;
        }

        player1Rate.text = "aveRate: " + aveRate1 + "\nmaxRate: " + maxRate1 + "\ncurrRate: " + currRate1;
        player2Rate.text = "aveRate: " + aveRate2 + "\nmaxRate: " + maxRate2;
    }

	void Start(){

    }

    void getRecordRate() {
        TextAsset textAsset = Resources.Load("setting") as TextAsset;

        string[] methods = textAsset.text.Split('\n');
        for (int i = 0; i < methods.Length; i++) {
            string[] values = methods[i].Split(' ');
            if (values[0] == Server.getMethod().ToString()) {
                player2name.text = "record (" + values[1] + ")";
                graphDataPlayer2.Clear();
                for (int j = 2; j < values.Length; j++) {
                    GraphData gd = new GraphData();
                    gd.marbles = int.Parse(values[j]);
                    graphDataPlayer2.Add(gd);
                }
                break;
            }
        }
    }

    void Update() {
        if (viewing) {
            if (Input.GetButtonUp("Fire1")) {
                if (graphDataPlayer1.Count < graphDataPlayer2.Count) {
                    mainCamera.transform.position = Vector3.zero;
                    viewing = false;
                } else {
                    congratulation.SetActive(true);
                }
            }
        }
    }
	
	private void ShowData(GraphData[] gdlist,int playerNum,float gap) {

		// Adjusting value to fit in graph
		for(int i = 0; i < gdlist.Length; i++)
		{
			// since Y axis is from 0 to 7 we are dividing the marbles with the highestValue
			// so that we get a value less than or equals to 1 and than we can multiply that
			// number with Y axis range to fit in graph. 
			// e.g. marbles = 90, highest = 90 so 90/90 = 1 and than 1*7 = 7 so for 90, Y = 7
			gdlist[i].marbles = (gdlist[i].marbles/highestValue)*7;
		}
		if(playerNum == 1) 
			StartCoroutine(BarGraphBlue(gdlist,gap));
		else if(playerNum == 2) 
			StartCoroutine(BarGraphGreen(gdlist,gap));
	}

    private void AddPlayer1Data(int numOfStones){
		GraphData gd = new GraphData();
		gd.marbles = numOfStones;
		graphDataPlayer1.Add(gd);
	}
    private void AddPlayer2Data(int numOfStones){
		GraphData gd = new GraphData();
		gd.marbles = numOfStones;
		graphDataPlayer2.Add(gd);
	}

    private void ShowGraph(){
		ClearGraph();

		if(graphDataPlayer1.Count >= 1 && graphDataPlayer2.Count >= 1){
			holder = Instantiate(HolderPrefb,transform.position,Quaternion.identity) as GameObject;
			holder.name = "h2";

			GraphData[] gd1 = new GraphData[graphDataPlayer1.Count];
			GraphData[] gd2 = new GraphData[graphDataPlayer2.Count];
			for(int i = 0; i < graphDataPlayer1.Count; i++){
				GraphData gd = new GraphData();
				gd.marbles = graphDataPlayer1[i].marbles;
				gd1[i] = gd;
			}
			for(int i = 0; i < graphDataPlayer2.Count; i++){
				GraphData gd = new GraphData();
				gd.marbles = graphDataPlayer2[i].marbles;
				gd2[i] = gd;
			}

			dataGap = GetDataGap(graphDataPlayer2.Count);


			int dataCount = 0;
			int gapLength = 1;
			float gap = 1.0f;
			bool flag = false;

			while(dataCount < graphDataPlayer2.Count)
			{
				if(dataGap > 1){

					if((dataCount+dataGap) == graphDataPlayer2.Count){

						dataCount+=dataGap-1;
						flag = true;
					}
					else if((dataCount+dataGap) > graphDataPlayer2.Count && !flag){

						dataCount =	graphDataPlayer2.Count-1;
						flag = true;
					}
					else{
						dataCount+=dataGap;
						if(dataCount == (graphDataPlayer2.Count-1))
							flag = true;
					}
				}
				else
					dataCount+=dataGap;

				gapLength++;
			}

			if(graphDataPlayer2.Count > 13)
			{
				if(graphDataPlayer2.Count < 40)
					gap = 13.0f/graphDataPlayer2.Count;
				else if(graphDataPlayer2.Count >= 40){
					gap = 13.0f/gapLength;
				}
			}

			ShowData(gd1,1,gap);
			ShowData(gd2,2,gap);
		}
	}

    private void ClearGraph(){
		if(holder)
			Destroy(holder);
	}

	int GetDataGap(int dataCount){
		int value = 1;
		int num = 0;
		while((dataCount-(40+num)) >= 0){
			value+= 1;
			num+= 20;
		}
		
		return value;
	}


	IEnumerator BarGraphBlue(GraphData[] gd,float gap)
	{
		float xIncrement = gap;
		int dataCount = 0;
		bool flag = false;
		Vector3 startpoint = new Vector3((origin.position.x+xIncrement),(origin.position.y+gd[dataCount].marbles),(origin.position.z));//origin.position;//

		while(dataCount < gd.Length)
		{
			
			Vector3 endpoint = new Vector3((origin.position.x+xIncrement),(origin.position.y+gd[dataCount].marbles),(origin.position.z));
			startpoint = new Vector3(startpoint.x,startpoint.y,origin.position.z);
			// pointer is an empty gameObject, i made a prefab of it and attach it in the inspector
			GameObject p = Instantiate(pointer, new Vector3(startpoint.x, startpoint.y, origin.position.z),Quaternion.identity) as GameObject;
			p.transform.parent = holder.transform;


			GameObject lineNumber = Instantiate(xLineNumber, new Vector3(origin.position.x+xIncrement, origin.position.y-0.18f , origin.position.z),Quaternion.identity) as GameObject;
			lineNumber.transform.parent = holder.transform;
			lineNumber.GetComponent<TextMesh>().text = (dataCount+1).ToString();


			// linerenderer is an empty gameObject with Line Renderer Component Attach to it, 
			// i made a prefab of it and attach it in the inspector
			GameObject lineObj = Instantiate(linerenderer,startpoint,Quaternion.identity) as GameObject;
			lineObj.transform.parent = holder.transform;
			lineObj.name = dataCount.ToString();
			
			LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
			
			lineRenderer.material = bluemat;
			lineRenderer.SetWidth(lrWidth, lrWidth);
			lineRenderer.SetVertexCount(2);

			while(Vector3.Distance(p.transform.position,endpoint) > 0.2f)
			{
				float step = 5 * Time.deltaTime;
				p.transform.position = Vector3.MoveTowards(p.transform.position, endpoint, step);
				lineRenderer.SetPosition(0, startpoint);
				lineRenderer.SetPosition(1, p.transform.position);
				
				yield return null;
			}
			
			lineRenderer.SetPosition(0, startpoint);
			lineRenderer.SetPosition(1, endpoint);
			
			
			p.transform.position = endpoint;
			GameObject pointered = Instantiate(pointerRed,endpoint,pointerRed.transform.rotation) as GameObject ;
			pointered.transform.parent = holder.transform;
			startpoint = endpoint;

			if(dataGap > 1){
				if((dataCount+dataGap) == gd.Length){
					dataCount+=dataGap-1;
					flag = true;
				}
				else if((dataCount+dataGap) > gd.Length && !flag){
					dataCount =	gd.Length-1;
					flag = true;
				}
				else{
					dataCount+=dataGap;
					if(dataCount == (gd.Length-1))
						flag = true;
				}
			}
			else
				dataCount+=dataGap;

			xIncrement+= gap;
			
			yield return null;
			
		}
	}

	IEnumerator BarGraphGreen(GraphData[] gd, float gap)
	{
		float xIncrement = gap;
		int dataCount = 0;
		bool flag = false;

		Vector3 startpoint = new Vector3((origin.position.x+xIncrement),(origin.position.y+gd[dataCount].marbles),(origin.position.z));
		while(dataCount < gd.Length)
		{
			
			Vector3 endpoint = new Vector3((origin.position.x+xIncrement),(origin.position.y+gd[dataCount].marbles),(origin.position.z));
			startpoint = new Vector3(startpoint.x,startpoint.y,origin.position.z);
			// pointer is an empty gameObject, i made a prefab of it and attach it in the inspector
			GameObject p = Instantiate(pointer, new Vector3(startpoint.x, startpoint.y, origin.position.z),Quaternion.identity) as GameObject;
			p.transform.parent = holder.transform;
			
			// linerenderer is an empty gameObject with Line Renderer Component Attach to it, 
			// i made a prefab of it and attach it in the inspector
			GameObject lineObj = Instantiate(linerenderer,startpoint,Quaternion.identity) as GameObject;
			lineObj.transform.parent = holder.transform;
			lineObj.name = dataCount.ToString();
			
			LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer>();
			
			lineRenderer.material = greenmat;
			lineRenderer.SetWidth(lrWidth, lrWidth);
			lineRenderer.SetVertexCount(2);

			while(Vector3.Distance(p.transform.position,endpoint) > 0.2f)
			{
				float step = 5 * Time.deltaTime;
				p.transform.position = Vector3.MoveTowards(p.transform.position, endpoint, step);
				lineRenderer.SetPosition(0, startpoint);
				lineRenderer.SetPosition(1, p.transform.position);
				
				yield return null;
			}
			
			lineRenderer.SetPosition(0, startpoint);
			lineRenderer.SetPosition(1, endpoint);
			
			
			p.transform.position = endpoint;
			GameObject pointerblue = Instantiate(pointerBlue,endpoint,pointerBlue.transform.rotation) as GameObject; 
			pointerblue.transform.parent = holder.transform;
			startpoint = endpoint;

			if(dataGap > 1){
				if((dataCount+dataGap) == gd.Length){
					dataCount+=dataGap-1;
					flag = true;
				}
				else if((dataCount+dataGap) > gd.Length && !flag){
					dataCount =	gd.Length-1;
					flag = true;
				}
				else{
					dataCount+=dataGap;
					if(dataCount == (gd.Length-1))
						flag = true;
				}
			}
			else
				dataCount+=dataGap;

			xIncrement+= gap;
			
			yield return null;
			
		}
	}



	public class GraphData
	{
		public float marbles;
	}
}
