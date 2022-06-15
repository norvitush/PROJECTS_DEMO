using UnityEngine;
using UnityEngine.UI;


namespace VOrb
{
    public class FonCellView : FancyScrollViewCell<FonCellData, FonScrollViewContext>
    {

        [SerializeField]
        Animator animator = null;

        [SerializeField]
        Image image_fon = null;
     
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
        public override void UpdateContent(FonCellData itemData)
        {

            if (Context != null)
            {
                var isSelected = Context.SelectedIndex == DataIndex;
            


                Transform BaseCont = GetComponentInChildren<Image>().transform;
                
                Image pict = BaseCont.Find("content_img").GetComponent<Image>();
                pict.sprite = itemData.fon_sprite;
                if (itemData.fon.Id> CollectingSystem.Instance.RateProgress(true))
                {                    
                    pict.color = new Color32(255, 255, 255, 150);
                    image_fon.color = isSelected
                    ? new Color32(198, 198, 198, 30)
                    : new Color32(255, 255, 255, 30);
                }
                else
                {                    
                    pict.color = new Color32(255, 255, 255, 255);
                    image_fon.color = isSelected
                    ? new Color32(198, 198, 198, 255)
                    : new Color32(255, 255, 255, 100);    
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


        float currentPosition = 0;
        void OnEnable()
        {
            UpdatePosition(currentPosition);
        }
    }
}
