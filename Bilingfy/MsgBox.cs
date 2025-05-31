using System.Threading.Tasks;
using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;

namespace Bilingfy;

public static class MsgBox
{
    const int WIDTH = 640;

    public static async void ShowUnknownError(Window owner, string raiser)
    {
        var msgBoxParams = new MessageBoxStandardParams
        {
            CanResize = false,
            Icon = Icon.Error,
            MaxWidth = WIDTH,
            ShowInCenter = true,
            ContentTitle = "This is a super rare error!",
            ContentMessage = string.Format(@"We don't really know what went wrong ¯\_(ツ)_/¯
Please contact the developer so we can figure it out!
Keyword of the possible reason: {0}", raiser),
            ButtonDefinitions = ButtonEnum.Ok
        };
        await MessageBoxManager.GetMessageBoxStandard(msgBoxParams).ShowWindowDialogAsync(owner);
    }

    public static async void ShowError(Window owner, string title, string message)
    {
        var msgBoxParams = new MessageBoxStandardParams
        {
            CanResize = false,
            Icon = Icon.Error,
            MaxWidth = WIDTH,
            ShowInCenter = true,
            ContentTitle = title,
            ContentMessage = message,
            ButtonDefinitions = ButtonEnum.Ok,

        };
        await MessageBoxManager.GetMessageBoxStandard(msgBoxParams).ShowWindowDialogAsync(owner);
    }
    
    public static async Task<bool> ShowConfirmation(Window owner, string title, string message)
    {
        var msgBoxParams = new MessageBoxStandardParams
        {
            CanResize = false,
            Icon = Icon.Question,
            MaxWidth = WIDTH,
            ShowInCenter = true,
            ContentTitle = title,
            ContentMessage = message,
            ButtonDefinitions = ButtonEnum.YesNo
        };
        var result = await MessageBoxManager.GetMessageBoxStandard(msgBoxParams).ShowWindowDialogAsync(owner);
        return result == ButtonResult.Yes;
    }
}