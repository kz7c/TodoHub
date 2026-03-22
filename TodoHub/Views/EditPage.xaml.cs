using System.Net.Http.Headers;
using System.Text.Json;
namespace TodoHub.Views;

public partial class EditPage : ContentPage
{
    private int? _issueNum;
    public EditPage(int? issueNum = null)
	{
        InitializeComponent();
        _issueNum = issueNum;
        
        if(issueNum == null )
        {
            Title = "新規作成";
        }

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        MainGrid.IsVisible = false;
        LoadingLabel.IsVisible = true;

        if (_issueNum != null)
        {
            await issue_load(); // ← ここでちゃんと待つ
        }

        MainGrid.IsVisible = true;
        LoadingLabel.IsVisible = false;
    }

    // issueNumがnullでない場合、つまり編集モードの場合に呼び出されるメソッド--------------------------------------------------
    private async Task issue_load()
    {
        try
        {
            // トークンとリポジトリの情報を取得---------------------------------------------------
            string? token = await SecureStorage.Default.GetAsync("github_token");
            string repo = Preferences.Default.Get("github_repo", "");
            // リポジトリが設定されていない場合は処理を中断
            if (string.IsNullOrEmpty(repo))
            {
                return;
            }
            // トークンが設定されていない場合は処理を中断
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            // APIを叩いてTodoを取得する処理------------------------------------------------------
            // HttpClientのインスタンスを作成
            var APIclient = new HttpClient();
            // ユーザーエージェントを設定
            APIclient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(AppInfo.Name, AppInfo.BuildString));
            
            // トークン
            APIclient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            string url = "https://api.github.com/repos/" + repo + "/issues/" + _issueNum;

            var response = await APIclient.GetAsync(url);
            var json = await response.Content.ReadAsStringAsync();
            JsonDocument issueData = JsonDocument.Parse(json);

            // タイトルと内容をUIに反映
            string issue_Title = issueData.RootElement.GetProperty("title").GetString() ?? "";
            CreateTitle.Text = issue_Title;
            Title = "編集中: " + issue_Title;
            CreateBody.Text = issueData.RootElement.GetProperty("body").GetString();
        }

        catch (Exception)
        {
            await DisplayAlertAsync("", "エラーが発生しました。", "OK");
        }
    }




    // 保存ボタンが押されたときの処理---------------------------------------------------------------------------------------
    private async void SavingBtn_Clicked(object? sender, EventArgs e)
	{
        // 保存ボタンの動作を無効化
        SavingBtn.IsEnabled = false;
        try
        {
        // トークンとリポジトリの情報を取得---------------------------------------------------
            string? token = await SecureStorage.Default.GetAsync("github_token");
            string repo = Preferences.Default.Get("github_repo", "");

            // リポジトリが設定されていない場合は処理を中断
            if (string.IsNullOrEmpty(repo))
            {
                return;
            }

            // トークンが設定されていない場合は処理を中断
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            // 内容が空の場合
            if (string.IsNullOrEmpty(CreateBody.Text))
            {
                await DisplayAlertAsync("", "内容が無いよう", "OK");
                return;
            }

            // タイトルが空の場合は内容からタイトルを生成する
            if (string.IsNullOrEmpty(CreateTitle.Text))
            { 
                if (CreateBody.Text.Length > 10)
                {
                    CreateTitle.Text = CreateBody.Text.Substring(0, 10) + "...";
                }
                else
                {
                    CreateTitle.Text = CreateBody.Text;
                }
            }

        // APIを叩いてTodoを取得する処理------------------------------------------------------
            // HttpClientのインスタンスを作成
            var APIclient = new HttpClient();

            // ユーザーエージェントを設定
            APIclient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(AppInfo.Name, AppInfo.BuildString));

            // トークン
            APIclient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

         
        // APIのエンドポイントURLを設定
            string url;
            if (_issueNum == null)
            {// 新規作成の場合
                url = "https://api.github.com/repos/" + repo + "/issues";

            }
            else
            {// 編集の場合
                url = "https://api.github.com/repos/" + repo + "/issues/" + _issueNum;

            }

        // リクエストを作成
            var issue = new
            {
                title = CreateTitle.Text,
                body = CreateBody.Text
            };
            string json = JsonSerializer.Serialize(issue);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            // POSTリクエストを送信
            var response = await APIclient.PostAsync(url, content);

            // レスポンスの内容を取得
            string result = await response.Content.ReadAsStringAsync();

            await DisplayAlertAsync("", result, "OK");

            if (_issueNum == null)
            {   // 新規作成の場合、入力領域の初期化
                CreateTitle.Text = "";
                CreateBody.Text = "";
            }
            else
            {   // 編集の場合、前の画面に戻る
                await Navigation.PopAsync();
            }


        }

        catch (Exception)
        {
            await DisplayAlertAsync("", "エラーが発生しました。", "OK");
        }

        finally // 最後に保存ボタンの動作を再有効化
        {
            SavingBtn.IsEnabled = true;
        }
    }
}