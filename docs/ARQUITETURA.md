# Arquitetura do Rota do Sabor

## 1. Visão geral das camadas

```
┌──────────────────────────── RotaDoSabor.Wpf (Apresentação) ─────────────────────────────┐
│ Views (XAML + code-behind fino)  →  Controllers  →  INavegador / IDialogoService         │
│ App.xaml.cs + Configuracao/CompositionRoot.cs (injeção de dependência, appsettings.json) │
└───────────────┬──────────────────────────────────────────────────────────┬───────────────┘
                │ usa interfaces                                            │ registra
┌───────────────▼──────────── RotaDoSabor.Application ──────────┐  ┌────────▼──── RotaDoSabor.Infrastructure ────┐
│ Serviços (casos de uso), DTOs, validações, sessão,             │◄─┤ RotaDoSaborDbContext, Fluent API, Migrations │
│ Strategy de pagamento, simulador de status,                    │  │ Seed, repositórios EF Core,                  │
│ INTERFACES de persistência (IPedidoRepository...)              │  │ HistoricoPedidoSqlRepository (ADO.NET),      │
└───────────────┬────────────────────────────────────────────────┘  │ Pbkdf2PasswordHasher                         │
                │                                                     └──────────────┬──────────────────────────────┘
┌───────────────▼──────────── RotaDoSabor.Domain ────────────────────────────────────▼──────┐
│ Entidades (Cliente, Restaurante, Produto, Prato, Bebida, Pedido, ItemPedido, Endereco,     │
│ Pagamento, Cupom, Avaliacao, CartaoSimulado, Categoria, Carrinho, ItemCarrinho),           │
│ enums (StatusPedido, StatusPagamento, FormaPagamento, TipoCartao), IPreparacao, regras     │
└────────────────────────────────────────────────────────────────────────────────────────────┘
```

A Infrastructure **implementa** as interfaces declaradas na Application (inversão de dependência). A Application nunca referencia EF Core nem WPF. O Domain não referencia ninguém.

## 2. MVC no WPF

| Papel | Onde | Exemplo |
|---|---|---|
| **Model** | Domain (entidades e regras) + Application (serviços e DTOs) | `Pedido.Cancelar()`, `CheckoutService.FinalizarAsync()`, `DetalhePedidoDto` |
| **View** | `src/RotaDoSabor.Wpf/Views/*.xaml` | `CarrinhoView.xaml` (DataGrid, botões) |
| **Controller** | `src/RotaDoSabor.Wpf/Controllers/*.cs` | `CarrinhoController` recebe a ação "Aumentar", chama `ICarrinhoService` e trata erros |

Regras seguidas:
- O **code-behind** só lê/escreve controles e encaminha eventos ao controller (ex.: `BtnAumentar_Click → _controller.Aumentar(id)`).
- **Nenhum SQL e nenhuma regra de negócio** nas Views.
- Controllers não conhecem classes WPF: dependem de `INavegador` e `IDialogoService` (implementados por `NavegadorWpf` e `DialogoServiceWpf`). Isso permitiu compilar/testar a lógica sem a interface gráfica.
- `ControllerBase.ExecutarAsync` centraliza o tratamento de exceções: `RegraDeNegocioException` vira aviso; falha técnica (banco fora do ar) vira mensagem de erro amigável (`MensagensErro`).

### Fluxo de uma ação (exemplo: confirmar pedido)
`FinalizacaoView.BtnConfirmar_Click` → `FinalizacaoController.ConfirmarAsync` → `ICheckoutService.FinalizarAsync` → (`Pedido`, `Pagamento`, `IProcessadorPagamento` → `PixPagamentoStrategy`) → `IPedidoRepository.AdicionarAsync` (EF Core grava Pedido + Itens + Pagamento numa transação) → controller navega para `Tela.Acompanhamento`.

## 3. SOLID com classes reais

| Princípio | Como foi aplicado | Arquivos |
|---|---|---|
| **S — Responsabilidade única** | Cada controller atende uma tela; cada serviço um caso de uso; cada repositório um agregado. Validação de formulário separada em validadores. | `CarrinhoController`, `CheckoutService`, `PedidoService`, `CupomService`, `PedidoRepository`, `ValidadorCartao` |
| **O — Aberto/fechado** | Pagamento por **Strategy**: nova forma de pagamento = nova classe `IPagamentoStrategy` + 1 linha no DI. `ProcessadorPagamentoSimulado` não muda. | `Application/Pagamentos/*`, `DependencyInjection.cs` |
| **L — Substituição de Liskov** | `Prato` e `Bebida` substituem `Produto` em qualquer lugar (carrinho, pedido, cardápio, TPH). `CartaoCreditoPagamentoStrategy` e `CartaoDebitoPagamentoStrategy` substituem `CartaoPagamentoStrategyBase`. | `Domain/Entidades/Produto.cs`, `Prato.cs`, `Bebida.cs` |
| **I — Segregação de interfaces** | `IPreparacao` só é implementada por `Prato` (bebida não é obrigada a "preparar"). Interfaces pequenas por responsabilidade: `IHistoricoPedidoConsulta` (só leitura SQL), `ICupomRepository`, `IPasswordHasher`, `IRelogio`. | `Domain/Interfaces/IPreparacao.cs`, `Application/Persistencia/*` |
| **D — Inversão de dependência** | Serviços dependem de interfaces (`IPedidoRepository`, `IProcessadorPagamento`, `IRelogio`). As implementações concretas são ligadas apenas no composition root. | `Wpf/Configuracao/CompositionRoot.cs`, `Application/DependencyInjection.cs`, `Infrastructure/DependencyInjection.cs` |

## 4. Clean Code — onde aparece

- **Nomes claros em português** de domínio: `PodeSerCancelado()`, `CalcularDesconto()`, `ObterPrevisaoEntrega()`.
- **Métodos curtos**; regras complexas quebradas em métodos privados (`CheckoutService.ObterCartaoParaPagamentoAsync`, `EnderecoService.MarcarComoUnicoPadrao`).
- **Sem duplicação**: `Guarda` (validações das entidades), `Mapeamentos` (entidade → DTO), `CartaoPagamentoStrategyBase`, `ControllerBase`.
- **Validações** em dois níveis: formulário (`Application/Validacoes`) e invariantes do domínio (construtores e métodos das entidades).
- **Sem SQL nas Views**; SQL manual isolado em `HistoricoPedidoSqlRepository`.
- **Sem senhas fixas**: LocalDB com autenticação do Windows; nenhum usuário com senha no seed.
- **Tratamento de exceções** centralizado (`ControllerBase`, `MensagensErro`, `App.InicializarBancoAsync`).
- **async/await** em todo acesso a banco.
- **decimal** para dinheiro (`HasPrecision(10,2)` no banco).
- **Constantes** em vez de números mágicos (`Carrinho.QuantidadeMaximaPorItem`, `Avaliacao.NotaMaxima`).
- Comentários só onde explicam um "porquê" (ex.: concorrência no `PedidoRepository`).

## 5. EF Core, Code First e Migrations

- `RotaDoSaborDbContext` com um `IEntityTypeConfiguration<T>` por entidade em `Infrastructure/Persistencia/Configuracoes` (Fluent API), aplicados com `ApplyConfigurationsFromAssembly`.
- Banco gerado a partir das classes (**Code First**); migration `InitialCreate` em `Infrastructure/Migrations`.
- `InicializadorBanco` executa `MigrateAsync()` e o seed (`DadosIniciais`) na inicialização. O seed só insere se a tabela estiver vazia (**sem duplicação**).
- Repositórios usam `IDbContextFactory` (um contexto curto por operação — recomendação da Microsoft para apps desktop) e `AsNoTracking()` nas leituras.
- Enums gravados como texto (`HasConversion<string>()`) — o banco fica legível (`Pendente`, `Aprovado`...).
- `Pedido.RowVersion` (`rowversion`) dá **concorrência otimista**: se a simulação e o cancelamento mexerem no mesmo pedido ao mesmo tempo, um deles é rejeitado em vez de sobrescrever o outro.
- `IDesignTimeDbContextFactory` (`RotaDoSaborDbContextFactory`) permite usar `Add-Migration`/`Update-Database`.

### Relacionamentos (Fluent API)

| Relacionamento | Configuração | Exclusão |
|---|---|---|
| Cliente 1—N Endereco | `EnderecoConfiguration` | Cascade |
| Cliente 1—N CartaoSimulado | `CartaoSimuladoConfiguration` | Cascade |
| Cliente 1—N Pedido | `PedidoConfiguration` | **Restrict** (preserva histórico) |
| Restaurante 1—N Produto | `ProdutoConfiguration` | Restrict |
| Restaurante 1—N Pedido | `PedidoConfiguration` | Restrict |
| Endereco 1—N Pedido | `PedidoConfiguration` | Restrict |
| Cupom 0..1—N Pedido | `PedidoConfiguration` | Restrict |
| Pedido 1—N ItemPedido (**composição**) | `PedidoConfiguration` | Cascade |
| Produto 1—N ItemPedido | `ItemPedidoConfiguration` | Restrict |
| **Pedido N—N Produto via ItemPedido** | tabela associativa `ItensPedido` (com quantidade e preço) | — |
| Pedido 1—1 Pagamento | `HasOne/WithOne` + índice único `Pagamentos.PedidoId` | Cascade |
| Pedido 1—0..1 Avaliacao | índice único `Avaliacoes.PedidoId` | Restrict |
| CartaoSimulado 0..1—N Pagamento | `PagamentoConfiguration` | Restrict |
| Categoria 1—N Restaurante / Produto | | Restrict |
| **Herança Produto → Prato/Bebida** | **TPH**, discriminador `TipoProduto` | — |

**Múltiplos caminhos de cascata**: o SQL Server recusa `Cliente→Endereco→Pedido` e `Cliente→Pedido` ambos em cascata. Por isso tudo que aponta para `Pedido`/`Avaliacao` usa Restrict; cascata só na composição (itens e pagamento do pedido).

**Preservação de histórico**: endereço ou cartão já usado em pedido não é apagado fisicamente — é inativado (`Ativo = false`). Editar um endereço usado cria uma nova versão e inativa a antiga. O nome e o preço do produto são copiados para `ItemPedido` no momento da compra.

Índices únicos: `Clientes.Email`, `Clientes.NomeUsuario`, `Cupons.Codigo`, `Categorias.Nome`, `CartoesSimulados.IdentificadorFicticio`, `Pagamentos.PedidoId`, `Avaliacoes.PedidoId`. Restrição CHECK: `Avaliacoes.Nota BETWEEN 1 AND 5`.

## 6. Funcionalidade com SQL manual (ADO.NET)

- **Funcionalidade**: Histórico de pedidos do cliente autenticado (tela *Histórico de pedidos*).
- **Arquivo**: `src/RotaDoSabor.Infrastructure/Repositorios/Sql/HistoricoPedidoSqlRepository.cs` (cópia da consulta em `docs/sql/consulta-historico-pedidos.sql`).
- `SqlConnection` + `SqlCommand` + `SqlDataReader`, tudo assíncrono.
- Recebe o Id do cliente logado e usa `@ClienteId` como **SqlParameter** (sem concatenação → sem SQL Injection).
- `INNER JOIN Restaurantes` para o nome; `LEFT JOIN Avaliacoes` para saber se já foi avaliado.
- `ORDER BY p.DataHora DESC` (mais recente primeiro).
- Exposta pela interface `IHistoricoPedidoConsulta` (Application), consumida por `HistoricoService`.

## 7. Segurança

- Senha: **PBKDF2-SHA256**, salt aleatório de 16 bytes, 100.000 iterações, comparação em tempo constante (`Pbkdf2PasswordHasher`). Login com mensagem única para usuário inexistente ou senha errada.
- Cartões: só `IdentificadorFicticio`, `Bandeira`, `UltimosDigitos`, `Tipo`, `Apelido`. **Número completo, nome impresso, validade e CVV não existem no banco** (são validados e descartados; a tela limpa os campos após salvar).
- Pagamentos 100% simulados (`ProcessadorPagamentoSimulado`), sem integração externa.

## 8. Simulação de acompanhamento

`SimuladorStatusPedidoService` (Application/Simulacao):
- Laço em segundo plano (`PeriodicTimer`) iniciado no `App.OnStartup`, verificando a cada 5 s.
- Avança pedidos cujo tempo no status atual passou do configurado (`appsettings.json → SimulacaoPedido`).
- Pedidos **CANCELADOS** (e ENTREGUES) não são carregados nem avançam.
- Cada etapa é **gravada no banco** (`DataAtualizacaoStatus`), então fechar e abrir o app não perde o estado.
- **Testável sem esperar**: usa `IRelogio` (relógio injetável) e o botão "Avançar status (demo)" chama `AvancarManualmenteAsync`.
- Tempo estimado = maior `TempoPreparo` entre os itens que implementam `IPreparacao` + `TempoEntregaMinutos` do restaurante.

## 9. Diferenças em relação aos diagramas originais (e por quê)

| Original | Implementado | Motivo |
|---|---|---|
| Endereco 1—1 Pedido | Endereco 1—N Pedido | o mesmo endereço é usado em vários pedidos |
| `IPreparacao.preparar(): void` | `Preparar(): string` + `TempoPreparo` | um método `void` sem efeito não teria utilidade; o tempo é usado no cálculo da estimativa |
| `forma : String` em Pagamento | enum `FormaPagamento` | evita valores inválidos |
| StatusPagamento com 3 valores | + `Estornado` | cancelamento de pedido já pago (Pix/cartão) |
| enums `PENDENTE`, `EM_PREPARO`... | `Pendente`, `EmPreparo`... (exibidos como `PENDENTE`, `EM_PREPARO` em `EnumTextos`) | convenção de nomes do C# |
| — | `Categoria`, `Cupom`, `Avaliacao`, `CartaoSimulado`, `Carrinho`, `ItemCarrinho`, `TipoCartao` | requisitos do MVP e novas funcionalidades |

Os diagramas atualizados estão em `docs/diagramas/`.

## 10. Mapa requisito → código

| Requisito | Arquivo(s) principal(is) |
|---|---|
| Cadastro / login por e-mail ou usuário / logout | `AutenticacaoService`, `ValidadorCadastroCliente`, `LoginView`, `CadastroView` |
| Restaurantes, busca, filtro por categoria, média | `CatalogoService`, `RestauranteRepository`, `HomeView`, `RestaurantesView` |
| Cardápio (Prato/Bebida, disponibilidade) | `CardapioView`, `ProdutoRepository`, `Prato`, `Bebida` |
| Carrinho (1 restaurante, quantidades, confirmação de troca) | `Carrinho`, `CarrinhoService`, `CardapioController.AdicionarAsync` |
| Endereços (CRUD, padrão, seleção) | `EnderecoService`, `EnderecosView`, `FinalizacaoView` |
| Pagamento simulado | `Application/Pagamentos/*`, `Pagamento` |
| Cupons | `Cupom`, `CupomService`, `Pedido.AplicarCupom` |
| Pedido (finalizar, cancelar, repetir, detalhes) | `CheckoutService`, `PedidoService`, `AcompanhamentoView` |
| Acompanhamento | `SimuladorStatusPedidoService` |
| Histórico (SQL manual) | `HistoricoPedidoSqlRepository`, `HistoricoView` |
| Avaliação | `AvaliacaoService`, `Restaurante.RegistrarAvaliacao`, `AvaliacaoRepository`, `AvaliacaoView` |
| Perfil | `PerfilService`, `PerfilView` |
