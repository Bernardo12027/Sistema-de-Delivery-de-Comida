# Rota do Sabor — Delivery de comida (C# / WPF / .NET 10 / SQL Server LocalDB)

Aplicativo desktop acadêmico de delivery (perfil **Cliente**) feito em **C# + WPF**, **.NET 10**, **EF Core 10 (Code First + Migrations)**, **SQL Server LocalDB**, arquitetura **MVC em camadas**, **SOLID**, **Clean Code** e **Injeção de Dependência**. Uma funcionalidade usa **SQL manual (ADO.NET)**: o histórico de pedidos.

> Documentação complementar em `docs/`:
> - `docs/ARQUITETURA.md` — MVC, camadas, SOLID (com classes reais), Clean Code, EF Core, relacionamentos, SQL manual, segurança.
> - `docs/ROTEIRO_APRESENTACAO.md` — roteiro de demonstração para a apresentação.
> - `docs/VALIDACAO.md` — o que foi verificado e o que precisa ser testado no Windows.
> - `docs/diagramas/` — diagrama de classes e de casos de uso (PlantUML `.puml` + Mermaid `.mmd` + PNG).
> - `docs/sql/consulta-historico-pedidos.sql` — a consulta SQL manual.

---

## 1. Requisitos da máquina

| Item | Versão |
|---|---|
| Windows | 10 ou 11 |
| Visual Studio | 2026 (Community serve) |
| Cargas de trabalho do VS | **Desenvolvimento para desktop com .NET** e **Armazenamento e processamento de dados** (instala o SQL Server Express **LocalDB**) |
| SDK | .NET 10 (vem com o VS 2026) |

Conferir o LocalDB (Prompt de Comando):

```bat
sqllocaldb info
```

Deve aparecer `MSSQLLocalDB`. Se não aparecer:

```bat
sqllocaldb create MSSQLLocalDB -s
```

## 2. Abrir e executar no Visual Studio 2026

1. Descompacte o ZIP e abra **`RotaDoSabor.slnx`** (também há um `RotaDoSabor.sln` equivalente).
2. Aguarde o VS **restaurar os pacotes NuGet** (automático; se não ocorrer: botão direito na solução → *Restaurar Pacotes NuGet*).
3. Botão direito em **RotaDoSabor.Wpf** → *Definir como Projeto de Inicialização*.
4. **F5**.

Na primeira execução o app **cria o banco `RotaDoSaborDB` sozinho** (aplica as migrations com `Database.MigrateAsync()`) e insere os **dados iniciais** (seed). Nas próximas execuções nada é duplicado.

### Pacotes NuGet usados (já referenciados nos `.csproj`)

| Projeto | Pacote |
|---|---|
| Application | `Microsoft.Extensions.DependencyInjection.Abstractions` 10.0.0 |
| Infrastructure | `Microsoft.EntityFrameworkCore.SqlServer` 10.0.0 (traz o `Microsoft.Data.SqlClient` usado no SQL manual), `Microsoft.EntityFrameworkCore.Design` 10.0.0 |
| Wpf | `Microsoft.Extensions.DependencyInjection`, `Microsoft.Extensions.Configuration.Json`, `Microsoft.Extensions.Configuration.Binder` 10.0.0, `Microsoft.EntityFrameworkCore.Design` e `Microsoft.EntityFrameworkCore.Tools` 10.0.0 |

Não é preciso instalar nada manualmente: o *restore* baixa tudo. (Se quiser instalar pelo console: `Install-Package Microsoft.EntityFrameworkCore.SqlServer -Version 10.0.0 -ProjectName RotaDoSabor.Infrastructure`.)

## 3. Banco de dados

String de conexão em `src/RotaDoSabor.Wpf/appsettings.json`:

```json
"RotaDoSabor": "Server=(localdb)\\MSSQLLocalDB;Database=RotaDoSaborDB;Trusted_Connection=True;MultipleActiveResultSets=True;TrustServerCertificate=True"
```

Usa autenticação do Windows — **nenhuma senha no código**.

### Criar/atualizar o banco manualmente (opcional — o app já faz isso ao iniciar)

**Console do Gerenciador de Pacotes** (Ferramentas → Gerenciador de Pacotes do NuGet → Console):

```powershell
Update-Database -Project RotaDoSabor.Infrastructure -StartupProject RotaDoSabor.Wpf
```

**Ou pelo terminal** (na pasta da solução):

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update -p src/RotaDoSabor.Infrastructure -s src/RotaDoSabor.Wpf
```

### Migrations

- Ficam em `src/RotaDoSabor.Infrastructure/Migrations` (`InitialCreate` + `RotaDoSaborDbContextModelSnapshot`).
- Conferir se o modelo bate com a migration:
  `dotnet ef migrations has-pending-model-changes -p src/RotaDoSabor.Infrastructure -s src/RotaDoSabor.Wpf`
- Nova migration depois de alterar entidades:
  `Add-Migration NomeDaMudanca -Project RotaDoSabor.Infrastructure -StartupProject RotaDoSabor.Wpf`

**Se a migration inicial der qualquer problema** (ela foi escrita fora do Windows; veja `docs/VALIDACAO.md`), regenere-a — leva 1 minuto:

```powershell
Drop-Database -Project RotaDoSabor.Infrastructure -StartupProject RotaDoSabor.Wpf
# apague a pasta src/RotaDoSabor.Infrastructure/Migrations
Add-Migration InitialCreate -Project RotaDoSabor.Infrastructure -StartupProject RotaDoSabor.Wpf
Update-Database -Project RotaDoSabor.Infrastructure -StartupProject RotaDoSabor.Wpf
```

### Ver os dados
*Exibir → Pesquisador de Objetos do SQL Server* → `(localdb)\MSSQLLocalDB` → `RotaDoSaborDB` → Tabelas.

### Zerar o banco (para a apresentação começar limpa)
`Drop-Database -Project RotaDoSabor.Infrastructure -StartupProject RotaDoSabor.Wpf` e rode o app de novo (ele recria tudo e repopula o seed).

## 4. Rodar em outro computador

1. Instale o VS 2026 com as duas cargas de trabalho do item 1.
2. Confirme o LocalDB com `sqllocaldb info`.
3. Abra a solução e pressione F5 — banco e dados iniciais são criados automaticamente.
4. Se o LocalDB tiver outro nome de instância, ajuste só o `appsettings.json`.

## 5. Dados de demonstração (seed)

- **6 restaurantes**: Smash Burguer, Pizza Prime, Rei do Temaki, Marmitaria da Vó, Açaí da Praia (começa **sem avaliações**), Burger Station.
- **32 produtos** (pratos e bebidas), com **itens indisponíveis** em todos os restaurantes principais.
- **Categorias**: Hambúrguer, Pizza, Marmita, Sushi, Açaí (+ Bebidas, usada só nos produtos).
- **Cupons**: `BEMVINDO10` (10%), `ROTA20` (20%), `SABOR50` (50%), `GRATIS100` (100% — mostra que o desconto não passa do total), `EXPIRADO15` (vencido), `INATIVO5` (desativado).
- **Nenhum usuário pré-cadastrado** (não há senha no código): crie o seu na tela *Cadastro*.
- **Pagamento simulado**: aprovado por padrão; **cartão terminado em `0000` é recusado** (para mostrar o status RECUSADO); **dinheiro** fica PENDENTE e é aprovado na entrega.
- **Acompanhamento**: o pedido avança sozinho (20 s em PENDENTE, 30 s EM_PREPARO, 30 s SAIU_PARA_ENTREGA — ajustável em `appsettings.json`, seção `SimulacaoPedido`). O botão **"Avançar status (demo)"** avança na hora.

## 6. Estrutura

```
RotaDoSabor/
├─ RotaDoSabor.slnx / RotaDoSabor.sln
├─ Directory.Build.props
├─ README.md
├─ docs/  (arquitetura, roteiro, validação, diagramas, sql)
└─ src/
   ├─ RotaDoSabor.Domain/          Entidades, enums, IPreparacao, regras centrais (sem dependências)
   ├─ RotaDoSabor.Application/     Serviços (casos de uso), DTOs, validações, interfaces de repositório,
   │                               pagamentos (Strategy), simulação de status, sessão
   ├─ RotaDoSabor.Infrastructure/  DbContext, configurações Fluent API, Migrations, Seed, repositórios EF,
   │                               HistoricoPedidoSqlRepository (ADO.NET), hash PBKDF2
   └─ RotaDoSabor.Wpf/             Views (XAML), Controllers, navegação, App/composition root, appsettings.json
```

Dependências: `Wpf → Application, Infrastructure, Domain` · `Infrastructure → Application, Domain` · `Application → Domain` · `Domain → (nada)`. Sem ciclos.

## 7. Funcionalidades novas pedidas pelo professor

| Funcionalidade | Onde está |
|---|---|
| Editar perfil | `PerfilView` → `PerfilController` → `PerfilService` → `Cliente.AtualizarPerfil` |
| Gerenciar cartões fictícios | `CartoesView` → `CartaoController` → `CartaoService` → `CartaoSimulado` |
| Cancelar pedido pendente | `AcompanhamentoView` → `PedidoController.CancelarAsync` → `PedidoService` → `Pedido.Cancelar` |
| Repetir pedido anterior | `HistoricoView`/`AcompanhamentoView` → `PedidoController.RepetirAsync` → `PedidoService.RepetirAsync` |
| Definir endereço padrão | `EnderecosView` → `EnderecoController.DefinirPadraoAsync` → `EnderecoService` → `Endereco.DefinirComoPadrao` |
