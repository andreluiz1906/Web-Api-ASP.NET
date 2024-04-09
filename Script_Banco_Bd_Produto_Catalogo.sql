-- --------------------------------------------------------
-- Servidor:                     127.0.0.1
-- Versão do servidor:           8.0.36 - MySQL Community Server - GPL
-- OS do Servidor:               Win64
-- HeidiSQL Versão:              12.6.0.6819
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Copiando estrutura do banco de dados para bd_produto_catalogo
DROP DATABASE IF EXISTS `bd_produto_catalogo`;
CREATE DATABASE IF NOT EXISTS `bd_produto_catalogo` /*!40100 DEFAULT CHARACTER SET utf8mb3 */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `bd_produto_catalogo`;

-- Copiando estrutura para tabela bd_produto_catalogo.categoria
DROP TABLE IF EXISTS `categoria`;
CREATE TABLE IF NOT EXISTS `categoria` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nome` varchar(80) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Id_usuario` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `nome_UNIQUE` (`Nome`),
  KEY `fk_Categoria_Usuario1_idx` (`Id_usuario`),
  CONSTRAINT `fk_Categoria_Usuario1` FOREIGN KEY (`Id_usuario`) REFERENCES `usuario` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Copiando dados para a tabela bd_produto_catalogo.categoria: ~2 rows (aproximadamente)
INSERT INTO `categoria` (`Id`, `Nome`, `Id_usuario`) VALUES
	(1, 'Saúde e bem-estar', 1),
	(2, 'Equipamentos de escritório', 1),
	(3, 'Informática', 1);

-- Copiando estrutura para tabela bd_produto_catalogo.permissao
DROP TABLE IF EXISTS `permissao`;
CREATE TABLE IF NOT EXISTS `permissao` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nome` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `nome_UNIQUE` (`Nome`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Copiando dados para a tabela bd_produto_catalogo.permissao: ~2 rows (aproximadamente)
INSERT INTO `permissao` (`Id`, `Nome`) VALUES
	(2, 'Administrador'),
	(1, 'Leitor');

-- Copiando estrutura para tabela bd_produto_catalogo.produto
DROP TABLE IF EXISTS `produto`;
CREATE TABLE IF NOT EXISTS `produto` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Nome` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Descricao` varchar(250) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci DEFAULT NULL,
  `Valor_venda` decimal(10,2) NOT NULL,
  `Valor_compra` decimal(10,2) NOT NULL,
  `data_cadastro` datetime NOT NULL,
  `Id_categoria` int NOT NULL,
  `Id_usuario` int NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Nome_UNIQUE` (`Nome`),
  KEY `fk_Produto_Categoria1_idx` (`Id_categoria`),
  KEY `fk_Produto_Usuario1_idx` (`Id_usuario`),
  CONSTRAINT `fk_Produto_Categoria1` FOREIGN KEY (`Id_categoria`) REFERENCES `categoria` (`Id`),
  CONSTRAINT `fk_Produto_Usuario1` FOREIGN KEY (`Id_usuario`) REFERENCES `usuario` (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Copiando dados para a tabela bd_produto_catalogo.produto: ~11 rows (aproximadamente)
INSERT INTO `produto` (`Id`, `Nome`, `Descricao`, `Valor_venda`, `Valor_compra`, `data_cadastro`, `Id_categoria`, `Id_usuario`) VALUES
	(1, 'Aparelho Medidor de Pressão', 'Aparelho para medir pressão arterial. Digital de pulso', 80.00, 60.00, '2024-04-05 23:31:28', 1, 1),
	(2, 'Termômetro Infravermelho', 'Termômetro Infravermelho Sem Contato', 150.00, 109.80, '2024-04-05 23:32:26', 1, 1),
	(3, 'Termômetro Digital', 'Aparelho medidor de febre, digital com beep', 35.90, 20.16, '2024-04-05 23:32:56', 1, 1),
	(4, 'Balança Digital Até 150kg', 'Balança Bioimpedância Digital Corporal Aplicativo Bluetooth', 57.00, 39.99, '2024-04-05 23:33:36', 1, 1),
	(5, 'Papel Sulfite 500 folhas', 'Papel Sulfite 500 Folhas A4', 33.60, 21.99, '2024-04-05 23:48:50', 2, 1),
	(6, 'Organizador de Papéis para mesa', 'Organizador de papéis para mesa de escritório 02 bandejas', 49.50, 30.00, '2024-04-05 23:49:38', 2, 1),
	(7, 'Calculadora de Mesa', 'Calculadora display 12 dígitos', 10.50, 5.00, '2024-04-05 23:50:23', 2, 1),
	(8, 'Bobina Térmica PDV 80x30', 'Bobina Térmica para PDV (Cupom Fiscal) cor amarelo', 80.00, 59.89, '2024-04-05 23:52:28', 3, 1),
	(9, 'Impressora Multifuncional HP Smart Tank 581', 'Impressora Multifuncional, impressão colorida com tanque de tinta', 789.00, 412.99, '2024-04-05 23:53:37', 3, 1),
	(10, 'Impressora de Etiquetas Elgin L42', 'Impressora Térmica Elgin L42 Pro ', 1157.00, 812.00, '2024-04-05 23:54:48', 3, 1),
	(11, 'Kit Teclado e Mouse Logitech', 'Kit com Teclado e Mouse sem fio Elgin, modelo MK220', 119.00, 89.99, '2024-04-05 23:55:33', 3, 1);

-- Copiando estrutura para tabela bd_produto_catalogo.usuario
DROP TABLE IF EXISTS `usuario`;
CREATE TABLE IF NOT EXISTS `usuario` (
  `id` int NOT NULL AUTO_INCREMENT,
  `Data_cadastro` datetime NOT NULL,
  `Apelido` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Email` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Senha` varchar(900) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci NOT NULL,
  `Id_permissao` int NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE KEY `Email_UNIQUE` (`Email`),
  UNIQUE KEY `Apelido_UNIQUE` (`Apelido`),
  KEY `fk_Usuario_Permissao_idx` (`Id_permissao`),
  CONSTRAINT `fk_Usuario_Permissao` FOREIGN KEY (`Id_permissao`) REFERENCES `permissao` (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Copiando dados para a tabela bd_produto_catalogo.usuario: ~2 rows (aproximadamente)
INSERT INTO `usuario` (`id`, `Data_cadastro`, `Apelido`, `Email`, `Senha`, `Id_permissao`) VALUES
	(1, '2024-04-05 23:30:47', 'André', 'andre.luiz@teste.com', '2fdf554dc3ee2485c647b1ef5683dd7a', 2),
	(2, '2024-04-05 23:56:19', 'André Luiz', 'andre.cunha@teste.com', '4d47635546c6d6947e87ca06ee466e21', 1);

-- Copiando estrutura para trigger bd_produto_catalogo.produto_bi_validar_data_cadastro
DROP TRIGGER IF EXISTS `produto_bi_validar_data_cadastro`;
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `produto_bi_validar_data_cadastro` BEFORE INSERT ON `produto` FOR EACH ROW BEGIN
    IF NEW.data_cadastro IS NULL THEN
        SET NEW.data_cadastro = CURRENT_TIMESTAMP;
    END IF;	
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Copiando estrutura para trigger bd_produto_catalogo.produto_bi_validar_valor_compra
DROP TRIGGER IF EXISTS `produto_bi_validar_valor_compra`;
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `produto_bi_validar_valor_compra` BEFORE INSERT ON `produto` FOR EACH ROW BEGIN
	IF NEW.Valor_compra <= 0.00 THEN
		SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'O valor de compra deve ser maior que zero.';
	END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Copiando estrutura para trigger bd_produto_catalogo.produto_bi_validar_valor_venda
DROP TRIGGER IF EXISTS `produto_bi_validar_valor_venda`;
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `produto_bi_validar_valor_venda` BEFORE INSERT ON `produto` FOR EACH ROW BEGIN
	IF NEW.Valor_venda <= 0.00 THEN
		SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'O valor de venda deve ser maior que zero.';
	END IF;
	
	IF NEW.Valor_venda <= NEW.VALOR_compra THEN
		SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'O valor de venda deve ser maior que o valor de compra.';
	END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Copiando estrutura para trigger bd_produto_catalogo.usuario_bi_validar_data_cadastro
DROP TRIGGER IF EXISTS `usuario_bi_validar_data_cadastro`;
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `usuario_bi_validar_data_cadastro` BEFORE INSERT ON `usuario` FOR EACH ROW BEGIN
    IF NEW.Data_cadastro IS NULL THEN
        SET NEW.Data_cadastro = CURRENT_TIMESTAMP;
    END IF;	
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;
-- Copiando estrutura para trigger bd_produto_catalogo.produto_bu_validar_valor_compra
DROP TRIGGER IF EXISTS `produto_bu_validar_valor_compra`;
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `produto_bu_validar_valor_compra` BEFORE UPDATE ON `produto` FOR EACH ROW BEGIN
	IF NEW.Valor_compra <= 0.00 THEN
		SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'O valor de compra deve ser maior que zero.';
	END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;

-- Copiando estrutura para trigger bd_produto_catalogo.produto_bu_validar_valor_venda
DROP TRIGGER IF EXISTS `produto_bu_validar_valor_venda`;
SET @OLDTMP_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';
DELIMITER //
CREATE TRIGGER `produto_bu_validar_valor_venda` BEFORE UPDATE ON `produto` FOR EACH ROW BEGIN
	IF NEW.Valor_venda <= 0.00 THEN
		SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'O valor de venda deve ser maior que zero.';
	END IF;
	
	IF NEW.Valor_venda <= NEW.VALOR_compra THEN
		SIGNAL SQLSTATE '45000'
      SET MESSAGE_TEXT = 'O valor de venda deve ser maior que o valor de compra.';
	END IF;
END//
DELIMITER ;
SET SQL_MODE=@OLDTMP_SQL_MODE;
/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
