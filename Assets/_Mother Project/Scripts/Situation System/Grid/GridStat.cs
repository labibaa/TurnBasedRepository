using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.AI;
using Unity.AI.Navigation;
using TMPro;

public class GridStat : MonoBehaviour
{
    public int Visited = -1;
   
    public bool s=false;
    public bool IsWalkable = true;
    public bool IsOccupied = false;
    public bool HasOrb = false;
   
    public bool IsClimbable;
    public string OccupiedObject;
    public GameObject OccupiedGameObject;
    public Direction PressedKeyDirection = Direction.none;
    public int Height = -1;
    public Color storedGridColor;
    public TextMeshPro QText;
    public TextMeshPro RText;
    public TextMeshPro SText;
    public TextMeshPro Offset;
    public List<Vector2> neighborCoordinates;
    [Header("Direction - Transform Mapping")]
    [SerializeField] Transform upTransform;
    [SerializeField] Transform downTransform;
    [SerializeField] Transform upLeftTransform;
    [SerializeField] Transform upRightTransform;
    [SerializeField] Transform downLeftTransform;
    [SerializeField] Transform downRightTransform;

    public Dictionary<HexOrientation, Transform> directionTransformMap = new Dictionary<HexOrientation, Transform>();


    // this script is for every grid tile data

    private void Awake()
    {
        Visited = -1;
        IsWalkable = true;
        IsOccupied = false;
        HasOrb = false;
        Height = -1;
        storedGridColor = new Color(255, 255, 255, 255);

    }


    private void Start()
    {
        var originalScale = transform.localScale;
        
        //transform.localScale = Vector3.zero;
        //transform.DOScale(originalScale + new Vector3(0.2f, 0.2f, 0.2f), 0.5f).OnComplete(() =>
        //{
        //    transform.DOScale(originalScale, 0.2f);
        //});


        directionTransformMap.Add(HexOrientation.Up, upTransform);
        directionTransformMap.Add(HexOrientation.Down, downTransform);
        directionTransformMap.Add(HexOrientation.UpLeft, upLeftTransform);
        directionTransformMap.Add(HexOrientation.UpRight, upRightTransform);
        directionTransformMap.Add(HexOrientation.DownLeft, downLeftTransform);
        directionTransformMap.Add(HexOrientation.DownRight, downRightTransform);

        //GetComponent<Renderer>().material.color = new Color(0,0,0,0);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag) || other.CompareTag(StringData.OtherObject)|| other.CompareTag(StringData.Ally))
        {

            IsWalkable = false;
            IsOccupied = true;
            OccupiedObject = other.tag;
            OccupiedGameObject = other.gameObject;
           // OccupiedGameObject.transform.position = this.transform.position;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag) || other.CompareTag(StringData.OtherObject)|| other.CompareTag(StringData.Ally))
        {

            IsWalkable = false;
            IsOccupied = true;
            OccupiedObject = other.tag;
            OccupiedGameObject = other.gameObject;
          
        }
        
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(StringData.PlayerTag)|| other.CompareTag(StringData.OtherObject)|| other.CompareTag(StringData.Ally))
        {
            ClearGrid();
        }
    }

    public void ClearGrid()
    {
        IsWalkable = true;
        IsOccupied = false;
        OccupiedObject = null;
        OccupiedGameObject = null;
    }
    public void SetGridCellColor(Color color)
    {
        
        GetComponent<SpriteRenderer>().color =color;
    }
   
}
