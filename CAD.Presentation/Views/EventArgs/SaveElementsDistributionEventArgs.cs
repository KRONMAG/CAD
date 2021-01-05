using CodeContracts;

namespace CAD.Presentation.Views.EventArgs
{
    public class SaveElementsDistributionEventArgs
    {
        public string FilePath { get; }

        public SaveElementsDistributionEventArgs(string filePath)
        {
            Requires.NotNull(filePath, nameof(filePath));

            FilePath = filePath;
        }
    }
}