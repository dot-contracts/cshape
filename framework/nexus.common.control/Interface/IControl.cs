using nexus.common.control.Themes;
using nexus.common.control;

public interface IControl
{
    #region Properties

    string     Name         { get; }
    bool       IsReadOnly   { get; set; }
    double     ValueDbl     { get; set; }   // 🔄 Made settable
    string     Value        { get; set; }   // 🔄 Made settable
    bool       IsValidated  { get; set; }   // ✅ Newly added
    object     Tag          { get; }
    Visibility Visibility   { get; }
    double     Opacity      { get; set; }

    #endregion

    #region Methods

    void Focus();
    void SetFocus();
    bool Validate();
    bool ProcessKey(string key, bool sendKey);

    #endregion

    #region Events

    event EventHandler<object>?            PreviewMouseDown;
    event EventHandler<string>?            OnProcessKey;
    event EventHandler<KeyDownEventArgs>?  OnProcessKeyDown;
    event EventHandler<ChangeEventArgs>?   OnChange;
    event EventHandler<ChangedEventArgs>?  OnChanged;
    event EventHandler<ClickedEventArgs>?  OnClicked;

    #endregion
}
