using UnityEngine;

namespace SMT3.Core
{
    public interface ITickable
    {
        void OnTick(double songTime);
    }
}
