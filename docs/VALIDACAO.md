# Validação — o que foi verificado e o que falta testar no Windows

Este projeto foi escrito em um ambiente Linux com o **.NET SDK 10.0.114**, sem acesso ao NuGet.org e sem Windows. Por isso **não foi possível executar o aplicativo WPF nem o SQL Server LocalDB aqui**. Abaixo está, de forma transparente, o que foi verificado de verdade e o que precisa ser conferido no Windows.

## ✔ Verificado neste ambiente

| Item | Como |
|---|---|
| `RotaDoSabor.Domain` compila no .NET 10 | `dotnet build` real — 0 erros, 0 avisos |
| `RotaDoSabor.Application` compila no .NET 10 | `dotnet build` real — 0 erros |
| Código C# da Infrastructure (exceto a pasta Migrations) e do WPF (controllers, navegação, code-behind, App, CompositionRoot) | compilado junto com o Domain/Application reais, usando **stubs** das APIs do EF Core, SqlClient e WPF (os pacotes reais não puderam ser baixados). Detecta erros de tipo, nomes e nulabilidade do nosso código, mas não garante 100% as assinaturas das bibliotecas |
| XAML | checagem estática: XML válido, `x:Class` ↔ code-behind, todo `Click`/`Checked`/`SelectionChanged` tem método correspondente, todo `StaticResource` existe no `App.xaml`, sem `DisplayMemberPath` + `ItemTemplate` no mesmo controle |
| Regras de negócio (81 cenários) | console de testes com repositórios em memória chamando os **serviços reais** da Application e o hash PBKDF2 real: cadastro/validações/duplicidade, login por e-mail e usuário, logout, filtros, cardápio, cálculo do carrinho (2×29,90 + 6,50 = 66,30), bloqueio de quantidades, troca de restaurante com confirmação, endereços e padrão único, cartões (só últimos 4 dígitos/bandeira), cupons (válido, vencido, inativo, 100% limitado ao total), checkout (totais, preço no momento da compra, Pix aprovado, cartão final 0000 recusado, dinheiro pendente→aprovado na entrega), cancelamento só PENDENTE com estorno, simulação de status com relógio falso (cancelado não avança), avaliação (só entregue, 1–5, uma vez, média atualizada), histórico por cliente e ordem decrescente, repetir pedido (ignora indisponível, usa preço atual), editar perfil, exclusão com preservação de histórico. **Resultado: 81 ok, 0 falhas.** |
| Migration `InitialCreate`, `.Designer` e `ModelSnapshot` | gerados a partir de uma especificação única espelhando as configurações Fluent API (mesmos tamanhos, precisões, FKs, índices, delete behaviors, TPH, rowversion, CHECK). **Não foram executados contra um SQL Server.** |
| Diagramas | Mermaid renderizado em PNG (`docs/diagramas/*.png`); PlantUML escrito à mão (não renderizado aqui) |

## ⚠ Pendente de teste no Windows (faça nesta ordem)

1. **Compilar a solução** no VS 2026 (restore NuGet + compilação do XAML).
2. **F5** → banco `RotaDoSaborDB` criado e seed aplicado. Se a migration falhar, regenere-a (README, seção 3) — o código do modelo continua o mesmo.
3. `dotnet ef migrations has-pending-model-changes -p src/RotaDoSabor.Infrastructure -s src/RotaDoSabor.Wpf` → deve dizer que não há mudanças pendentes. Se apontar diferença, regenere a migration (README).
4. Fechar e abrir o app → conferir que o seed **não duplicou** (6 restaurantes, 32 produtos, 6 cupons).
5. Cadastro/login (por e-mail e usuário) e conferir a coluna `SenhaHash` (começa com `PBKDF2-SHA256$`).
6. Fluxo completo de pedido → conferir tabelas `Pedidos`, `ItensPedido`, `Pagamentos` (valores e status).
7. Histórico (SQL manual) listando do mais recente para o mais antigo.
8. Simulação: status mudando sozinho e persistido (fechar/abrir o app no meio).
9. Cancelar pedido pendente enquanto a simulação está ligada (concorrência via `RowVersion`).
10. Avaliação → `Restaurantes.AvaliacaoMedia`/`QuantidadeAvaliacoes` atualizados.
11. Tabela `CartoesSimulados` sem número completo, validade ou CVV.
12. Parar o LocalDB (`sqllocaldb stop MSSQLLocalDB`) com o app aberto e clicar em algo → mensagem amigável, sem travar.
13. Visual das telas (alinhamentos/tamanhos podem precisar de pequenos ajustes de margem).

## Checklist de requisitos

- [x] C#, .NET 10, WPF/XAML com controles nativos, sem bibliotecas visuais externas
- [x] SQL Server LocalDB, EF Core 10, Code First, Migrations, Fluent API
- [x] MVC em camadas (Domain, Application, Infrastructure, Wpf) sem dependência circular
- [x] Injeção de dependência (Microsoft.Extensions.DependencyInjection)
- [x] SQL manual com ADO.NET parametrizado (histórico)
- [x] Todas as funcionalidades do MVP e as 5 novas funcionalidades implementadas no código
- [x] Seed: 6 restaurantes, 32 produtos (pratos/bebidas, disponíveis e indisponíveis), 5 categorias exigidas, cupons válidos e vencidos, sem usuários com senha
- [x] Diagramas de classes e de casos de uso atualizados
- [ ] Execução real no Windows (itens da seção anterior)
