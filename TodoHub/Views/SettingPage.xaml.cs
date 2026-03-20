namespace TodoHub.Views;
using Microsoft.Maui.Storage;

public partial class SettingPage : ContentPage
{
	string token = string.Empty;
	public SettingPage()
	{
		InitializeComponent();
	}

    private void ChangeTokenBtn_Clicked(object sender, EventArgs e)// トークン変更ボタンの動作
    {
        ChangeTokenBtn.IsVisible = false; // ボタン消す
        TokenForm.IsVisible = true;       // フォーム出す
    }

    [Obsolete]
	private async void RegisterTokenBtn_Clicked(object? sender, EventArgs e)// 変更ボタンの動作
	{
        // 登録ボタンの動作を無効化
        RegisterTokenBtn.IsEnabled = false;

        try
        {
            token = TokenEntry.Text;

            if (string.IsNullOrEmpty(token))// トークンが空の場合はエラーメッセージを表示
            {
                await DisplayAlertAsync("エラー", "トークンを入力してください。", "OK");
                return;
            }


            // トークンをセキュアに保存
            await SecureStorage.Default.SetAsync("github_token", token);
            await DisplayAlertAsync("", "トークンの登録が完了しました。", "OK");

            // 変更フォームを消して、トークン変更ボタンを表示
            TokenForm.IsVisible = false;
            ChangeTokenBtn.IsVisible = true;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("エラー", "保存に失敗ました。", "OK");
        }
        finally
        {
            // 変更ボタンの動作を再有効化
            RegisterTokenBtn.IsEnabled = true;
        }
    }
}