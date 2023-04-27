-- Creates the sample database located in api/users.db

DROP TABLE IF EXISTS User;
DROP TABLE IF EXISTS Article;

CREATE TABLE User (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Guid      CHAR(36),
	Username  VARCHAR(255),
	PasswordHash VARCHAR(255),
	Firstname VARCHAR(255),
	Lastname  VARCHAR(255),
	Email     VARCHAR(255)
);

CREATE TABLE Article (
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Guid   CHAR(36),
	Content    VARCHAR(65535)
);

INSERT INTO Article (Guid, Content) VALUES ('46f5e025-fb38-4a28-83a2-692f96bf8174', 'Article 1');
INSERT INTO Article (Guid, Content) VALUES ('03b378a5-3f3c-4eaf-8b20-61c1a38ab7a0', 'Article 2');

