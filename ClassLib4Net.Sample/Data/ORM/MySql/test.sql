/*
Navicat MySQL Data Transfer

Source Server         : localhost
Source Server Version : 50721
Source Host           : localhost:3306
Source Database       : test

Target Server Type    : MYSQL
Target Server Version : 50721
File Encoding         : 65001

Date: 2018-04-06 16:21:49
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for epinfo
-- ----------------------------
DROP TABLE IF EXISTS `epinfo`;
CREATE TABLE `epinfo` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `epGuid` varchar(64) NOT NULL,
  `epName` text,
  `epDescription` text,
  `epShortName` varchar(30) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=35 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for keywordsreply
-- ----------------------------
DROP TABLE IF EXISTS `keywordsreply`;
CREATE TABLE `keywordsreply` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `type` varchar(255) COLLATE utf8_unicode_ci DEFAULT NULL,
  `keywords` text COLLATE utf8_unicode_ci,
  `ReplyContent` text COLLATE utf8_unicode_ci,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8 COLLATE=utf8_unicode_ci;

-- ----------------------------
-- Table structure for qrcode
-- ----------------------------
DROP TABLE IF EXISTS `qrcode`;
CREATE TABLE `qrcode` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `qrGuid` varchar(64) NOT NULL,
  `actionName` varchar(64) DEFAULT NULL,
  `SceneStr` varchar(64) DEFAULT NULL,
  `ticket` text,
  `url` text,
  `expireSeconds` int(11) DEFAULT NULL,
  `epid` varchar(64) DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for templet
-- ----------------------------
DROP TABLE IF EXISTS `templet`;
CREATE TABLE `templet` (
  `ID` int(11) NOT NULL AUTO_INCREMENT,
  `type` varchar(30) DEFAULT NULL,
  `text` text,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
  `ID` int(10) NOT NULL AUTO_INCREMENT,
  `userName` varchar(64) DEFAULT NULL,
  `openID` varchar(64) DEFAULT NULL,
  `nickName` varchar(64) DEFAULT NULL,
  `sex` int(11) DEFAULT NULL,
  `city` varchar(64) DEFAULT NULL,
  `country` varchar(64) DEFAULT NULL,
  `provice` varchar(64) DEFAULT NULL,
  `language` varchar(64) DEFAULT NULL,
  `headimgurl` text,
  `subscribe_time` datetime DEFAULT NULL,
  `unionid` varchar(64) DEFAULT NULL,
  `remark` varchar(64) DEFAULT NULL,
  `groupid` varchar(64) DEFAULT NULL,
  `tagid_list` text,
  `scene_str` varchar(64) DEFAULT NULL,
  `unsubscribe_time` datetime DEFAULT NULL,
  PRIMARY KEY (`ID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
