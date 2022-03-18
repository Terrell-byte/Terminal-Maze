using System;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands;
using TMPro;
using UnityEngine;
using UnityEditor;

public class MapObject
{
    [SerializeField] private GameObject _mapElement;
    [SerializeField] private Vector3 _elementOffset;
    [SerializeField] private Color _objectType;
    public GameObject MapElement
    {
        get => _mapElement;
        set => _mapElement = value;
    }

    public Vector3 ElementOffset
    {
        get => _elementOffset;
        set => _elementOffset = value;
    }

    public Color ObjectType
    {
        get => _objectType;
        set => _objectType = value;
    }
}

public class MapBuilder : EditorWindow
{
    //VARIABLES
    private Texture2D _mapSprite;
    private List<MapObject> _gameObjectList = new List<MapObject>();
    private const string GenerateLvlBtn = "Generate Level";
    List<Color> colors = new List<Color>();
    Vector3 elementYOffset;

    //EDITOR LABELS
    [MenuItem("Tool/MapBuilder")]
    public static void ShowWindow()
    {
        GetWindow<MapBuilder>("MapBuilder");
    }

    //ENABLE GUI ELEMENTS
    private void OnGUI()
    {
        
        Widget_MapSpriteField();
        Button_GenerateObjectFields();
        var distinctColors = colors.Distinct();
        var index = 0;
        foreach (var color in distinctColors)
        {
            _gameObjectList.Add(new MapObject());
            Widget_Element(color,"Color", _gameObjectList[index]);
            _gameObjectList[index].ObjectType = color;
            index++;
            
        }
        Button_GenerateLevel();
    }
    
    private void Widget_MapSpriteField()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Map Texture");
        _mapSprite = (Texture2D)EditorGUILayout.ObjectField(_mapSprite, typeof(Texture2D), false);
        EditorGUILayout.EndHorizontal();
    }

    
    private void Widget_Element(Color color, string label, MapObject element)
    {
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.BeginHorizontal();
        GUILayout.Box(new GUIContent(BuildBoxTexture(color,new Vector2Int(128,12))));
        element.MapElement = (GameObject)EditorGUILayout.ObjectField(label, element.MapElement, typeof(GameObject),false);
        EditorGUILayout.EndHorizontal();
        element.ElementOffset = EditorGUILayout.Vector3Field("Offset",element.ElementOffset);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    
    private void Button_GenerateLevel()
    {
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(GenerateLvlBtn))
        {
            GenerateLevel();
        }
    }

    //Generates a level based on each individual pixel on the given _mapSprite png file
    private void GenerateLevel()
    {

        if (GameObject.Find("LevelObjects"))
        {
            DestroyImmediate(GameObject.Find("LevelObjects"));
        }
        
        var levelObjects = new GameObject("LevelObjects");
        var pix = _mapSprite.GetPixels();
        var levelX = _mapSprite.width;
        var levelZ = _mapSprite.height;
        var spawnPositons = new Vector3[pix.Length];
        var startingSpawnPosition = new Vector3(0, 0, 0);
        var currentSpawnPosisiton = startingSpawnPosition;
        var counter = 0;


        //This code is a for loop converts the Texture2D to a Vector3 Array called spawnPositions. The spawn positions are being stored in an array
        //called "spawnPositions". The counter variable is used to keep track of the index of the spawnPosition array.
        for (int z = 0; z < levelZ; z++)
        {
            for (int x = 0; x < levelX; x++)
            {
                spawnPositons[counter] = currentSpawnPosisiton;
                counter++;
                currentSpawnPosisiton.x++;
            }

            currentSpawnPosisiton.x = startingSpawnPosition.x;
            currentSpawnPosisiton.z++;
        }
        counter = 0;
 
        //Instantiates an object based on the color value in spawnPosition array
        foreach (var pos in spawnPositons)
        {
            Color c = pix[counter];
            var distinctColors = colors.Distinct();
            
            foreach (var mapObject in _gameObjectList)
            {
                if (mapObject.ObjectType == c)
                {
                    Instantiate(mapObject.MapElement, pos + mapObject.ElementOffset, Quaternion.identity, levelObjects.transform);
                }
            }
            
            counter++;
        }
    }

    void Button_GenerateObjectFields()
    {
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Generate Object Fields"))
        {
            
            CountColors();
        }
    }
    public void CountColors()
    {
        colors.Clear();
        for (int x = 0; x < _mapSprite.width; x++) {
            for (int y = 0; y < _mapSprite.height; y++) {
                colors.Add(_mapSprite.GetPixel(x, y));
            }
        }
    }
    
    public Texture2D BuildBoxTexture(Color color, Vector2Int TextureSize)
    {
        var tex = new Texture2D(TextureSize.x, TextureSize.y);

        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                tex.SetPixel(x,y, color);
            }
        }
        tex.Apply();
        return tex;
    }
}
