<?php
require('includes/DBconnect.php');

set_time_limit(0);
$tickerFile = fopen('tickers.txt', 'r');
while (!feof($tickerFile)) {
    $currentTicker = trim(fgets($tickerFile));

    $currentFileURL = createURL($currentTicker);
    $currentTxtFile = "txtFiles/$currentTicker.txt";

    getCSV($currentFileURL, $currentTxtFile);
    fileToDB($currentTxtFile, $currentTicker);
}
fclose($tickerFile);
echo 'Action Complete!';

function createURL($ticker) {
    $startYear = 2010;
    $currentMonth = date('n') - 1;
    $currentDay = date('j');
    $currentYear = date('Y');
    return "http://real-chart.finance.yahoo.com/table.csv?s=$ticker&d=$currentMonth&e=$currentDay&f=$currentYear&g=d&a=00&b=1&c=$startYear&ignore=.csv";
}

function getCSV($url, $output) {
    $content = file_get_contents($url);
    $content = trim(str_replace('Date,Open,High,Low,Close,Volume,Adj Close', '', $content));
    file_put_contents($output, $content);
}

function fileToDB($txtFile, $tableName) {
    global $dbh;
    $sql = "CREATE TABLE IF NOT EXISTS $tableName (date DATE, PRIMARY KEY(date), "
            . "open FLOAT, high FLOAT, low FLOAT, close FLOAT, "
            . "volume INT, amountChange FLOAT, percentChange FLOAT) "
            . "COLLATE utf8_general_ci";
    $q = $dbh->prepare($sql);
    $q->execute();

    $file = fopen($txtFile, 'r');
    while (!feof($file)) {
        $line = fgets($file);
        $tokens = explode(',', $line);

        $date = $tokens[0];
        $open = $tokens[1];
        $high = $tokens[2];
        $low = $tokens[3];
        $close = $tokens[4];
        $volume = $tokens[5];
        $amountChange = $close - $open;
        $percentChange = ($amountChange / $open) * 100;

        $sql = "INSERT INTO $tableName (date, open, high, low, close, volume, amountChange, percentChange) "
                . "VALUES ('$date', '$open', '$high', '$low', '$close', '$volume', '$amountChange', '$percentChange')";
        $q = $dbh->prepare($sql);
        $q->execute();
    }
    fclose($file);
}
