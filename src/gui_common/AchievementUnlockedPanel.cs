using Godot;

public class AchievementUnlockedPanel : Control
{
    [Export]
    public NodePath? NameLabelPath;

    [Export]
    public NodePath DescriptionLabelPath = null!;

    [Export]
    public NodePath AnimationPlayerPath = null!;

#pragma warning disable CA2213
    private Label nameLabel = null!;
    private Label descriptionLabel = null!;
    private AnimationPlayer animationPlayer = null!;
#pragma warning restore CA2213

    public override void _Ready()
    {
        nameLabel = GetNode<Label>(NameLabelPath);
        descriptionLabel = GetNode<Label>(DescriptionLabelPath);
        animationPlayer = GetNode<AnimationPlayer>(AnimationPlayerPath);
    }

    public override void _EnterTree()
    {
        AchievementManager.OnShowNewAchievementPanel += ShowPanel;
    }

    public override void _ExitTree()
    {
        AchievementManager.OnShowNewAchievementPanel -= ShowPanel;
    }

    public void OnPanelAnimationCompleted(string animationName)
    {
        _ = animationName;
        Visible = false;

        // Show the next achievement in the queue if possible
        AchievementManager.ResolveNextCompletedAchievement();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (NameLabelPath != null)
            {
                NameLabelPath.Dispose();
                DescriptionLabelPath.Dispose();
                AnimationPlayerPath.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    private void ShowPanel(object sender, Achievement achievement)
    {
        Visible = true;

        nameLabel.Text = achievement.Name;
        descriptionLabel.Text = achievement.Description;

        animationPlayer.Play("FadeIn");
    }

}
