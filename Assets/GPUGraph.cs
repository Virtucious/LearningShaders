using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GPUGraph : MonoBehaviour
{
    const int maxResolution = 1000;

    [SerializeField] ComputeShader computeShader;
    [SerializeField, Range(10, maxResolution)] private int resolution = 10;
    [SerializeField] private FunctionLibrary.FunctionName function;
    [SerializeField, Min(0f)] float functionDuration = 1f, transitionDuration = 1f;
    [SerializeField] Material material;
    [SerializeField] Mesh mesh;

    float duration;
    bool transitioning;
    FunctionLibrary.FunctionName transitionFunction;
    public enum TransitionMode {Cycle, Random}
    [SerializeField] TransitionMode transitionMode = TransitionMode.Cycle;

    static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        transitionProgressId = Shader.PropertyToID("_TransitionProgress");
    ComputeBuffer positionsBuffer;

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3*4);
    }

    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }

    private void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if(duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }
        UpdateFunctionOnGPU();
    }

    private void PickNextFunction()
    {
        function = transitionMode == TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunctionName(function) :
            FunctionLibrary.GetRandomFunctionNameOtherThan(function); 
    }

    private void UpdateFunctionOnGPU()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        if(transitioning)
        {
            computeShader.SetFloat(
                    transitionProgressId,
                    Mathf.SmoothStep(0f, 1f, duration / transitionDuration)
            );
        }
        var kernelIndex = (int)function + (int)(transitioning ? transitionFunction : function) * FunctionLibrary.FunctionCount;
        computeShader.SetBuffer(kernelIndex, positionsId, positionsBuffer);
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);
        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }
}
