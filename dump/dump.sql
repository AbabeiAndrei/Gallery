-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               5.7.19-log - MySQL Community Server (GPL)
-- Server OS:                    Win64
-- HeidiSQL Version:             9.4.0.5125
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;


-- Dumping database structure for gallery
CREATE DATABASE IF NOT EXISTS `gallery` /*!40100 DEFAULT CHARACTER SET utf8 */;
USE `gallery`;

-- Dumping structure for table gallery.albums
CREATE TABLE IF NOT EXISTS `albums` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `created_by` int(11) NOT NULL,
  `privacy` smallint(6) NOT NULL,
  `row_state` smallint(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_albums_users_created_by` (`created_by`),
  CONSTRAINT `FK_albums_users_created_by` FOREIGN KEY (`created_by`) REFERENCES `users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.
-- Dumping structure for table gallery.files
CREATE TABLE IF NOT EXISTS `files` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `file_path` varchar(1024) NOT NULL,
  `thumbnail_path` varchar(1024) DEFAULT NULL,
  `extension` varchar(64) NOT NULL,
  `uploaded_by` int(11) NOT NULL,
  `uploaded_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `size` int(11) NOT NULL,
  `row_state` smallint(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_files_users_uploaded_by` (`uploaded_by`),
  CONSTRAINT `FK_files_users_uploaded_by` FOREIGN KEY (`uploaded_by`) REFERENCES `users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.
-- Dumping structure for table gallery.photos
CREATE TABLE IF NOT EXISTS `photos` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `name` varchar(255) NOT NULL DEFAULT '0',
  `album_id` int(11) DEFAULT NULL,
  `uploaded_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `uploaded_by` int(11) NOT NULL,
  `privacy` smallint(6) NOT NULL,
  `row_state` smallint(6) NOT NULL,
  PRIMARY KEY (`id`),
  KEY `FK_photos_albums_album_id` (`album_id`),
  KEY `FK_photos_users_uploaded_by` (`uploaded_by`),
  CONSTRAINT `FK_photos_albums_album_id` FOREIGN KEY (`album_id`) REFERENCES `albums` (`id`),
  CONSTRAINT `FK_photos_users_uploaded_by` FOREIGN KEY (`uploaded_by`) REFERENCES `users` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.
-- Dumping structure for table gallery.users
CREATE TABLE IF NOT EXISTS `users` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `email` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `full_name` varchar(255) NOT NULL,
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `row_state` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- Data exporting was unselected.
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IF(@OLD_FOREIGN_KEY_CHECKS IS NULL, 1, @OLD_FOREIGN_KEY_CHECKS) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
