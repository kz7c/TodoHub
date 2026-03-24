# TodoHub

TodoHubは、GitHubのIssue機能を利用した、シンプルでクロスプラットフォームなTodo管理アプリケーションです。
.NET MAUIで構築されています。

## 概要

このアプリは、指定したGitHubリポジトリのIssueをTodoタスクとして扱います。Issueの作成、閲覧、編集をアプリから直接行うことができ、開発者のワークフローに統合しやすいTodo管理を実現します。

## 主な機能

*   **Todo一覧表示 (ホーム):** 設定したリポジトリのIssue一覧を取得し、Todoリストとして表示します。
*   **Todo作成 (作成):** 新しいIssueを作成し、Todoとして追加します。
*   **設定:** GitHub Personal Access Token (PAT) と対象リポジトリの設定を行います。

## 動作環境

*   .NET 10.0
*   Android / Windows (その他 .NET MAUIがサポートするプラットフォーム)

## セットアップと使用方法

### 1. 前準備

アプリを使用するには、GitHubのPersonal Access Token が必要です。

1.  GitHubの設定 ([Developer settings](https://github.com/settings/tokens)) にアクセスします。
2.  **Personal access tokens** (Fine-grained 推奨)を生成します。
3.  Todo管理に使うリポジトリを選択し、**Issues** の **Read and write** 権限を与え、トークンを作成します。
4.  生成されたトークンをコピーしておきます。

### 2. アプリの設定

アプリを起動後、「設定」タブを開き、以下の情報を入力してください。

*   **トークン設定:** 取得したトークンを入力して登録します。
*   **リポジトリ設定:** Todoとして使用したいリポジトリを `ユーザー名/リポジトリ名` の形式で入力して登録します（例: `user/my-todo-repo`）。

### 3. 使用開始

設定が完了したら、アプリを再起動してください。

### 4. ライセンス
このプロジェクトは [GNU GENERAL PUBLIC LICENSE](LICENSE) ライセンスのもとで提供されています