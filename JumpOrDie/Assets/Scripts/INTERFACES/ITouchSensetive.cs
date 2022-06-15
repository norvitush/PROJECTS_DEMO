using UnityEngine.Events;

namespace VOrb
{
    public interface ITouchSensetive
    {
       UnityEvent TapEvent { get; set; }
       UnityEvent SvipeEvent { get; set; }
    }
}

