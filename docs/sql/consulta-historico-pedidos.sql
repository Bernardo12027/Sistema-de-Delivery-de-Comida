-- =====================================================================
-- SQL MANUAL usado pela funcionalidade "Histórico de pedidos".
-- Executado via ADO.NET em:
--   src/RotaDoSabor.Infrastructure/Repositorios/Sql/HistoricoPedidoSqlRepository.cs
-- O valor de @ClienteId vem do cliente autenticado e é enviado como
-- PARÂMETRO (SqlParameter) — nunca concatenado no texto da consulta.
-- =====================================================================
SELECT  p.Id            AS PedidoId,
        p.DataHora      AS DataHora,
        r.Nome          AS NomeRestaurante,
        p.ValorTotal    AS ValorTotal,
        p.Status        AS Status,
        CAST(CASE WHEN a.Id IS NULL THEN 0 ELSE 1 END AS bit) AS JaAvaliado
FROM    dbo.Pedidos AS p
        INNER JOIN dbo.Restaurantes AS r ON r.Id = p.RestauranteId
        LEFT JOIN  dbo.Avaliacoes   AS a ON a.PedidoId = p.Id
WHERE   p.ClienteId = @ClienteId
ORDER BY p.DataHora DESC, p.Id DESC;

-- Para testar no SQL Server Object Explorer do Visual Studio:
-- DECLARE @ClienteId int = 1;  (depois execute o SELECT acima)
