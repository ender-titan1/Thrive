using System.Collections.Generic;
using Godot;

public class AchievementViewer : CustomWindow
{
    [Export]
    public NodePath? AchievementGridPath;

    [Export]
    public NodePath TabButtonsPath = null!;

#pragma warning disable CA2213
    [Export]
    public PackedScene AchievementCardScene = null!;

    private GridContainer achievementTile = null!;
    private TabButtons tabButtons = null!;
#pragma warning restore CA2213

    private readonly List<(Control Control, ICustomToolTip ToolTip)> registeredToolTips = new();

    private Dictionary<string, HashSet<AchievementCard>> stages = new();
    private bool tooltipsDetached;
    private bool hasBecomeVisibleAtLeastOnce;

    public override void _Ready()
    {
        achievementTile = GetNode<GridContainer>(AchievementGridPath);
        tabButtons = GetNode<TabButtons>(TabButtonsPath);
    }

    public override void _EnterTree()
    {
        base._EnterTree();

        if (tooltipsDetached)
            ReAttachToolTips();
    }

    public override void _ExitTree()
    {
        base._ExitTree();

        if (registeredToolTips.Count > 0)
            DetachToolTips();

        UnregisterToolTips();
    }

    public override void _Notification(int what)
    {
        base._Notification(what);

        if (what == NotificationTranslationChanged && hasBecomeVisibleAtLeastOnce)
        {
            InitializeViewer();
        }
        else if (what == NotificationVisibilityChanged && Visible && !hasBecomeVisibleAtLeastOnce)
        {
            hasBecomeVisibleAtLeastOnce = true;
            InitializeViewer();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (AchievementGridPath != null)
            {
                AchievementGridPath.Dispose();
                TabButtonsPath.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    private void InitializeViewer()
    {
        tabButtons.ClearTabButtons();
        achievementTile.QueueFreeChildren();

        Button? firstEntry = null;

        foreach (var achievement in SimulationParameters.Instance.GetAllAchievements())
        {
            string stage = achievement.Stage;

            if (!stages.ContainsKey(stage))
            {
                var tabButton = new Button
                {
                    Name = stage,
                    Text = TranslationServer.Translate(stage),
                    SizeFlagsHorizontal = 0,
                    ToggleMode = true,
                    ActionMode = BaseButton.ActionModeEnum.Press,
                };

                firstEntry ??= tabButton;
                tabButtons.AddNewTab(tabButton);
                stages.Add(stage, new());
            }

            var created = CreateAchievementCard(achievement);
            achievementTile.AddChild(created);
            stages[stage].Add(created);
        }

        firstEntry!.Pressed = true;
    }

    private AchievementCard CreateAchievementCard(Achievement achievement)
    {
        AchievementCard item;
        item = AchievementCardScene.Instance<AchievementCard>();
        item.Thumbnail = achievement.LoadedIcon!;
        item.Achievement = achievement;

        var tooltip = ToolTipHelper.GetDefaultToolTip();
        tooltip.Description = achievement.Description;

        item.RegisterToolTipForControl(tooltip, false);
        registeredToolTips.Add((item,  tooltip));

        return item;
    }

    private void UnregisterToolTips()
    {
        tooltipsDetached = false;
        if (registeredToolTips.Count < 1)
            return;

        foreach (var (control, tooltip) in registeredToolTips)
        {
            control.UnRegisterToolTipForControl(tooltip);
        }

        registeredToolTips.Clear();
    }

    private void DetachToolTips()
    {
        tooltipsDetached = true;

        foreach (var (control, tooltip) in registeredToolTips)
        {
            control.UnRegisterToolTipForControl(tooltip);
        }
    }

    private void ReAttachToolTips()
    {
        if (!tooltipsDetached)
            return;

        foreach (var (control, tooltip) in registeredToolTips)
        {
            control.RegisterToolTipForControl(tooltip, false);
        }

        tooltipsDetached = false;
    }

    private void OnCloseButtonPressed()
    {
        GUICommon.Instance.PlayButtonPressSound();
        Hide();
    }
}