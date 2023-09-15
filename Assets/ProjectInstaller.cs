using Reflex.Core;
using UnityEngine;

public class ProjectInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerDescriptor descriptor)
    {
        Debug.Log("HUI");
        descriptor.AddInstance("Hello");
    }
}