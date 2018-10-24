using UnityEditor; 
using UnityEditor.SceneManagement; 
using UnityEngine; 
 
 namespace SceneSearcher 
 { 
     public class SceneSearcherGeneratedClass : MonoBehaviour 
     { 
         [MenuItem("My cool game/Scenes/Test1Scene")] 
        public static void A636759995538801() 
        { 
             EditorSceneManager.OpenScene("Assets/SceneSearcher/ExapleScenes/first/Test1Scene.unity"); 
        } 
         [MenuItem("My cool game/Scenes/HelloWorld")] 
        public static void A636759995538802() 
        { 
             EditorSceneManager.OpenScene("Assets/SceneSearcher/ExapleScenes/second/HelloWorld.unity"); 
        } 
         [MenuItem("My cool game/Scenes/SecondSceneForTest")] 
        public static void A636759995538803() 
        { 
             EditorSceneManager.OpenScene("Assets/SceneSearcher/ExapleScenes/second/SecondSceneForTest.unity"); 
        } 
         [MenuItem("My cool game/Scenes/TestForSecondScene")] 
        public static void A636759995538804() 
        { 
             EditorSceneManager.OpenScene("Assets/SceneSearcher/ExapleScenes/second/TestForSecondScene.unity"); 
        } 
     } 
 }