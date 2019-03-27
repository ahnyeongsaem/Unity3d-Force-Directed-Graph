using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VolumetricLines;

public class Graphvisual : MonoBehaviour
{

    public int n = 80; //임시 갯수
    public int otu = 10; 
    static public float[,] matrix;
    static public List<Node> nodearray;
    static public List<Edge> edgearray;
    public List<KeyValuePair<int, float>> subEdgetoVector; //index 

    public class Edge
    {
        int index;
        public int startNodeIndex;
        public int endNodeIndex;
        public float value;

        public Edge(int _index, int _startNodeIndex, int _endNodeIndex, float _value)
        {
            index = _index;
            startNodeIndex = _startNodeIndex;
            endNodeIndex = _endNodeIndex;
            value = _value;
        }

    };

    public class Node
    {
        int index;
        public Vector3 position;
        public Color mycolor;
        public List<int> edgeindex;

        public Node(int _index, float max_position)
        {
            index = _index;
            position = new Vector3(Random.Range(0, max_position),
                Random.Range(0, max_position),
                Random.Range(0, max_position));
            edgeindex = new List<int>();
            mycolor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
    };


    static int Compare1(KeyValuePair<int, float> a, KeyValuePair<int, float> b)
    {
        return Mathf.Abs(b.Value).CompareTo(Mathf.Abs(a.Value));
    }

    public void InitSortnode()
    {

        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (matrix[j, i] > 0)
                {
                    edgearray.Add(new Edge(edgearray.Count, i, j, matrix[i, j]));
                    nodearray[i].edgeindex.Add(edgearray.Count - 1);
                }
            }
        }

    }



    public void Sortnode()
    {
        subEdgetoVector = new List<KeyValuePair<int, float>>();
        for (int i = 0; i < edgearray.Count; i++)
        {
            //TODO : Need Optimization
            subEdgetoVector.Add(new KeyValuePair<int, float>(
                i, edgearray[i].value - Vector3.Distance(nodearray[edgearray[i].startNodeIndex].position, nodearray[edgearray[i].endNodeIndex].position)));

        }
        subEdgetoVector.Sort(Compare1);

        int ekey = subEdgetoVector[0].Key;
        int snode = edgearray[ekey].startNodeIndex;

        Vector3 dot = nodearray[snode].position - nodearray[edgearray[ekey].endNodeIndex].position;


        dot.Normalize();
        nodearray[snode].position
            += dot * subEdgetoVector[0].Value / otu;

    }

    public void VisualizationInit()
    {
        for (int i = 0; i < nodearray.Count; i++)
        {
            GameObject tmp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tmp.transform.localScale *= 0.2f;
            tmp.name = "node" + i;
            tmp.GetComponent<MeshRenderer>().material.color = nodearray[i].mycolor;
            tmp.AddComponent<Light>().type = LightType.Point;
            tmp.GetComponent<Light>().color = nodearray[i].mycolor;

            //VolumetricLineBehavior volumetricLineBehavior = tmp.AddComponent<VolumetricLineBehavior>();
            //volumetricLineBehavior.StartPos(0, nodearray[edgearray[nodearray[i].edgeindex[j]].endNodeIndex].position);
            tmp.transform.position = nodearray[i].position;
            
            for (int j = 0; j < nodearray[i].edgeindex.Count; j++)
            {
                GameObject tmp2 = new GameObject();
                tmp2.name = "edge" + i + "to" + j;


                
                LineRenderer lineRenderer = tmp2.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                
                lineRenderer.widthMultiplier = 0.1f;
                lineRenderer.endColor = nodearray[i].mycolor;
                lineRenderer.startColor = nodearray[edgearray[nodearray[i].edgeindex[j]].endNodeIndex].mycolor;
                //lineRenderer.SetPosition(0, nodearray[i].position);
                lineRenderer.SetPosition(0, nodearray[edgearray[nodearray[i].edgeindex[j]].endNodeIndex].position);
                lineRenderer.SetPosition(1, nodearray[edgearray[nodearray[i].edgeindex[j]].startNodeIndex].position);
                


            }

        }
    }

    public void Visualization()
    {
        for (int i = 0; i < nodearray.Count; i++)
        {
            GameObject tmp = GameObject.Find("node" + i);
            tmp.transform.position = nodearray[i].position;

            for (int j = 0; j < nodearray[i].edgeindex.Count; j++)
            {
                GameObject tmp2 = GameObject.Find("edge" + i + "to" + j);
                LineRenderer lineRenderer = tmp2.GetComponent<LineRenderer>();
                //lineRenderer.SetPosition(0, nodearray[i].position);
                lineRenderer.SetPosition(0, nodearray[edgearray[nodearray[i].edgeindex[j]].endNodeIndex].position);
                lineRenderer.SetPosition(1, nodearray[edgearray[nodearray[i].edgeindex[j]].startNodeIndex].position);
            }

        }

    }

    // Use this for initialization
    void Start()
    {

        matrix = new float[n, n];
        //initialized matrix array
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] = -1;
            }
        }
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < i; j++)
            {
                matrix[i, j] = matrix[j, i] = Random.Range(-100.0f, 100.0f);

            }
        }

        nodearray = new List<Node>();
        edgearray = new List<Edge>();
        for (int i = 0; i < n; i++)
        {
            nodearray.Add(new Node(i, 50));
        }
        InitSortnode();
        Sortnode();
        VisualizationInit();


    }
    // Update is called once per frame
    void Update()
    {

        Sortnode();
        Visualization();

    }


}
