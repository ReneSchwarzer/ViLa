using ViLa.Model;
using WebExpress.WebCore.WebCondition;
using WebExpress.WebCore.WebMessage;

namespace ViLa.WebCondition
{
    public class ConditionTimeControl : ICondition
    {
        public bool Fulfillment(IRequest request)
        {
            return ViewModel.Instance.Settings.Mode == Mode.TimeControlled;
        }
    }
}
