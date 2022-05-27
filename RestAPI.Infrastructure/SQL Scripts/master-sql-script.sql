DROP DATABASE IF EXISTS `intel-rest-api-db`;

CREATE DATABASE `intel-rest-api-db`;

CREATE TABLE Customer (
    Id          INT NOT NULL AUTO_INCREMENT,
    Name      NVARCHAR(50) NOT NULL,
    DateAdded   DATETIME NOT NULL,
    DateUpdated DATETIME NULL,
    PRIMARY KEY (Id)
);

CREATE TABLE Purchase (
    Id              INT NOT NULL AUTO_INCREMENT,
    CustomerId      INT NOT NULL,
    OriginalCost    DECIMAL(8, 2) NOT NULL,
    FinalCost       DECIMAL(8, 2) NOT NULL,
    DiscountApplied DECIMAL(2, 2) NULL,
    DateAdded       DATETIME NOT NULL,
    DateUpdated     DATETIME NULL,
    PRIMARY KEY (Id)
);