using Reflex.Core;
using UnityEngine;

public class GreetInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerDescriptor descriptor)
    {
       // descriptor.AddInstance("World");
        descriptor.AddSingleton(typeof(Greeter), typeof(IStartable)); // IStartable will force it to be constructed on container build
    }
}