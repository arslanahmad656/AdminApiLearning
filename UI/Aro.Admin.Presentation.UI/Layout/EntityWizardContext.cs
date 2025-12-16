namespace Aro.Admin.Presentation.UI.Layout;

public class EntityWizardContext<TEntity>
{
    public int ActiveStepIndex { get; set; } = 0;
    public List<string> StepTitles { get; set; } = new();
    public bool EntityCreationComplete { get; set; } = false;
    public bool IsCreateMode { get; set; } = true;
    public Func<Task>? OnNext { get; set; }
    public Func<Task<bool>>? CurrentStepValidation { get; set; }
    public Action? OnBack { get; set; }
    public Func<Task>? OnSave { get; set; }
    public Func<Task>? OnEdit { get; set; }
    public TEntity? Entity { get; set; }
}

