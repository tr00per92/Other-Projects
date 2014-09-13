<?php
$hostname = 'localhost';
$dbName = 'stock_market';
$db_username = 'root';
$db_password = '';
$db_dsn = "mysql:host=$hostname; dbname=$dbName; charset=utf8";

try {
    $dbh = new PDO($db_dsn, $db_username, $db_password);
    $dbh->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
} catch (PDOException $e) {
    die($e->getMessage());
}
