-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Хост: 127.0.0.1
-- Час створення: Чрв 04 2025 р., 19:43
-- Версія сервера: 10.4.32-MariaDB
-- Версія PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База даних: `shop`
--

-- --------------------------------------------------------

--
-- Структура таблиці `baskets`
--

CREATE TABLE `baskets` (
  `Id` int(11) NOT NULL,
  `Date` datetime(6) NOT NULL,
  `TotalPrice` double NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп даних таблиці `baskets`
--

INSERT INTO `baskets` (`Id`, `Date`, `TotalPrice`) VALUES
(2, '2025-06-04 20:40:14.907404', 14);

-- --------------------------------------------------------

--
-- Структура таблиці `products`
--

CREATE TABLE `products` (
  `Id` int(11) NOT NULL,
  `Name` longtext NOT NULL,
  `ProductTypeId` int(11) NOT NULL,
  `Price` double NOT NULL,
  `inStock` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп даних таблиці `products`
--

INSERT INTO `products` (`Id`, `Name`, `ProductTypeId`, `Price`, `inStock`) VALUES
(2, 'Gvozdi', 1, 7, 997);

-- --------------------------------------------------------

--
-- Структура таблиці `productsinbaskets`
--

CREATE TABLE `productsinbaskets` (
  `Id` int(11) NOT NULL,
  `productId` int(11) NOT NULL,
  `BasketId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп даних таблиці `productsinbaskets`
--

INSERT INTO `productsinbaskets` (`Id`, `productId`, `BasketId`) VALUES
(2, 2, 2),
(3, 2, 2);

-- --------------------------------------------------------

--
-- Структура таблиці `producttypes`
--

CREATE TABLE `producttypes` (
  `Id` int(11) NOT NULL,
  `Name` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп даних таблиці `producttypes`
--

INSERT INTO `producttypes` (`Id`, `Name`) VALUES
(1, 'Gvozdi');

-- --------------------------------------------------------

--
-- Структура таблиці `__efmigrationshistory`
--

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Дамп даних таблиці `__efmigrationshistory`
--

INSERT INTO `__efmigrationshistory` (`MigrationId`, `ProductVersion`) VALUES
('20250604162314_HelloWorld', '7.0.20'),
('20250604162413_HelloWorld2', '7.0.20'),
('20250604163443_AddProductsInBasket', '7.0.20');

--
-- Індекси збережених таблиць
--

--
-- Індекси таблиці `baskets`
--
ALTER TABLE `baskets`
  ADD PRIMARY KEY (`Id`);

--
-- Індекси таблиці `products`
--
ALTER TABLE `products`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_Products_ProductTypeId` (`ProductTypeId`);

--
-- Індекси таблиці `productsinbaskets`
--
ALTER TABLE `productsinbaskets`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `IX_ProductsInBaskets_BasketId` (`BasketId`),
  ADD KEY `IX_ProductsInBaskets_productId` (`productId`);

--
-- Індекси таблиці `producttypes`
--
ALTER TABLE `producttypes`
  ADD PRIMARY KEY (`Id`);

--
-- Індекси таблиці `__efmigrationshistory`
--
ALTER TABLE `__efmigrationshistory`
  ADD PRIMARY KEY (`MigrationId`);

--
-- AUTO_INCREMENT для збережених таблиць
--

--
-- AUTO_INCREMENT для таблиці `baskets`
--
ALTER TABLE `baskets`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT для таблиці `products`
--
ALTER TABLE `products`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT для таблиці `productsinbaskets`
--
ALTER TABLE `productsinbaskets`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;

--
-- AUTO_INCREMENT для таблиці `producttypes`
--
ALTER TABLE `producttypes`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;

--
-- Обмеження зовнішнього ключа збережених таблиць
--

--
-- Обмеження зовнішнього ключа таблиці `products`
--
ALTER TABLE `products`
  ADD CONSTRAINT `FK_Products_ProductTypes_ProductTypeId` FOREIGN KEY (`ProductTypeId`) REFERENCES `producttypes` (`Id`) ON DELETE CASCADE;

--
-- Обмеження зовнішнього ключа таблиці `productsinbaskets`
--
ALTER TABLE `productsinbaskets`
  ADD CONSTRAINT `FK_ProductsInBaskets_Baskets_BasketId` FOREIGN KEY (`BasketId`) REFERENCES `baskets` (`Id`) ON DELETE CASCADE,
  ADD CONSTRAINT `FK_ProductsInBaskets_Products_productId` FOREIGN KEY (`productId`) REFERENCES `products` (`Id`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
