using Reflex.Core;
using UnityEngine;

public class LegsQuizInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private ButtonsHandler _buttonsHandler;
    [SerializeField] private CanvasSwitcher _canvasSwitcher;
    [SerializeField] private BackgroundsSwitcher _backgroundsSwitcher;
    [SerializeField] private CanvasDataManager _canvasDataManager;
    [SerializeField] private GameManager _gameManager;

    public void InstallBindings(ContainerDescriptor descriptor)
    {
        descriptor.AddInstance(_buttonsHandler);
        descriptor.AddInstance(_canvasSwitcher);
        descriptor.AddInstance(_backgroundsSwitcher);
        descriptor.AddInstance(_canvasDataManager);
        descriptor.AddInstance(_gameManager);
        descriptor.AddInstance(new TranslationsManager());

        //descriptor.AddSingleton(typeof(ButtonsHandler), typeof(IStartable));
    }
}