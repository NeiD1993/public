using System.Text.RegularExpressions;
using GameScene.Managers.ScoreItems.Settings;
using GameScene.Services.Content;
using GameScene.Services.Content.Parameters;

namespace GameScene.Services.ScoreItems
{
    public class ScoreItemInformationExtractionService : BaseContentWithFullyParameterizedNonDefaultExtractionService<string, 
        ScoreItemTextControlInformationCompositionSettings, object, string>
    {
        protected override string ExtractDefaultContent(string constantDefaultExtractionParameter)
        {
            return constantDefaultExtractionParameter;
        }

        protected override string ExtractNonDefaultContent(ContentNonDefaultExtractionParameters<ScoreItemTextControlInformationCompositionSettings, 
            object> nonDefaultExtractionParameter)
        {
            string changeableSectionContentReplacement = string.Format(nonDefaultExtractionParameter.Constant.ChangeableSectionSettings.Format, 
                nonDefaultExtractionParameter.Changeable);
            string information = Regex.Replace(nonDefaultExtractionParameter.Constant.Pattern, nonDefaultExtractionParameter.Constant.ChangeableSectionSettings.Identifier,
                changeableSectionContentReplacement);

            return information;
        }
    }
}