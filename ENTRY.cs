using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace VolumeBox.Toolbox
{   
    public class ENTRY : MonoBehaviour
    {
        [SerializeField] [Range(0, 1)] private float timeScale;
        [SerializeField] private string initialSceneName;
        public UnityEvent onLoadEvent;
        private Resolver resolver;
        private Messager messager;
        private Traveler traveler;
        private Updater updater;
        private Pooler pooler;
        private Saver saver;

        private void OnValidate() 
        {
            if(updater != null)
            {
                updater.TimeScale = timeScale;
            }          
        }

        private void Awake()
        {
            resolver = GetComponent<Resolver>();
            messager = GetComponent<Messager>();
            traveler = GetComponent<Traveler>();
            updater = GetComponent<Updater>();
            pooler = GetComponent<Pooler>();
            saver = GetComponent<Saver>();

            resolver.Run();

            resolver.AddInstance(resolver);
            resolver.AddInstance(messager);
            resolver.AddInstance(traveler);
            resolver.AddInstance(updater);
            resolver.AddInstance(pooler);
            resolver.AddInstance(saver);

            resolver.InjectInstances();

            messager.Run();
            traveler.Run();
            updater.Run();
            pooler.Run();
            saver.Run();

            updater.InitializeObjects(SceneManager.GetActiveScene().GetRootGameObjects());

            if(!string.IsNullOrEmpty(initialSceneName))
            {
                traveler.LoadScene(initialSceneName);
            }
        }

        private void Start() 
        {
            onLoadEvent?.Invoke();
        }
    }
}
