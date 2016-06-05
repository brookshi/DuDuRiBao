using Brook.DuDuRiBao.Common;
using Brook.DuDuRiBao.Events;
using Brook.DuDuRiBao.Models;
using Brook.DuDuRiBao.Utils;
using LLQ;
using System.Linq;
using System.Threading.Tasks;

namespace Brook.DuDuRiBao.ViewModels
{
    public class CircleStoryViewModel : ViewModelBase
    {
        public string CircleId { get; set; }

        public CircleInfo Circle { get; set; }

        private string _currentDate;

        private readonly ObservableCollectionExtended<Story> _storyDataList = new ObservableCollectionExtended<Story>();

        public ObservableCollectionExtended<Story> StoryDataList { get { return _storyDataList; } }

        public CircleStoryViewModel()
        {
            
        }

        public async Task RefreshCircleInfo()
        {
            if (string.IsNullOrEmpty(CircleId))
                return;

            Circle = await DataRequester.RequestCircleInfo(CircleId);
            Notify("Circle");
        }
    }
}
