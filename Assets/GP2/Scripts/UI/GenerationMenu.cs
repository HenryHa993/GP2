using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GenerationMenu : MonoBehaviour
{
    public DungeonGenerator generator;

    // Panel
    private VisualElement RootElement;
    private VisualElement PanelElement;
    
    // Random Walk
    private SliderInt RWIterations;
    private SliderInt RWSteps;
    private Slider RWRoomRatio;
    private Toggle RWRandomStart;
    
    // Corridor Walk
    private SliderInt CWIterations;
    private SliderInt CWSteps;
    private Toggle CWDeadEnds;

    private Button GenerateButton;

    private bool IsPanelHidden = false;

    private void Awake()
    {
        RootElement = GetComponent<UIDocument>().rootVisualElement;
        PanelElement = RootElement.Q<VisualElement>("Panel");
        
        RWIterations = RootElement.Q<SliderInt>("RWIterations");
        RWSteps = RootElement.Q<SliderInt>("RWSteps");
        RWRoomRatio = RootElement.Q<Slider>("RWRoomRatio");
        RWRandomStart = RootElement.Q<Toggle>("RWRandomStart");
        
        CWIterations = RootElement.Q<SliderInt>("CWIterations");
        CWSteps = RootElement.Q<SliderInt>("CWSteps");
        CWDeadEnds = RootElement.Q<Toggle>("CWDeadEnds");

        GenerateButton = RootElement.Q<Button>("GenerateButton");
        
        SetupMenu();

        RWIterations.RegisterValueChangedCallback((evt) => generator.Iterations = evt.newValue);
        RWSteps.RegisterValueChangedCallback((evt) => generator.StepsPerIteration = evt.newValue);
        RWRoomRatio.RegisterValueChangedCallback((evt) => generator.RoomPercent = evt.newValue);
        RWRandomStart.RegisterValueChangedCallback((evt) => generator.RandomStart = evt.newValue);

        CWIterations.RegisterValueChangedCallback((evt) => generator.CorridorIterations = evt.newValue);
        CWSteps.RegisterValueChangedCallback((evt) => generator.CorridorLength = evt.newValue);
        CWDeadEnds.RegisterValueChangedCallback((evt) => generator.DeadEnds = evt.newValue);
        GenerateButton.clicked += Generate;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsPanelHidden)
            {
                PanelElement.RemoveFromClassList("hidden");
            }
            else
            {
                PanelElement.AddToClassList("hidden");
            }

            IsPanelHidden = !IsPanelHidden;
        }
    }

    public void SetupMenu()
    {
        RWIterations.value = 20;
        RWSteps.value = 10;
        RWRoomRatio.value = 0.5f;
        
        CWIterations.value = 15;
        CWSteps.value = 15;
    }

    private void Generate()
    {
        generator.Generate();
    }
}
