using Reflex.Core;
using UnityEngine;

public class LegsQuizInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private ButtonsHandler _buttonsHandler;
    [SerializeField] private LoadingProgressBar _progressBar;
    [SerializeField] private CanvasSwitcher _canvasSwitcher;

    public void InstallBindings(ContainerDescriptor descriptor)
    {
        descriptor.AddInstance(_buttonsHandler);
        descriptor.AddInstance(_progressBar);
        descriptor.AddInstance(_canvasSwitcher);

        descriptor.AddSingleton(typeof(LegsQuizApi));
        //descriptor.AddSingleton(typeof(ButtonsHandler), typeof(IStartable));
    }
}