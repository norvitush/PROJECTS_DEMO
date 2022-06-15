using UnityEngine;
using UnityEngine.UI;


namespace VOrb
{
    public class AvatarCellView : FancyScrollViewCell<SkinCellData, AvatarScrollViewContext>
    {

        [SerializeField]
        Animator animator = null;

        [SerializeField]
        Image image_fon = null;
        [SerializeField]
        Image image_av = null;
     
        [SerializeField]
        Button ImgButton = null;



        static readonly int scrollTriggerHash = Animator.StringToHash("scroll");

        void Start()
        {
            ImgButton.onClick.AddListener(OnPressedCell);
        }

        /// <summary>
        /// Updates the content.
        /// </summary>
        /// <param name="itemData">Item data.</param>
        public override void UpdateContent(SkinCellData itemData)
        {
            
            


            if (Context != null)
            {
                var isSelected = Context.SelectedIndex == DataIndex;
                image_fon.color = isSelected
                    ? new Color32(0, 255, 255, 0)
                    : new Color32(255, 255, 255, 30);
                image_av.sprite = itemData.av_sprite;

                
                Transform BaseCont = GetComponentInChildren<Image>().transform;
                Image glow = BaseCont.Find("glow_img").GetComponent<Image>();
                Image pict = BaseCont.Find("content_img").GetComponent<Image>();

                if (itemData.skinInfo.Data.Enabled !=1)
                {
                    glow.sprite = itemData.glow_sprite;
                    glow.color = new Color32(255, 255, 255, 255);
                    pict.color = new Color32(0, 0, 0, 230);
                }
                else
                {
                    glow.color = new Color32(255, 255, 255, 0);
                    pict.color = new Color32(255, 255, 255, 255);
                }

            }
        }

        /// <summary>
        /// Updates the position.
        /// </summary>
        /// <param name="position">Position.</param>
        public override void UpdatePosition(float position)
        {
            currentPosition = position;
            if(gameObject.activeInHierarchy)
            {
                animator.Play(scrollTriggerHash, -1, position);                
            }
            animator.speed = 0;
        }

        void OnPressedCell()
        {
            if (Context != null)
            {
                Context.OnPressedCell(this);
            }
        }

        // GameObject が非アクティブになると Animator がリセットされてしまうため
        // 現在位置を保持しておいて OnEnable のタイミングで現在位置を再設定します
        float currentPosition = 0;
        void OnEnable()
        {
            UpdatePosition(currentPosition);
        }
    }
}
