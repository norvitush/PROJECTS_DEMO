using System;

namespace GoldenSoft.UI.MVVM
{
    public interface IUIModel 
    {
        public event Action Changed;

        public void SetDataSources(params object[] Sources);
        public void FreeDataSource();
    }
}

