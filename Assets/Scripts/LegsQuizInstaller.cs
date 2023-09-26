using System.Collections;
using System.Collections.Generic;
using Reflex.Core;
using UnityEngine;

public class LegsQuizInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private CanvasManager _canvasManager;

    public void InstallBindings(ContainerDescriptor descriptor)
    {
        descriptor.AddInstance(_canvasManager);
        descriptor.AddSingleton(typeof(LegsQuizApi), typeof(IStartable));
    }
}