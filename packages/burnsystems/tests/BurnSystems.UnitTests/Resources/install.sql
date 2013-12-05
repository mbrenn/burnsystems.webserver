CREATE TABLE IF NOT EXISTS persons (
  Id INT(11) NOT NULL AUTO_INCREMENT,
  Prename VARCHAR(255) DEFAULT NULL,
  Name VARCHAR(255) DEFAULT NULL,
  Age INT(11) NOT NULL,
  Weight FLOAT NOT NULL,
  Sex VARCHAR(255) DEFAULT NULL,
  Marriage DATETIME DEFAULT NULL,
  PRIMARY KEY (Id)
) 