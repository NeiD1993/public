using GameScene.Services.Content;

namespace GameScene.Services.Game
{
    public class RemainedStepsExtractionService : BaseContentWithChangeablyParameterizedNonDefaultExtractionService<int, int, int>
    {
        protected override int ExtractDefaultContent(int constantDefaultExtractionParameter)
        {
            return constantDefaultExtractionParameter;
        }

        protected override int ExtractNonDefaultContent(int nonDefaultExtractionParameter)
        {
            return (nonDefaultExtractionParameter > 0) ? --nonDefaultExtractionParameter : nonDefaultExtractionParameter;
        }
    }
}