CREATE SCHEMA `products` ;

CREATE TABLE `products`.`products` (
  `id` INT NOT NULL,
  `name` VARCHAR(150) NOT NULL,
  `category` VARCHAR(150) NOT NULL,
  `price` DECIMAL(5,2) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC))
COMMENT = 'items of things';

INSERT INTO `products`.`products` (`id`, `name`, `category`, `price`) VALUES ('1', 'Tomato Soup', 'Groceries', '1');
INSERT INTO `products`.`products` (`id`, `name`, `category`, `price`) VALUES ('2', 'Yo-yo', 'Toys', '3.75');
INSERT INTO `products`.`products` (`id`, `name`, `category`, `price`) VALUES ('3', 'Hammer', 'Hardware', '16.99');
