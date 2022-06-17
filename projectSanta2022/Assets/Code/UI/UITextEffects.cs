using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using VOrb;
using UnityEngine.UI;
namespace VOrb.SantaJam
{


    public class UITextEffects : Singlton<UITextEffects>
    {
       
        [SerializeField] private UIAnimator _animator;
        public static UIAnimator animator => Instance._animator;
        [SerializeField]private GameObject ScaleBox;        

        public static void SplashMainScreen(string sp_Text, TextEffectPreset preset, bool needToSplit = false)
        {
            
            TextMeshProUGUI SplashText = Instance.GetObject(preset.PoolObjectType, preset.RandomRotation);
            if (SplashText == null)
            {
                return;
            }
            SplashText.text = needToSplit ? Instance.GetSplitedLines(sp_Text) : sp_Text;
                if (preset.Colored)
                {
                    SplashText.color = preset.TxtColor;
                }
                if (preset.Movable)
                {
                    Vector3 txt_position;
                    var mn = UIWindowsManager.GetWindow<MainWindow>();
                    RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)mn.UIPanel.transform, preset.ScreenPosition, Camera.main, out txt_position);
                    SplashText.transform.position = txt_position;
                }
                if (preset.Scaleble)
                {
                    Instance.ScaleBox.transform.localScale = new Vector3(preset.Scale, preset.Scale, preset.Scale);
                    SplashText.gameObject.transform.SetParent(Instance.ScaleBox.transform);
                    SplashText.gameObject.SetActive(true);

                    Instance.Invoke(3.5f,() =>
                    {
                        if (Instance.ScaleBox.transform.childCount == 0)
                        {
                            Instance.ScaleBox.transform.localScale = Vector3.one;
                        }
                    }               
                );
            }
            else
            {
               
                SplashText.gameObject.SetActive(true);
            }
                
            

        }
       

        private string GetSplitedLines(string baseText)
        {
            string output = "";
            string[] strLines = new string[10];

            strLines = baseText.Split(' ');
            output = strLines[0] + " \n ";

            for (int i = 1; i < strLines.Length; i++)
            {
                output += strLines[i] + " ";
            }
            return output;
        }

        private TextMeshProUGUI GetObject( PooledObjectType objFromPool = PooledObjectType.NumberPopup1_smile,bool randomizedRotation = false)
        {
            TextMeshProUGUI txtMesh;
            GameObject SplashObj = ObjectPoolManager.Instance.GetPooledGameObject(objFromPool);

            if (SplashObj == null)
            {
                return null;
            }
            txtMesh = SplashObj.GetComponentInChildren<TextMeshProUGUI>();
            if (randomizedRotation)
            {
                float randRotation = UnityEngine.Random.Range(-45, 45);
                txtMesh.transform.RotateAround(SplashObj.transform.position, SplashObj.transform.forward,randRotation);
            }

            return txtMesh;
        }

    }

    

    public class TextEffectPreset
    {
        protected bool scaleble = false;
        protected bool movable = false;
        protected bool randomRotation = false;
        protected bool colored = false;
        protected PooledObjectType _poolObjectType;

        public float Scale = 1;
        public Vector2 ScreenPosition;        
        public Color32 TxtColor = new Color32(255,255,255,255);

        public PooledObjectType PoolObjectType { get => _poolObjectType; }
        public bool Scaleble { get => scaleble; }
        public bool Movable { get => movable; }
        public bool RandomRotation { get => randomRotation; }
        public bool Colored { get => colored;  }

        public TextEffectPreset(TextEffectBuilder builder) 
        {
            scaleble = builder.Scaleble;
            movable = builder.Movable;
            randomRotation = builder.RandomRotation;
            colored = builder.Colored;
            TxtColor = builder.TxtColor;
            Scale = builder.Scale;
            ScreenPosition = builder.ScreenPosition;
            _poolObjectType = builder.PoolObjectType;
        }
        public TextEffectPreset() { }
    }

    public class TextEffectBuilder: TextEffectPreset
    {
        public TextEffectBuilder MakeScaleble(float scale)
        {
            scaleble = true;
            Scale = scale;
            return this;
        }
        public TextEffectBuilder MakeColored(Color32 color)
        {
            colored = true;
            TxtColor = color;
            return this;
        }
        public TextEffectBuilder MakeMovable()
        {
            movable = true;
            return this;
        }
        public TextEffectBuilder MakeRandomRotateble()
        {
            randomRotation = true;            
            return this;
        }
        public TextEffectPreset Biuld(Vector2 screenPosition, PooledObjectType type)
        {
            ScreenPosition = screenPosition;
            _poolObjectType = type;
            return new TextEffectPreset(this);
        }
    }

}