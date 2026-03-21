using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Storage;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Maui.Networking;

namespace TodoHub.Views
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(Preferences.Default.Get("github_repo", "")))
            {
                // MainTitleLabel.Text = Preferences.Default.Get("github_repo", "");
                this.Title = Preferences.Default.Get("github_repo", "");
                TodoLoad();
            }
        }

        private async void TodoLoad()
        {
            try
            {
                // トークンとリポジトリの情報を取得---------------------------------------------------
                string? token = await SecureStorage.Default.GetAsync("github_token");
                string repo = Preferences.Default.Get("github_repo", "");

                // リポジトリが設定されていない場合は処理を中断
                if (string.IsNullOrEmpty(repo))
                {
                    MainMessage.IsVisible = true;
                    MainMessage.Text = "リポジトリが設定されていません。";
                    return;
                }

                if (string.IsNullOrEmpty(token))
                {
                    MainMessage.IsVisible = true;
                    MainMessage.Text = "トークンが設定されていません。";
                    return;
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

                var response = await APIclient.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();


                // JSONをパースして必要な情報を抽出------------------------------------------------------
                JsonDocument TodoListData = JsonDocument.Parse(json);
                TodoList.Children.Clear();

                foreach (JsonElement issue in TodoListData.RootElement.EnumerateArray())
                {
                    // 画面レンダリング
                    int number = issue.GetProperty("number").GetInt32();

                    string? title = issue.GetProperty("title").GetString();

                    DateTime utc = issue.GetProperty("updated_at").GetDateTime();
                    DateTime jst = utc.AddHours(9);// 日本時間に変換
                    string updated = jst.ToString("yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture);// 表示用にフォーマット

                    // コメント数
                    // int comments = issue.GetProperty("comments").GetInt32();

                    // div的なやつのUIを作成
                    Border item_border = new Border
                    {
                        Stroke = Colors.Gray,
                        StrokeThickness = 1,
                        Padding = 10,
                        Margin = new Thickness(0, 0, 0, 10),
                        StrokeShape = new RoundRectangle
                        {
                            CornerRadius = 10
                        }
                    };

                    VerticalStackLayout item = new VerticalStackLayout
                    {
                        Padding = 10,
                        Spacing = 5
                    };


                    item.Children.Add(new Label { Text = title });
                    item.Children.Add(new Label { Text = $"更新日: {updated}" });
                    // item.Children.Add(new Label { Text = $"コメント数: {comments}" });


                    // タップイベント


                    TapGestureRecognizer tap = new TapGestureRecognizer();

                    tap.Tapped += async (s, e) =>
                    {
                        item_border.BackgroundColor = Colors.LightGray;

                        await Task.Delay(100);

                        item_border.BackgroundColor = Colors.Transparent; // 背景を透明に

                        // idを渡して詳細ページへ
                        await Navigation.PushAsync(new EditPage(number));
                    };

                    item.GestureRecognizers.Add(tap);


                    //　ホバーイベント
                    PointerGestureRecognizer pointer = new PointerGestureRecognizer();
                    pointer.PointerEntered += (s, e) =>
                    {
                        item_border.BackgroundColor = Colors.LightGray;
                    };

                    pointer.PointerExited += (s, e) =>
                    {
                        item_border.BackgroundColor = Colors.Transparent;// 背景を透明に
                    };

                    item.GestureRecognizers.Add(pointer);
                    // 画面に追加
                    // itemをボーダーの中に入れる
                    item_border.Content = item;

                    // ここでボーダーを追加
                    TodoList.Children.Add(item_border);

                }

            }
            catch (Exception)
            {
                MainMessage.IsVisible = true;
                MainMessage.Text = "エラーが発生しました　\n ネットワークの設定や　\n リポジトリ・トークンの設定などを \n 見直してください。";
                return;
            }
        }
    }
}
