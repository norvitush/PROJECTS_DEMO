using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using VOrb;

public class Debuger : Singlton<Debuger>
{
    private Camera _cam;
    private bool _updateShow = false;
    
    public string curText = "";
    public Text layout = null;
    object curObject = null;
    float Timer = 0f;
    public bool ShowMouseonGUI = false;

 

    void ShowmouseonGUI()
    {
        Vector3 point = new Vector3();
        Event currentEvent = Event.current;
        Vector2 mousePos = new Vector2();
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = _cam.pixelHeight - currentEvent.mousePosition.y;
        point = _cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, _cam.nearClipPlane));
        layout.text = "Screen pixels: " + _cam.pixelWidth + ":" + _cam.pixelHeight + "\n";
        layout.text += "Mouse position: " +"\n"+ mousePos + "\n";
        layout.text += "World position: " +"\n"+ point.ToString("F3") + "\n";
    }
    void OnGUI()
    {
        if (ShowMouseonGUI)
        {
            ShowmouseonGUI();
        }
    }

    void Start()
    {
        _cam = Camera.main;
        if (layout == null)
        {
            layout = FindObjectOfType<Text>();
        }

    }

    public static void layoutThis<T>(T forOut)
    {
        Instance._updateShow = false;
        try
        {
            Instance.curText = forOut.ToString();
            Instance.layout.text = Instance.curText;
        }
        catch
        {
            Debug.Log("layout error");
        }
    }
    public static void AddToLayout<T>(T forOut)
    {      
        try
        {
            Instance.curText = forOut.ToString();
            Instance.layout.text += Instance.curText + ",   ";           
        }
        catch
        {
            Debug.Log("layout error");
        }
    }
    public static void ShowSomeTime(float time, object objForShow)
    {
        Instance.Timer = time;
        Instance.curObject = objForShow;
        Instance.StartCoroutine(Instance.ShowWhileTimerTrue(Instance._updateShow));

    }
    private IEnumerator ShowWhileTimerTrue(bool upd)
    {
        if(upd) Instance._updateShow =false;
        bool isok = false;
        try
        {
            if (Instance.layout != null) Instance.layout.text = "";
            isok = true;
        }
        catch (System.Exception)
        {
            Debug.Log("layout error");
        }
        while ((Instance.Timer >= 0) && isok)
        {
            Debug.Log(Instance.curObject.ToString() + "  |" + Time.time);
            Instance.layout.text = Instance.curObject.ToString() +", "+ Instance.Timer.ToString()+"s.";
            yield return new WaitForSeconds(1);
            Instance.Timer -= 1;        
        }
        Instance.Timer = 0f;
        Instance.curObject = null;
        Instance.layout.text = Instance.curText = "";
        Instance._updateShow = upd;
    }
    public static void ShowWhileFrames(float fixedUpdateFramesNum, object objForShow)
    {

        Instance.StartCoroutine(Instance.ShowWhileFr(fixedUpdateFramesNum, objForShow, Instance._updateShow));
    }
    private IEnumerator ShowWhileFr(float frames, object obj, bool upd)
    {
        if (upd) Instance._updateShow = false;
        bool isok = false;
        try
        {
            if (Instance.layout != null) Instance.layout.text = obj.ToString();
            isok = true;
        }
        catch (System.Exception)
        {
            Debug.Log("layout error");           
        }
      
        float fr = frames;
            while ((fr >= 0)&&isok)
            {
                Debug.Log(obj.ToString() + "  |" + fr.ToString());
                Instance.layout.text = obj.ToString();
                yield return new WaitForFixedUpdate();
                fr -= 1;
            }
            Instance.layout.text = "";
        Instance._updateShow = upd;
    }

    public static void ShowType<T>(T obj)
    {
        Debug.Log("*Debuger* ShowType<T> Pivot");
        Instance._updateShow = false;
        try
        {
                Debug.Log(obj.GetType().ToString() + "  |" + Time.time);
                Instance.layout.text = obj.GetType().ToString();
        }
        catch
        {
            Debug.Log("layout error");
        }
    }
    public static void StartUpdateOut(object obj)
    {
        Instance.curObject = obj;
        Instance._updateShow = true;
        Debug.Log("*Debuger* START UpdatePivot");
    }
    public static void StopUpdateOut()
    {
        Instance._updateShow = false;
        Debug.Log("*Debuger* STOP UpdatePivot");
    }
    void Update()
    {
        if (Instance._updateShow)
        {
            try
            {
                Instance.curText = Instance.curObject.ToString();
                Debug.Log(Instance.curText);
                Instance.layout.text = Instance.curText + Time.time;
            }
            catch
            {
                Debug.Log("layout error");
            }
        }
    }
}
