# Rota do Sabor — Sistema de Delivery de Comida

Etapa 1: **tela inicial**, **login**, **cadastro** e **banco de dados** (SQLite + Entity Framework Core, Code First).

## Como rodar

1. Visual Studio 2022 (17.8+) ou 2026 com a carga de trabalho **Desenvolvimento para desktop com .NET**.
2. Abrir `RotaDoSabor.sln` → o NuGet restaura os pacotes sozinho → **F5**.

O banco é criado automaticamente na primeira execução em:
`%LocalAppData%\RotaDoSabor\rotadosabor.db` (dá para abrir com o *DB Browser for SQLite*).

### Usuário mestre (criado automaticamente)

| Campo   | Valor              |
|---------|--------------------|
| Usuário | `Mestre`           |
| E-mail  | `mestre@gmail.com` |
| Senha   | `123456`           |

No login, o primeiro campo aceita **o usuário ou o e-mail** (sem diferenciar maiúsculas/minúsculas).

## Fluxo de telas (conforme o Excalidraw)

`Tela inicial` → `Login` → `Home (provisória)`  
`Tela inicial` → `Cadastro` → `Login`

## Estrutura (MVC em camadas)

```
RotaDoSabor/
├─ Models/         Usuario, DadosCadastro                      (M)
├─ Views/          WelcomeView, LoginView, CadastroView, HomeView
│  └─ Components/  PainelMarca (painel reutilizável)           (V)
├─ Controllers/    AuthController                              (C)
├─ Services/       AuthService, PasswordHasher, ValidadorCadastro, SessaoUsuario  (regras de negócio)
├─ Repositories/   IUsuarioRepository, UsuarioRepository       (acesso a dados)
├─ Data/           AppDbContext, DbInitializer, ConfiguracaoBanco
├─ Navegacao/      INavegador, Navegador
├─ Common/         Resultado
└─ Resources/      Styles.xaml (cores e estilos)
```

## Onde foram aplicados SOLID e Clean Code (para a documentação)

**SOLID**
- **S** (responsabilidade única): `PasswordHasher` só faz hash; `ValidadorCadastro` só valida; `UsuarioRepository` só persiste; `AuthController` só coordena tela ↔ serviço.
- **O** (aberto/fechado): `Navegador.NavegarPara<T>()` aceita qualquer tela nova sem mudar; `IPasswordHasher` permite trocar o algoritmo sem mexer no `AuthService`.
- **L** (Liskov): qualquer implementação de `IUsuarioRepository` (ex.: em memória, para testes) substitui `UsuarioRepository`.
- **I** (segregação de interfaces): interfaces pequenas — `IPasswordHasher`, `ISessaoUsuario`, `INavegador`, `IUsuarioRepository`.
- **D** (inversão de dependência): `AuthService` e `AuthController` recebem interfaces no construtor; as classes concretas só aparecem em `App.xaml.cs` (*Composition Root*).

**Clean Code**
- Nomes em português que dizem a intenção (`ObterPorNomeOuEmailAsync`, `CriarUsuarioMestre`).
- Métodos curtos, uma tarefa cada; code-behind das Views só repassa eventos ao controller.
- `Resultado` no lugar de exceções para fluxos esperados (senha errada, e-mail repetido).
- Constantes nomeadas (`CredenciaisInvalidas`, `SenhaTamanhoMinimo`) em vez de textos/números soltos.
- Estilos centralizados em `Styles.xaml` (nada de cor/fonte repetido nas telas).

## Banco de dados

- **Code First**: o esquema vem de `Models/Usuario.cs` + `Data/AppDbContext.cs`.
- **SQL puro** (exigência da atividade): `UsuarioRepository.ObterPorNomeOuEmailAsync` usa `FromSqlInterpolated` (parametrizado, sem risco de SQL Injection). Se o professor definir outra funcionalidade para isso, é só trocar.
- **Senha** nunca fica em texto puro: PBKDF2-SHA512 com salt aleatório (`PasswordHasher`).
- Usuário e e-mail são **únicos** (índice único) e ignoram maiúsculas/minúsculas (collation `NOCASE`).
- O nome de usuário **não pode conter `@`**, para não confundir com e-mail no login.

### Evoluindo o banco (próximos itens do backlog)

Hoje o esquema é criado com `EnsureCreated`, que **não altera** um banco já existente. Durante o desenvolvimento, ao adicionar tabelas, basta apagar o arquivo `.db`. Para usar *Migrations* (recomendado):

1. No *Package Manager Console*: `Add-Migration Inicial`
2. Nada mais: o `DbInitializer` detecta as migrations e passa a usar `Migrate()` sozinho.

(Apague o `.db` antigo antes da primeira migration.)

## Observação

O designer do Visual Studio pode mostrar "Não é possível criar uma instância" nas Views, porque elas recebem o controller por injeção de dependência. Isso só afeta a pré-visualização; o aplicativo roda normalmente.
