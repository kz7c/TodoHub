using System.Net.Http.Headers;
using System.Text.Json;
namespace TodoHub.Views;

public partial class EditPage : ContentPage
{

	public EditPage()
	{
		InitializeComponent();
	}

    // 保存ボタンが押されたときの処理
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

            // 内容が空の場合は何もしない。
            if (string.IsNullOrEmpty(CreateBody.Text))
            {
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

            // ユーザーエージェントを設定（GitHub APIはユーザーエージェントが必要）
            APIclient.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("TodoHubApp", "1.0"));

            // トークン使う場合
            APIclient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var url = "https://api.github.com/repos/" + repo + "/issues";

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

            // 入力領域の初期化
            CreateTitle.Text = "";
            CreateBody.Text = "";
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