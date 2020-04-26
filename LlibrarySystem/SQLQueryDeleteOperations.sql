DELETE FROM Operations;
dbcc checkident (Operations, reseed, 0);