namespace KhumaloCraft.Application.DisplayModes;

public interface IDisplayModeSelector
{
    DisplayModeType GetSelected();
    DisplayModeType GetDetectedDisplayMode();
    void SetSelectedInterface(DisplayModeType displayModeType);
}