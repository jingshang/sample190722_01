#ビルド用のコンテナを立てる。（なんとこいつが1.75GB！）
FROM mcr.microsoft.com/dotnet/core/sdk:2.1 AS build

#サーバーの/appに移動（存在しないので同時に作成される）
WORKDIR /app

# copy csproj and restore as distinct layers

#ソリューションファイルを /appへコピー
COPY *.sln .

#プロジェクトファイルを /app/sample190722_01 へコピー
COPY sample190722_01/*.csproj ./sample190722_01/

#実行⇒（プロジェクトの依存関係とツールを復元します。NuGet を使用します。）
RUN dotnet restore

# copy everything else and build app

#プロジェクト内のすべてのファイルを./sample190722_01/へコピー
COPY sample190722_01/. ./sample190722_01/

#サーバーのプロジェクトディレクトリに移動
WORKDIR /app/sample190722_01

#ビルドファイルをRelease構成でoutフォルダーにパックします。（-oは出力ディレクトリ指定オプション）
RUN dotnet publish -c Release -o out


#実行用のコンテナを立てる。（実行コンテナは小さいよ！263MB！）
FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 AS runtime

#サーバーの/appに移動
WORKDIR /app

#ビルド用のコンテナから実行フォルダ一式をコピーする
COPY --from=build /app/sample190722_01/out ./

#実行時のコマンド指定（第一引数＝コマンド、第二引数＝パラメータ）
ENTRYPOINT ["dotnet", "sample190722_01.dll"]