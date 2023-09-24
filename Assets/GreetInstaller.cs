using FileTransfer;
using Google.Protobuf;
using ProtoBuf;
using Reflex.Core;
using Unity.VisualScripting;
using UnityEngine;

public class GreetInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerDescriptor descriptor)
    {
       // descriptor.AddInstance("World");
       var testObject = new GetFilesRequest();

       testObject.ToByteArray();
       
       descriptor.AddSingleton(typeof(Greeter), typeof(IStartable)); // IStartable will force it to be constructed on container build
    }
}