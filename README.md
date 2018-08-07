# ExchangeRatesService

Запустить постгрес из PostgreSQLPortable-10.1.1\PostgreSQLPortable.exe.

В в субд должна быть бд Valutes с таблицей Valute

DROP TABLE public."Valute";
CREATE TABLE public."Valute"
(
  "Name" text NOT NULL,
  "Date" date NOT NULL,
  "Rate" money,
  CONSTRAINT "Valute_pkey" PRIMARY KEY ("Date", "Name")
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public."Valute"
  OWNER TO postgres;
