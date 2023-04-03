#if UNITY_EDITOR
using System.Threading.Tasks;
using UnityEditor;
#endif
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using VolumeBox.Toolbox.UIInformer;

namespace VolumeBox.Toolbox
{   
    public class ENTRY : MonoBehaviour
    {
        [Foldout("Time and Framerate")] 
        [SerializeField] [Range(0, 1)] 
        private float timeScale;
        
        [Foldout("Time and Framerate")] 
        [SerializeField]
        [Range(1, 900)]
        private int targetFrameRate;
        
        [Scene]
        [Foldout("Initial Scene")]
        [SerializeField] 
        private string initialSceneName;
        [Foldout("Initial Scene")]
        [SerializeField] 
        private SceneArgs initialSceneArgs;
        [SerializeField, Foldout("Initial Scene")]
        private bool manualFadeOut;
        [SerializeField, HideIf(nameof(manualFadeOut))]
        private float fadeOutDuration;
        
        [ReadOnly] public bool Autocompile;
        public UnityEvent onLoadEvent;
        
        private AudioPlayer audioPlayer;
        private Resolver resolver;
        private Messager messager;
        private Traveler traveler;
        private Updater updater;
        private Pooler pooler;
        private Saver saver;
        private Info info; 

        private void OnValidate() 
        {
            if(updater != null)
            {
                Updater.TimeScale = timeScale;
            }          
        }
        
#if UNITY_EDITOR
        [Button("Switch Autocompile")]
        public void SwitchAutocompile()
        {
            Autocompile = !Autocompile;
            EditorPrefs.SetBool("kAutoRefresh", Autocompile);
        }
#endif

        private async void Awake()
        {
            #if UNITY_EDITOR
            while (!EditorPlayStateHandler.EditorReady)
            {
                await Task.Yield();
            }
            #endif
            
            Application.targetFrameRate = targetFrameRate;

            resolver = GetComponent<Resolver>();
            
            audioPlayer = GetComponent<AudioPlayer>();
            messager = GetComponent<Messager>();
            traveler = GetComponent<Traveler>();
            updater = GetComponent<Updater>();
            pooler = GetComponent<Pooler>();
            saver = GetComponent<Saver>();
            info = GetComponent<Info>();


            resolver.Run();
            resolver.AddInstance(resolver);

            resolver.AddInstance(audioPlayer);
            resolver.AddInstance(messager);
            resolver.AddInstance(traveler);
            resolver.AddInstance(updater);
            resolver.AddInstance(pooler);
            resolver.AddInstance(saver);
            resolver.AddInstance(info);

            resolver.InjectInstances();

            audioPlayer.Run();
            messager.Run();
            traveler.Run();
            updater.Run();
            pooler.Run();
            saver.Run();
            info.Run();

            updater.InitializeObjects(SceneManager.GetActiveScene().GetRootGameObjects());

            if(!string.IsNullOrEmpty(initialSceneName) || initialSceneName != "MAIN")
            {
                await Traveler.LoadScene(initialSceneName, initialSceneArgs);

                if(!manualFadeOut)
                {
                    await Fader.Out(fadeOutDuration);
                }
            }
        }

        private void Start() 
        {
            onLoadEvent?.Invoke();
        }
    }
}
