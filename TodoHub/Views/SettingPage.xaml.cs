using Microsoft.Maui.Storage;
using System.Text.RegularExpressions;
namespace TodoHub.Views
{
    public partial class SettingPage : ContentPage
    {
        // 入力受け取り用の変数
        string token = string.Empty;
        string repo = string.Empty;


        public SettingPage()
        {
            InitializeComponent();
        }


        private void ChangeTokenBtn_Clicked(object? sender, EventArgs e)// トークン変更ボタンの動作
        {
            ChangeTokenBtn.IsVisible = false; // ボタン消す
            ChangeTokenForm.IsVisible = true; // フォーム出す
        }


        private async void RegisterTokenBtn_Clicked(object? sender, EventArgs e)// トークンの登録ボタンの動作
        {
            // 登録ボタンの動作を無効化
            RegisterTokenBtn.IsEnabled = false;

            try
            {
                token = ChangeTokenEntry.Text;

                if (string.IsNullOrEmpty(token))// トークンが空の場合はエラーメッセージを表示
                {
                    await DisplayAlertAsync("エラー", "トークンを入力してください。", "OK");
                    return;
                }


                // トークンをセキュアに保存
                await SecureStorage.Default.SetAsync("github_token", token);
                await DisplayAlertAsync("", "トークンの登録が完了しました。", "OK");

                // 変更フォームを消して、トークン変更ボタンを表示
                ChangeTokenForm.IsVisible = false;
                ChangeTokenBtn.IsVisible = true;
            }
            catch (Exception)
            {
                await DisplayAlertAsync("エラー", "保存に失敗ました。", "OK");
            }
            finally
            {
                // 登録ボタンの動作を再有効化
                RegisterTokenBtn.IsEnabled = true;
            }
        }



        // -----------------------------------------------------------------------------------------------------
        // -----------------------------------------------------------------------------------------------------


        private void ChangeRepoBtn_Clicked(object? sender, EventArgs e)// リポジトリ変更ボタンの動作
        {
            ChangeRepoBtn.IsVisible = false; // ボタン消す
            ChangeRepoForm.IsVisible = true; // フォーム出す
            if (!string.IsNullOrEmpty(Preferences.Default.Get("github_repo", "")))
            {
                ChangeRepoEntry.Text = Preferences.Default.Get("github_repo", "");
            }
        }


        private async void RegisterRepoBtn_Clicked(object? sender, EventArgs e)// リポジトリの登録ボタンの動作
        {
            // 登録ボタンの動作を無効化
            RegisterRepoBtn.IsEnabled = false;

            try
            {
                repo = ChangeRepoEntry.Text;
                var RepoPattern = @"^[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?/[a-zA-Z0-9._-]+$";

                if (!Regex.IsMatch(repo, RepoPattern))
                {
                    await DisplayAlertAsync("エラー", "正しいリポジトリ名を入力してください。", "OK");
                    return;
                }

                if (string.IsNullOrEmpty(repo))// トークンが空の場合はエラーメッセージを表示
                {
                    await DisplayAlertAsync("エラー", "リポジトリ名を入力してください。", "OK");
                    return;
                }


                // リポジトリ名を保存
                Preferences.Default.Set("github_repo", repo);
                await DisplayAlertAsync("", "リポジトリの登録が完了しました。", "OK");

                // 変更フォームを消して、トークン変更ボタンを表示
                ChangeRepoForm.IsVisible = false;
                ChangeRepoBtn.IsVisible = true;
            }
            catch (Exception)
            {
                await DisplayAlertAsync("エラー", "保存に失敗ました。", "OK");
            }
            finally
            {
                // 登録ボタンの動作を再有効化
                RegisterRepoBtn.IsEnabled = true;
            }
        }
    }
}