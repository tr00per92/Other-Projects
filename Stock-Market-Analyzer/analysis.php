<?php
require('includes/DBconnect.php');

$tickerFile = fopen('tickers.txt', 'r');
while (!feof($tickerFile)) {
    $currentTicker = trim(fgets($tickerFile));

    // some variables to store the stock movement
    $nextDayIncrease = 0;
    $nextDayDecrease = 0;
    $total = 0;
    $sumOfIncreases = 0;
    $sumOfDecreases = 0;

    // getting the current stock from the database
    $sql = "SELECT date, percentChange FROM $currentTicker "
         . "WHERE percentChange < '0' ORDER BY date ASC";
    $q = $dbh->prepare($sql);
    $q->execute();
    while ($row = $q->fetch(PDO::FETCH_ASSOC)) {
        $date = $row['date'];
        $sql1 = "SELECT date, percentChange FROM $currentTicker "
              . "WHERE date > '$date' ORDER BY date ASC LIMIT 1";
        $q1 = $dbh->prepare($sql1);
        $q1->execute();
        if ($q1->rowCount() === 1) {
            $nextRow = $q1->fetch(PDO::FETCH_ASSOC);
            $nextPercentChange = $nextRow['percentChange'];
            $total++;
            if ($nextPercentChange > 0) {
                $nextDayIncrease++;
                $sumOfIncreases += $nextPercentChange;
            } else if ($nextPercentChange < 0) {
                $nextDayDecrease++;
                $sumOfDecreases += $nextPercentChange;
            }
        } else if ($q1->rowCount() !== 0) {
            echo 'Error';
        }
    }
    // some more calculations
    $nextDayIncreasePercent = ($nextDayIncrease / $total) * 100;
    $nextDayDecreasePercent = ($nextDayDecrease / $total) * 100;
    $averageIncreasePercent = $sumOfIncreases / $nextDayIncrease;
    $averageDecreasePercent = $sumOfDecreases / $nextDayDecrease;
    // place for custom trading formulas
    $buyValue = $nextDayDecreasePercent * $averageIncreasePercent;
    $sellValue = $nextDayDecreasePercent * $averageDecreasePercent;

    // adding results to DB
    $sql = "REPLACE INTO analysis (ticker, daysInc, percentOfDaysInc, avgIncPercent, "
              . "daysDec, percentOfDaysDec, avgDecPercent, buyValue, sellValue) "
         . "VALUES ('$currentTicker', '$nextDayIncrease', '$nextDayIncreasePercent', '$averageIncreasePercent', "
              . "'$nextDayDecrease', '$nextDayDecreasePercent', '$averageDecreasePercent', '$buyValue', '$sellValue')";
    $q = $dbh->prepare($sql);
    $q->execute();
}

echo 'Action Complete!';
