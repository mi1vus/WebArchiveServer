CREATE SCHEMA IF NOT EXISTS `terminal_archive` ;

CREATE TABLE IF NOT EXISTS `terminal_archive`.`terminal_groups` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(150) NOT NULL,
  PRIMARY KEY (`id`)
);

CREATE TABLE IF NOT EXISTS `terminal_archive`.`terminals` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `hasp_id` VARCHAR(20) NOT NULL,
  `id_group` INT NOT NULL,
  `address` VARCHAR(20) NOT NULL,
  `name` VARCHAR(150) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_hasp_UNIQUE` (`id_hasp` ASC),
  FOREIGN KEY (`id_group`) REFERENCES `terminal_archive`.`terminal_groups`(`id`)
);  

CREATE TABLE IF NOT EXISTS `terminal_archive`.`parameters` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `path` VARCHAR(150) NOT NULL,
  `name` VARCHAR(150) NOT NULL,
  `description` VARCHAR(150) NOT NULL,
  PRIMARY KEY (`id`)
);

CREATE TABLE IF NOT EXISTS `terminal_archive`.`terminal_parameters` (
  `id_terminal` INT NOT NULL,
  `id_parameter` INT NOT NULL,
  `value` VARCHAR(150) NOT NULL,
  `save_date` TIMESTAMP NOT NULL DEFAULT '1970-01-02 00:00:00',
  `last_edit_date` TIMESTAMP ON UPDATE CURRENT_TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`id_terminal`,`id_parameter`),
  FOREIGN KEY (`id_terminal`) REFERENCES `terminal_archive`.`terminals`(`id`),
  FOREIGN KEY (`id_parameter`) REFERENCES `terminal_archive`.`parameters`(`id`)
);

CREATE TABLE IF NOT EXISTS `terminal_archive`.`order_fuels` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(150) NOT NULL,
  PRIMARY KEY (`id`)
);

CREATE TABLE IF NOT EXISTS `terminal_archive`.`order_payment_types` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(150) NOT NULL,
  PRIMARY KEY (`id`)
);

CREATE TABLE IF NOT EXISTS `terminal_archive`.`order_states` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(150) NOT NULL,
  PRIMARY KEY (`id`)
);

CREATE TABLE IF NOT EXISTS `terminal_archive`.`orders` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `id_terminal` INT NOT NULL,
  `RNN` VARCHAR(20) NOT NULL,
  `id_fuel` INT NOT NULL,
  `id_pump` INT NOT NULL,
  `id_payment` INT NOT NULL,
  `id_state` INT NOT NULL,
  `pre_price` DECIMAL(5,2) UNSIGNED NOT NULL,
  `price` DECIMAL(5,2) UNSIGNED NOT NULL,
  `pre_quantity` DECIMAL(5,2) UNSIGNED NOT NULL,
  `quantity` DECIMAL(5,2) UNSIGNED NOT NULL,
  `pre_summ` DECIMAL(5,2) UNSIGNED NOT NULL,
  `summ` DECIMAL(5,2) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `terminal_RNN_UNIQUE` (`id_terminal` ASC,`RNN` ASC),
  FOREIGN KEY (`id_terminal`) REFERENCES `terminal_archive`.`terminals`(`id`),
  FOREIGN KEY (`id_fuel`) REFERENCES `terminal_archive`.`order_fuels`(`id`),
  FOREIGN KEY (`id_payment`) REFERENCES `terminal_archive`.`order_payment_types`(`id`),
  FOREIGN KEY (`id_state`) REFERENCES `terminal_archive`.`order_states`(`id`)
);  

CREATE TABLE IF NOT EXISTS `terminal_archive`.`details` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `description` VARCHAR(150) NOT NULL,
  PRIMARY KEY (`id`)
);

CREATE TABLE IF NOT EXISTS `terminal_archive`.`order_details` (
  `id` INT NOT NULL AUTO_INCREMENT,
  `id_order` INT NOT NULL,
  `id_detail` INT NOT NULL,
  `value` VARCHAR(150) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `order_detail_UNIQUE` (`id_order` ASC,`id_detail` ASC),
  FOREIGN KEY (`id_order`) REFERENCES `terminal_archive`.`orders`(`id`),
  FOREIGN KEY (`id_detail`) REFERENCES `terminal_archive`.`details`(`id`)
);

INSERT INTO `terminal_archive`.`terminal_groups` (`name`) VALUES 
('Тестовая');

INSERT INTO `terminal_archive`.`terminals` (`id_hasp`, `id_group`, `address`, `name`) VALUES 
('1', '1', 'Исследователей, 15', 'Тестовый');

INSERT INTO `terminal_archive`.`order_fuels` (`name`) VALUES 
('-'),
('95'),
('98'),
('80'),
('-'),
('92');

INSERT INTO `terminal_archive`.`order_payment_types` (`name`) VALUES 
('наличные'), 
('карты'), 
('топливные карты');

INSERT INTO `terminal_archive`.`order_payment_types` (`name`) VALUES 
('создан'),
('готов к оплате'),
('оплачен'),
('установлен на ТРК'),
('выполнен'),
('выполнен и пересчитан');

SELECT t.`id`, t.id_hasp,  g.`name` AS `группа` ,  t.`address` , t.`name`, p.id AS `id параметра`, p.path AS `путь параметра`, p.name AS `имя параметра` ,
tp.value AS `значение параметра`, tp.last_edit_date, tp.save_date
FROM terminal_archive.terminals AS t
LEFT JOIN terminal_archive.terminal_groups AS g ON t.id_group = g.id
LEFT JOIN terminal_archive.terminal_parameters AS tp ON t.id = tp.id_terminal
LEFT JOIN terminal_archive.parameters AS p ON tp.id_parameter = p.id
WHERE tp.save_date < tp.last_edit_date
ORDER BY t.id asc;

SELECT o.`id`, s.name AS `состояние`,  t.`name` AS `терминал` ,  `RNN` , d.description AS `доп. параметр`, od.value AS `значение`,
f.`name` AS `топливо` , p.`name` AS `оплата` , o.id_pump AS `колонка`,  
`pre_price` ,  `price` ,  `pre_quantity` ,  `quantity` ,  `pre_summ` ,  `summ` FROM terminal_archive.orders AS o
LEFT JOIN terminal_archive.order_fuels AS f ON o.id_fuel = f.id
LEFT JOIN terminal_archive.order_payment_types AS p ON o.id_payment = p.id
LEFT JOIN terminal_archive.terminals AS t ON o.id_terminal = t.id
LEFT JOIN terminal_archive.order_states AS s ON o.id_state = s.id
LEFT JOIN terminal_archive.order_details AS od ON o.id = od.id_order
LEFT JOIN terminal_archive.details AS d ON od.id_detail = d.id
ORDER BY o.id asc;
