using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.IO;

public class SceneViewCapture : MonoBehaviour
{
    [MenuItem("Tools/Capture SceneView")]
    public static void CaptureSceneView(){
        SceneView sceneView = SceneView.lastActiveSceneView;
        if(sceneView == null){
            Debug.LogWarning("SceneView가 활성화되어 있지 않습니다.");
            return;
        }

        Camera sceneCamera = sceneView.camera;
        if (sceneCamera == null){
            Debug.LogWarning("SceneView 카메라를 찾을 수 없습니다.");
            return;
        }

        int width = 1920;
        int height = 1080;
        RenderTexture rt = new RenderTexture(width, height, 24);
        sceneCamera.targetTexture = rt;

        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        sceneCamera.Render();

        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0,0,width,height),0,0);
        screenshot.Apply();

        byte[] bytes = screenshot.EncodeToPNG();
        string filePath = Path.Combine(Application.dataPath,"SceneViewScreenshot.png");
        File.WriteAllBytes(filePath, bytes);

        sceneCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        Debug.Log("SceneView 캡처 완료: " + filePath);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
