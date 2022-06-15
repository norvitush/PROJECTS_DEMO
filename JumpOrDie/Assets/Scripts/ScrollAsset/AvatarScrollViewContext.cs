using System;

namespace VOrb
{
    public class AvatarScrollViewContext
    {
        int selectedIndex = -1;

        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                if (value == selectedIndex)
                {
                    return;
                }

                selectedIndex = value;

                OnSelectedIndexChanged?.Invoke(selectedIndex);
            }
        }

        public Action<AvatarCellView> OnPressedCell;
        public Action<int> OnSelectedIndexChanged;
    }
}
