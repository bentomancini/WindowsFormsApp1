# Tecfy Music Player

Aplicativo desktop de música em **Windows Forms (C# / .NET Framework 4.7.2)** que integra busca, reprodução, playlists, favoritas, artistas e uma home responsiva com destaques.

## Funcionalidades

- **Login e cadastro** de usuários (criar conta e recuperar senha).
- **Busca** de músicas, artistas e playlists integrada com a **API do Spotify** e com a **iTunes Search API**.
- **Player de música** com preview das faixas (reprodução via **NAudio**, controle de volume, tempo e equalizador animado).
- **Home** com artistas e álbuns em destaque, que podem ser tocados diretamente.
- **Aba de Artistas** com perfil, imagem, gêneros, seguidores e as 10 músicas mais famosas (tocáveis).
- **Playlists**:
  - Criar, renomear, apagar e reordenar playlists (arrastar e soltar).
  - Adicionar músicas vindas da busca a uma playlist.
  - Lista lateral ("Suas Playlists") com cards e capas, atualizada em tempo real.
- **Favoritas**: marcar músicas como favoritas e acompanhar o contador.
- **Perfil do usuário**: avatar com iniciais, nome editável e saudação dinâmica, persistidos em `perfil_usuario.json`.
- **Tema claro/escuro**.

## Tecnologias

| Camada          | Tecnologia                                            |
|-----------------|-------------------------------------------------------|
| Interface       | Windows Forms + **Guna.UI2**                          |
| Linguagem       | C# (.NET Framework 4.7.2)                             |
| Banco de dados  | SQL Server (`Tecfy`) via ADO.NET (DAO)                |
| Música          | **NAudio** (reprodução de preview)                    |
| APIs externas   | **Spotify Web API**, **iTunes Search API**            |
| WebView         | **WebView2** (telas auxiliares)                       |

## Estrutura do projeto

```
WindowsFormsApp1.sln
WindowsFormsApp1/
├── Form1.cs                # Login
├── Form2.cs                # Aba principal (home, playlists, artistas, perfil)
├── Form3.cs                # Resultados de busca / player
├── ControleInicio.cs       # Home com destaques de artistas e álbuns
├── ControleArtistas.cs     # Perfil e músicas do artista
├── ControlePlaylists.cs    # Gestão de playlists (CRUD + reordenação)
├── ControleEqualizer.cs    # Animação do player
├── SpotifyService.cs       # Chamadas Spotify e iTunes
├── UsuarioService.cs       # Autenticação/cadastro de usuários
├── ArtistaDAO.cs           # Acesso a artistas
├── MusicaDAO.cs            # Acesso a músicas
├── PlaylistDAO.cs          # CRUD de playlists e músicas
├── FavoritoDAO.cs          # Favoritas
├── Banco.cs                # Conexão com SQL Server
├── Tema.cs                 # Temas claro/escuro
└── assets/                 # Página auxiliar (HTML/CSS/JS)
```

## Pré-requisitos

- **Visual Studio 2019+** ou MSBuild.
- **.NET Framework 4.7.2** Developer Pack.
- **SQL Server** acessível com o banco `Tecfy`.
- Pacotes NuGet (restaurados automaticamente): Guna.UI2, NAudio, SpotifyAPI.Web, Microsoft.Web.WebView2, Newtonsoft.Json.

## Como executar

1. Clone o repositório:

   ```bash
   git clone https://github.com/joaopedrotb/WindowsFormsApp1.git
   ```

2. Abra `WindowsFormsApp1.sln` no Visual Studio.

3. Restaure os pacotes NuGet (o Visual Studio faz isso ao compilar, ou use `nuget restore`).

4. Ajuste a connection string em `WindowsFormsApp1/Banco.cs` para apontar para o seu servidor SQL:

   ```csharp
   public const string StringConexao =
       @"Data Source=SEU_SERVIDOR; Initial Catalog=Tecfy; Integrated Security=True";
   ```

5. Compile e execute (`F5`).

## Configuração do Spotify

O aplicativo usa credenciais da [Spotify Developer Dashboard](https://developer.spotify.com/dashboard). As credenciais atuais ficam em `WindowsFormsApp1/SpotifyService.cs`:

```csharp
private const string ClientId = "SEU_CLIENT_ID";
private const string ClientSecret = "SEU_CLIENT_SECRET";
```

> **Atenção:** não suba credenciais reais para o repositório. Registre o app no dashboard do Spotify e preencha os seus valores localmente.

## Banco de dados

O app espera o banco `Tecfy` com as tabelas principais:

- `Usuarios` (`Id`, `Nome`, `Email`, `Senha`, ...)
- `Artistas` (`Id`, `Nome`, ...)
- `Musicas` (`Id`, `Titulo`, `Album`, `DuracaoSegundos`, `CapaUrl`, `ArquivoUrl`, `ArtistaId`)
- `Playlists` (`Id`, `UsuarioId`, `Nome`, `Descricao`, `Publica`)
- `PlaylistMusicas` (`PlaylistId`, `MusicaId`, `Ordem`)
- `Favoritas`

## Colaboradores

- [joaopedrotb](https://github.com/joaopedrotb)
- [bentomancini](https://github.com/bentomancini)