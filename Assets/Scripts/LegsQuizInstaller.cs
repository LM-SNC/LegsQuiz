using Reflex.Core;
using UnityEngine;

public class LegsQuizInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private ButtonsHandler _buttonsHandler;

    public void InstallBindings(ContainerDescriptor descriptor)
    {
        descriptor.AddInstance(_buttonsHandler);
        descriptor.AddSingleton(typeof(LegsQuizApi), typeof(IStartable));
        //descriptor.AddSingleton(typeof(ButtonsHandler), typeof(IStartable));
    }
}